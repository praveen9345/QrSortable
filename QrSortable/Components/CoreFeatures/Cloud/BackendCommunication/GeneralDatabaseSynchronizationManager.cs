namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.DataManagement;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using QrSortable.Components.UiFunctionality.Notification;

    public class GeneralDatabaseSynchronizationManager : IGeneralDatabaseSynchronizationManager
    {
        private readonly IDatabaseManager _database;
        private readonly IBackendCommunicationService _backend;
        private readonly IGeneralInformationManager _generalInfoManager;
        private readonly IToastService _toast;

        public GeneralDatabaseSynchronizationManager(
            IDatabaseManager database,
            IBackendCommunicationService backend,
            IGeneralInformationManager generalInfoManager,
            IToastService toast)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            _generalInfoManager = generalInfoManager ?? throw new ArgumentNullException(nameof(generalInfoManager));
            _toast = toast;
        }

        public async Task<bool> UploadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var generalInfo = await _generalInfoManager.GetGeneralInformationAsync();
                var multiUserId = generalInfo?.MultiUserId ?? string.Empty;
                if (string.IsNullOrWhiteSpace(multiUserId))
                {
                    // Make sure multiuser id exists; try to generate
                    var ok = await _generalInfoManager.GenerateTheMultiuserIdAsync();
                    if (!ok) { _toast?.DisplayToast("Failed creating device id for sync."); return false; }
                    generalInfo = await _generalInfoManager.GetGeneralInformationAsync();
                    multiUserId = generalInfo.MultiUserId;
                }

                // Upload StorageEntry
                bool storageOk = await UploadEntitiesAsync<StorageEntry>(entity =>
                {
                    var dto = new DtoStorageEntryModel
                    {
                        StorageId = entity.StorageId.ToString(),
                        Category = entity.Category ?? string.Empty,
                        CreatedDate = entity.CreatedDate.ToString("o"),
                        BarcodeValue = entity.BarcodeValue ?? string.Empty,
                        BarcodeType = entity.BarcodeType ?? string.Empty,
                        Location = entity.Location ?? string.Empty,
                        SearchInfo = entity.SearchInfo ?? string.Empty,
                        ItemName = entity.ItemName ?? string.Empty,
                        Description = entity.Description ?? string.Empty,
                        ImageList = entity.ImageList?.ToList() ?? new List<byte[]>(),
                        BackgroundColorHex = entity.BackgroundColorHex ?? string.Empty
                    };
                    SetMultiuserId(dto, multiUserId);
                    return dto;
                });

                // Upload Orders
                bool ordersOk = await UploadEntitiesAsync<YoursOrderData>(entity =>
                {
                    var dto = new DtoOrdersModel
                    {
                        OrderId = entity.OrderId ?? string.Empty,
                        Title = entity.Title ?? string.Empty,
                        Description = entity.Description ?? string.Empty,
                        CodeType = entity.CodeType ?? string.Empty,
                        PageType = entity.PageType ?? string.Empty,
                        ProductQuantity = entity.ProductQuantity.ToString(),
                        DateTime = entity.DateTime.ToString("o"),
                        TotalPrice = entity.TotalPrice ?? string.Empty,
                        Name = entity.Name ?? string.Empty,
                        Street = entity.Street ?? string.Empty,
                        HouseNo = entity.HouseNo ?? string.Empty,
                        ZipCode = entity.ZipCode ?? string.Empty,
                        City = entity.City ?? string.Empty,
                        Country = entity.Country ?? string.Empty,
                        Email = entity.Email ?? string.Empty,
                        ReferenceCode = entity.ReferenceCode ?? string.Empty,
                        ShipmentTracking = entity.ShipmentTracking ?? string.Empty,
                        StatusOfOrder = entity.StatusOfOrder ?? string.Empty,
                        PdfFiles = entity.PdfFiles ?? new List<byte[]>()
                    };
                    SetMultiuserId(dto, multiUserId);
                    return dto;
                });

                var overall = storageOk && ordersOk;
                if (overall) _toast?.DisplayToast("Upload finished successfully.");
                return overall;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GeneralDatabaseSynchronizationManager.UploadAllAsync: {ex}");
                _toast?.DisplayToast("Upload failed.");
                return false;
            }
        }

        public async Task<bool> UploadEntitiesAsync<T>(Func<T, DtoFirestoreData> mapper)
            where T : DatabaseEntry
        {
            try
            {
                var list = await _database.GetListAsync<T>();
                if (list == null || !list.Any()) return true;

                foreach (var entity in list)
                {
                    var dto = mapper(entity);
                    if (dto == null) continue;

                    try
                    {
                        await _backend.InsertAsync(dto);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"UploadEntitiesAsync<{typeof(T).Name}>: failed: {ex}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UploadEntitiesAsync<{typeof(T).Name}>: {ex}");
                return false;
            }
        }

        // Helper to set get-only auto-property MultiuserId via reflection
        private static void SetMultiuserId(object dto, string id)
        {
            if (dto == null || id == null) return;

            // find backing field for the auto-property
            var field = dto.GetType().GetField("<MultiuserId>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(dto, id);
                return;
            }

            // fallback: try property set via reflection if a setter exists
            var prop = dto.GetType().GetProperty("MultiuserId");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(dto, id);
            }
        }
    }
}