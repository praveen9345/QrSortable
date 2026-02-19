namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.AccessManagement;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     Implementation of a backend synchronization to upload data from the general database.
    /// </summary>
    public class GeneralDatabaseSynchronizationManager : IGeneralDatabaseSynchronizationManager
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IGeneralInformationManager _generalInfoManager;
        private readonly IToastService _toast;
        private readonly IConnectivityService _connectivityService;
        private readonly ISharedMethodService _sharedMethodService;

        public GeneralDatabaseSynchronizationManager(IDatabaseManager databaseManager, IGeneralInformationManager generalInfoManager,
            IToastService toast, IConnectivityService connectivityService,
            ISharedMethodService sharedMethodService)
        {
            _databaseManager = databaseManager;
            _generalInfoManager = generalInfoManager;
            _toast = toast;
            _connectivityService = connectivityService;

            _sharedMethodService = sharedMethodService;
        }

        /// <summary>
        ///     Transfers all relevant data that was collected within the app to the backend.
        /// </summary>
        /// <returns>
        ///     True, if the last backup synchronization was updated successfully or if backend is not used. False, otherwise.
        /// </returns>
        public async Task<bool> SynchronizeAppDataAsync()
        {
            // Check internet connection
            var isInternetConnectionAvailable = await _connectivityService.CheckInternetConnectionAvailableAsync();
            if (!isInternetConnectionAvailable)
                return false;

            // Get general information
            if (!(await _generalInfoManager.GetGeneralInformationAsync()).IsBackendUsed)
                return false;

            // Get local storage entries
            var dbStorage = await _databaseManager.GetListAsync<StorageEntry>() ?? new List<StorageEntry>();

            // Get backend Firestore entries
            var db = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
            var collection = db.Collection("StorageEntries");

            var multiuserId = (await _generalInfoManager.GetGeneralInformationAsync()).MultiUserId;

            var querySnapshot = await collection
                .WhereEqualTo("MultiuserId", multiuserId)
                .GetSnapshotAsync();

            // Early return if both local and backend are empty
            if ((dbStorage == null || !dbStorage.Any()) && querySnapshot.Count == 0)
                return false;

            // Convert backend to dictionary
            var backendEntries = querySnapshot.Documents
                .Where(d => d.ContainsField("StorageId"))
                .ToDictionary(d => d.GetValue<string>("StorageId"), d => d);

            // Convert local to dictionary
            var localEntries = dbStorage.ToDictionary(e => _sharedMethodService.ConvertToString(e.StorageId));

            // Early return if data is identical
            if (backendEntries.Count == localEntries.Count)
            {
                bool allSame = true;

                foreach (var backendKvp in backendEntries)
                {
                    var storageId = backendKvp.Key;
                    var backendDoc = backendKvp.Value;

                    if (!localEntries.TryGetValue(storageId, out var localEntry))
                    {
                        allSame = false;
                        break;
                    }

                    // Download backend images
                    var backendImages = ReadImagesFromFirestore(backendDoc);

                    var backendCategory = backendDoc.GetValue<string>("Category");
                    var backendBarcodeValue = backendDoc.GetValue<string>("BarcodeValue");
                    var backendBarcodeType = backendDoc.GetValue<string>("BarcodeType");
                    var backendLocation = backendDoc.GetValue<string>("Location");
                    var backendSearchInfo = backendDoc.GetValue<string>("SearchInfo");
                    var backendItemName = backendDoc.GetValue<string>("ItemName");
                    var backendDescription = backendDoc.GetValue<string>("Description");
                    var backendBackgroundColorHex = backendDoc.GetValue<string>("BackgroundColorHex");
                    var compareImageLists = CompareImageLists(localEntry.ImageList, backendImages);

                    // Compare all fields including images
                    if (localEntry.Category != backendCategory || localEntry.BarcodeValue != backendBarcodeValue ||
                       localEntry.BarcodeType != backendBarcodeType || localEntry.Location != backendLocation ||
                       localEntry.SearchInfo != backendSearchInfo || localEntry.ItemName != backendItemName ||
                       localEntry.Description != backendDescription || !compareImageLists)
                    {
                        allSame = false;
                        break;
                    }
                }
                if (allSame)
                {
                    return true; // All data matches → skip sync
                }
            }

            // No backend → upload all
            if (querySnapshot.Count == 0)
            {
                foreach (var entry in dbStorage)
                {
                    dynamic dataDec = entry;
                    var document = new Dictionary<string, object>
                     {
                        { "MultiuserId",  multiuserId ?? string.Empty },
                        { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty },
                        { "Category", dataDec.Category ?? string.Empty },
                        { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty },
                        { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty },
                        { "BarcodeType", dataDec.BarcodeType ?? string.Empty },
                        { "Location", dataDec.Location ?? string.Empty },
                        { "SearchInfo", dataDec.SearchInfo ?? string.Empty },
                        { "ItemName", dataDec.ItemName ?? string.Empty },
                        { "Description", dataDec.Description ?? string.Empty },
                        { "ImageUrls",  dataDec.ImageList ??  new List<string>()},
                        { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty }
                    };

                    await collection.AddAsync(document);

                }
                return true;
            }

            // Backend exists → sync new & existing
            foreach (var entry in dbStorage)
            {
                var storageId = _sharedMethodService.ConvertToString(entry.StorageId) ?? string.Empty;

                backendEntries.TryGetValue(storageId, out var backendDoc);

                dynamic dataDec = entry;

                // Upload new images
                var document = new Dictionary<string, object>
                {
                    { "MultiuserId",  multiuserId ?? string.Empty },
                    { "StorageId", storageId },
                    { "Category", dataDec.Category ?? string.Empty },
                    { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty },
                    { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty },
                    { "BarcodeType", dataDec.BarcodeType ?? string.Empty },
                    { "Location", dataDec.Location ?? string.Empty },
                    { "SearchInfo", dataDec.SearchInfo ?? string.Empty },
                    { "ItemName", dataDec.ItemName ?? string.Empty },
                    { "Description", dataDec.Description ?? string.Empty },
                    { "ImageUrls",  dataDec.ImageList??  new List<string>()},
                    { "BackgroundColorHex", entry.BackgroundColorHex ?? string.Empty }
                };

                if (backendDoc == null)
                    await collection.AddAsync(document);
                else
                    await backendDoc.Reference.SetAsync(document, SetOptions.Overwrite);
            }

            // Sync backend → local database
            foreach (var backend in backendEntries)
            {
                var storageId = backend.Key;
                var doc = backend.Value;

                var images = ReadImagesFromFirestore(doc);

                if (images == null || images.Count == 0)
                {
                    await _toast.DisplayToast("Something went wrong. please try again.");
                    return false;
                }

                if (localEntries.TryGetValue(storageId, out var local))
                {
                    // Update existing local entry
                    local.Category = doc.GetValue<string>("Category");
                    local.CreatedDate = DateTime.Parse(doc.GetValue<string>("CreatedDate"));
                    local.BarcodeValue = doc.GetValue<string>("BarcodeValue");
                    local.BarcodeType = doc.GetValue<string>("BarcodeType");
                    local.Location = doc.GetValue<string>("Location");
                    local.SearchInfo = doc.GetValue<string>("SearchInfo");
                    local.ItemName = doc.GetValue<string>("ItemName");
                    local.Description = doc.GetValue<string>("Description");
                    local.BackgroundColorHex = doc.GetValue<string>("BackgroundColorHex");
                    local.ImageList = images;

                    await _databaseManager.UpdateAsync(local);
                }
                else
                {
                    // Add new local entry
                    var newEntry = new StorageEntry
                    {
                        StorageId = Guid.Parse(storageId),
                        Category = doc.GetValue<string>("Category"),
                        CreatedDate = DateTime.Parse(doc.GetValue<string>("CreatedDate")),
                        BarcodeValue = doc.GetValue<string>("BarcodeValue"),
                        BarcodeType = doc.GetValue<string>("BarcodeType"),
                        Location = doc.GetValue<string>("Location"),
                        SearchInfo = doc.GetValue<string>("SearchInfo"),
                        ItemName = doc.GetValue<string>("ItemName"),
                        Description = doc.GetValue<string>("Description"),
                        BackgroundColorHex = doc.GetValue<string>("BackgroundColorHex"),
                        ImageList = images
                    };

                    _databaseManager.BeginTransaction();
                    var addedItem = await _databaseManager.AddAsync(newEntry);
                    if (addedItem != null)
                        _databaseManager.CommitTransaction();
                    else
                        _databaseManager.Rollback();
                }
            }

            // Delete local entries removed from backend
            foreach (var local in dbStorage)
            {
                var storageId = _sharedMethodService.ConvertToString(local.StorageId);
                if (!backendEntries.ContainsKey(storageId))
                {
                    await _databaseManager.DeleteAsync(local);
                }
            }

            return true;
        }

        public async Task<bool> ClearBackendAndSyncLocalDataAsync()
        {
            // Check internet connection
            if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                return false;

            // Reset only allowed when backend is NOT used
            if (!(await _generalInfoManager.GetGeneralInformationAsync()).IsBackendUsed)
                return false;

            try
            {
                // Create Firestore once
                var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
                var collection = firestoreDb.Collection("StorageEntries");

                var multiuserId = (await _generalInfoManager.GetGeneralInformationAsync()).MultiUserId;

                //Get backend entries
                var querySnapshot = await collection
                    .WhereEqualTo("MultiuserId", multiuserId)
                    .GetSnapshotAsync();

                if (querySnapshot.Count != 0)
                {
                    //Delete backend entries (batch-safe)
                    const int batchSize = 500;
                    var documents = querySnapshot.Documents;
                    int deletedCount = 0;

                    while (deletedCount < documents.Count)
                    {
                        var batch = firestoreDb.StartBatch();

                        foreach (var document in documents.Skip(deletedCount).Take(batchSize))
                        {

                            // Delete Firestore document
                            batch.Delete(document.Reference);
                        }

                        await batch.CommitAsync();
                        deletedCount += batchSize;
                    }

                }

                //Upload all local entries
                var dbStorage = await _databaseManager.GetListAsync<StorageEntry>();
                if (dbStorage == null || !dbStorage.Any())
                    return false;

                foreach (var entry in dbStorage)
                {
                    dynamic dataDec = entry;
                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", multiuserId ?? string.Empty },
                        { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty },
                        { "Category", dataDec.Category ?? string.Empty },
                        { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty },
                        { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty },
                        { "BarcodeType", dataDec.BarcodeType ?? string.Empty },
                        { "Location", dataDec.Location ?? string.Empty },
                        { "SearchInfo", dataDec.SearchInfo ?? string.Empty },
                        { "ItemName", dataDec.ItemName ?? string.Empty },
                        { "Description", dataDec.Description ?? string.Empty },
                        { "ImageUrls",  dataDec.ImageList ?? new List<string>() },
                        { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty }
                    };

                    await collection.AddAsync(document);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Centralized error handling
                Console.WriteLine(
                    $"SynchronizeAppByResetBackendAsync failed for MultiUserId : {ex}");

                return false;
            }
        }

        private bool CompareImageLists(IList<byte[]> localImages, IList<byte[]> backendImages)
        {
            if (localImages.Count != backendImages.Count)
                return false;

            for (int i = 0; i < localImages.Count; i++)
            {
                if (!localImages[i].SequenceEqual(backendImages[i]))
                    return false;
            }

            return true;
        }

        private List<byte[]> ReadImagesFromFirestore(DocumentSnapshot doc)
        {
            var images = new List<byte[]>();

            if (!doc.ContainsField("ImageUrls"))
                return images;

            var rawList = doc.GetValue<IList<object>>("ImageUrls");

            foreach (var item in rawList)
            {
                switch (item)
                {
                    case string base64:
                        images.Add(Convert.FromBase64String(base64));
                        break;

                    case byte[] bytes:
                        images.Add(bytes);
                        break;

                    case Google.Cloud.Firestore.Blob blob:
                        // Try these different approaches for different SDK versions
                        byte[] blobBytes = null;

                        // Approach 1: Check if ToByteArray() exists (older versions)
                        var toByteArrayMethod = blob.GetType().GetMethod("ToByteArray");
                        if (toByteArrayMethod != null)
                        {
                            blobBytes = (byte[])toByteArrayMethod.Invoke(blob, null);
                        }
                        // Approach 2: Check if ByteString property exists (some versions)
                        else if (blob.GetType().GetProperty("ByteString") != null)
                        {
                            dynamic dynamicBlob = blob;
                            var byteString = dynamicBlob.ByteString;
                            blobBytes = byteString.ToByteArray();
                        }
                        // Approach 3: Check if Bytes property exists (some versions)
                        else if (blob.GetType().GetProperty("Bytes") != null)
                        {
                            dynamic dynamicBlob = blob;
                            var memory = (ReadOnlyMemory<byte>)dynamicBlob.Bytes;
                            blobBytes = memory.ToArray();
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Unable to convert Blob to byte array. Available properties: " +
                                string.Join(", ", blob.GetType().GetProperties().Select(p => p.Name))
                            );
                        }

                        images.Add(blobBytes);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported ImageUrls type: {item.GetType()}"
                        );
                }
            }

            return images;
        }
    }
}
