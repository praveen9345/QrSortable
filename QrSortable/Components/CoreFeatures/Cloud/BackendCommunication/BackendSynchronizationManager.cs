namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.DataManagement;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Notification;

    /// <summary>
    /// Connectivity-aware queue + persistent retry.
    /// Uploads persisted queue entries on startup/resume and removes them from DB when uploaded.
    /// </summary>
    public class BackendSynchronizationManager : IBackendSynchronizationManager, IDisposable
    {
        private readonly IBackendCommunicationService _backend;
        private readonly IMauiEssentialsWrapper _mauiWrapper;
        private readonly IToastService _toastService;
        private readonly IDatabaseManager _databaseManager;

        private readonly ConcurrentQueue<QueueItem> _queue = new();
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);
        private CancellationTokenSource _cts = new();

        private bool _initialized;

        private const int MaxRetriesPerItem = 5;
        private readonly TimeSpan InitialBackoff = TimeSpan.FromSeconds(2);

        // Simple wrapper to hold persistent DB entity + in-memory DTO
        private class QueueItem
        {
            public UploadQueueEntry Entry { get; set; }
            public DtoFirestoreData Dto { get; set; }
        }

        public BackendSynchronizationManager(
            IBackendCommunicationService backendCommunicationService,
            IMauiEssentialsWrapper mauiWrapper,
            IToastService toastService,
            IDatabaseManager databaseManager)
        {
            _backend = backendCommunicationService ?? throw new ArgumentNullException(nameof(backendCommunicationService));
            _mauiWrapper = mauiWrapper ?? throw new ArgumentNullException(nameof(mauiWrapper));
            _toastService = toastService;
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;
            _initialized = true;

            _mauiWrapper.ConnectivityChanged += OnConnectivityChanged;

            // Load persisted queue entries into memory
            try
            {
                var persisted = await _databaseManager.GetListAsync<UploadQueueEntry>();
                if (persisted != null)
                {
                    foreach (var p in persisted.OrderBy(x => x.QueuedAt))
                    {
                        var dto = DeserializeDtoFromPayload(p);
                        if (dto != null)
                        {
                            _queue.Enqueue(new QueueItem { Entry = p, Dto = dto });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendSynchronizationManager.Initialize: loading persisted queue failed: {ex}");
            }

            _ = Task.Run(() => TryProcessQueueAsync(_cts.Token));
        }

        public async Task EnqueueAsync(DtoFirestoreData dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // ensure multiuser id
            var multiId = dto.MultiuserId;
            if (string.IsNullOrWhiteSpace(multiId))
            {
                // try to obtain from GeneralInformation if available in DB (avoid required dependency)
                try
                {
                    var generalInfo = await _databaseManager.GetListAsync<GeneralInformation>();
                    var gi = generalInfo?.FirstOrDefault();
                    if (gi != null && !string.IsNullOrWhiteSpace(gi.MultiUserId))
                        multiId = gi.MultiUserId;
                }
                catch { /* ignore */ }
            }

            var payload = JsonSerializer.Serialize(dto);
            var entry = new UploadQueueEntry
            {
                MultiuserId = multiId ?? string.Empty,
                CollectionName = dto.CollectionName,
                DtoTypeName = dto.GetType().FullName,
                Payload = payload,
                RetryCount = 0,
                QueuedAt = DateTime.UtcNow
            };

            try
            {
                var added = await _databaseManager.AddAsync(entry);
                if (added != null)
                {
                    // Db returned entity with ID: load that exact instance reference if needed
                    _queue.Enqueue(new QueueItem { Entry = added, Dto = dto });
                }
                else
                {
                    // fallback: still push in-memory
                    _queue.Enqueue(new QueueItem { Entry = entry, Dto = dto });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendSynchronizationManager.EnqueueAsync: persist failed: {ex}");
                // fallback: just enqueue in memory (non-persistent)
                _queue.Enqueue(new QueueItem { Entry = entry, Dto = dto });
            }

            // kick processing if internet available
            if (_mauiWrapper.IsInternetConnectionAvailable())
                _ = Task.Run(() => TryProcessQueueAsync(_cts.Token));
        }

        public async Task ForceProcessQueueAsync()
        {
            await TryProcessQueueAsync(_cts.Token);
        }

        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private void OnConnectivityChanged(object sender, EventArgs e)
        {
            if (_mauiWrapper.IsInternetConnectionAvailable())
            {
                _ = Task.Run(() => TryProcessQueueAsync(_cts.Token));
            }
        }

        private async Task TryProcessQueueAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (!await _processingSemaphore.WaitAsync(0))
            {
                // already processing
                return;
            }

            try
            {
                while (!_queue.IsEmpty && !token.IsCancellationRequested)
                {
                    if (!_mauiWrapper.IsInternetConnectionAvailable())
                        break;

                    if (!_queue.TryDequeue(out var item))
                        break;

                    var success = await TryUploadWithRetriesAsync(item, token);
                    if (!success)
                    {
                        // increment retry count in DB, re-enqueue, and back off
                        try
                        {
                            item.Entry.RetryCount++;
                            await _databaseManager.UpdateAsync(item.Entry);
                        }
                        catch { }

                        _queue.Enqueue(item);
                        await Task.Delay(TimeSpan.FromSeconds(5), token).ContinueWith(_ => { });
                        break;
                    }
                    else
                    {
                        // remove persisted DB entry after successful upload
                        try
                        {
                            await _databaseManager.DeleteAsync(item.Entry);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"BackendSynchronizationManager: cannot delete persisted queue entry: {ex}");
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendSynchronizationManager: Unexpected exception: {ex}");
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        private async Task<bool> TryUploadWithRetriesAsync(QueueItem item, CancellationToken token)
        {
            int attempt = 0;
            TimeSpan backoff = InitialBackoff;

            while (attempt < MaxRetriesPerItem && !token.IsCancellationRequested)
            {
                attempt++;
                try
                {
                    await _backend.InsertAsync(item.Dto);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BackendSynchronizationManager: Upload attempt {attempt} for entry {item.Entry?.ID} failed: {ex.Message}");
                    if (attempt >= MaxRetriesPerItem)
                    {
                        _toastService?.DisplayToast("Background upload failed. It will be retried when connection restores.");
                        return false;
                    }

                    try { await Task.Delay(backoff, token); } catch { }
                    backoff = backoff + backoff;
                }
            }

            return false;
        }

        // try to map Payload back to a Dto; prefer CollectionName mapping, else try DtoTypeName
        private DtoFirestoreData DeserializeDtoFromPayload(UploadQueueEntry entry)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.Payload)) return null;

            try
            {
                // known collection mapping
                if (string.Equals(entry.CollectionName, "StorageEntries", StringComparison.OrdinalIgnoreCase))
                {
                    return JsonSerializer.Deserialize<DtoStorageEntryModel>(entry.Payload);
                }

                if (string.Equals(entry.CollectionName, "Orders", StringComparison.OrdinalIgnoreCase))
                {
                    return JsonSerializer.Deserialize<DtoOrdersModel>(entry.Payload);
                }

                // fallback: try to use Type name if present
                if (!string.IsNullOrWhiteSpace(entry.DtoTypeName))
                {
                    var t = Type.GetType(entry.DtoTypeName);
                    if (t != null && typeof(DtoFirestoreData).IsAssignableFrom(t))
                    {
                        var obj = JsonSerializer.Deserialize(entry.Payload, t);
                        return obj as DtoFirestoreData;
                    }
                }

                // last-ditch: try Orders DTO then Storage DTO
                try { return JsonSerializer.Deserialize<DtoOrdersModel>(entry.Payload); } catch { }
                try { return JsonSerializer.Deserialize<DtoStorageEntryModel>(entry.Payload); } catch { }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeserializeDtoFromPayload: {ex}");
            }

            return null;
        }

        public void Dispose()
        {
            _mauiWrapper.ConnectivityChanged -= OnConnectivityChanged;
            _cts.Cancel();
            _processingSemaphore.Dispose();
            _cts.Dispose();
        }
    }
}