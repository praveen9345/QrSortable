namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
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
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ISharedMethodService _sharedMethodService;

        private string _multiUserId = string.Empty;

        public GeneralDatabaseSynchronizationManager(IDatabaseManager databaseManager,IGeneralInformationManager generalInfoManager,
            IToastService toast, IConnectivityService connectivityService,IFirebaseStorageService firebaseStorageService,
            ISharedMethodService sharedMethodService)
        {
            _databaseManager = databaseManager;
            _generalInfoManager = generalInfoManager;
            _toast = toast;
            _connectivityService = connectivityService;
            _firebaseStorageService = firebaseStorageService;
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
            // Step 0: Check internet connection
            var isInternetConnectionAvailable = await _connectivityService.CheckInternetConnectionAvailableAsync();
            if (!isInternetConnectionAvailable)
                return false;

            // Step 1: Get general information
            var generalInformation = await _generalInfoManager.GetGeneralInformationAsync();
            _multiUserId = generalInformation.MultiUserId;

            if (generalInformation.IsBackendUsed)
                return false;

            // Step 2: Get local storage entries
            var dbStorage = await _databaseManager.GetListAsync<StorageEntry>() ?? new List<StorageEntry>();

            // Step 3: Get backend Firestore entries
            var Db = FirestoreDb.Create(Configuration.FirebaseConfig.PROJECT_ID);
            var collection = Db.Collection("StorageEntries");

            var querySnapshot = await collection
                .WhereEqualTo("MultiuserId", _multiUserId)
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

            // Step 4: Early return if data is identical
            if(backendEntries.Count == localEntries.Count) 
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
                    var backendImageUrls = backendDoc.ContainsField("ImageUrls")
                        ? backendDoc.GetValue<List<string>>("ImageUrls"): new List<string>();

                    var backendImages = await _firebaseStorageService.DownloadImagesAsync(backendImageUrls);

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
            
            // Step 5: No backend → upload all
            if (querySnapshot.Count == 0)
            {
                foreach (var entry in dbStorage)
                {
                    var imageUrls = await _firebaseStorageService.UploadImagesAsync(entry.ImageList);

                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", _multiUserId ?? string.Empty },
                        { "StorageId", _sharedMethodService.ConvertToString(entry.StorageId) ?? string.Empty },
                        { "Category", entry.Category ?? string.Empty },
                        { "CreatedDate", _sharedMethodService.ConvertToString(entry.CreatedDate) ?? string.Empty },
                        { "BarcodeValue", entry.BarcodeValue ?? string.Empty },
                        { "BarcodeType", entry.BarcodeType ?? string.Empty },
                        { "Location", entry.Location ?? string.Empty },
                        { "SearchInfo", entry.SearchInfo ?? string.Empty },
                        { "ItemName", entry.ItemName ?? string.Empty },
                        { "Description", entry.Description ?? string.Empty },
                        { "ImageUrls", imageUrls ?? new List<string>() },
                        { "BackgroundColorHex", entry.BackgroundColorHex ?? string.Empty }
                    };

                    await collection.AddAsync(document);
                }
                return true;
            }

            // Step 6: Backend exists → sync new & existing
            foreach (var entry in dbStorage)
            {
                var storageId = _sharedMethodService.ConvertToString(entry.StorageId) ?? string.Empty;

                backendEntries.TryGetValue(storageId, out var backendDoc);

                // Delete old images if entry exists
                if (backendDoc != null && backendDoc.ContainsField("ImageUrls"))
                {
                    var oldImageUrls = backendDoc.GetValue<List<string>>("ImageUrls")
                        ?.Where(u => !string.IsNullOrWhiteSpace(u))
                        .ToList();

                    if (oldImageUrls != null && oldImageUrls.Count > 0)
                        await _firebaseStorageService.DeleteImagesAsync(oldImageUrls);
                }

                // Upload new images
                var imageUrls = await _firebaseStorageService.UploadImagesAsync(entry.ImageList);

                var document = new Dictionary<string, object>
        {
            { "MultiuserId", _multiUserId ?? string.Empty },
            { "StorageId", storageId },
            { "Category", entry.Category ?? string.Empty },
            { "CreatedDate", _sharedMethodService.ConvertToString(entry.CreatedDate) ?? string.Empty },
            { "BarcodeValue", entry.BarcodeValue ?? string.Empty },
            { "BarcodeType", entry.BarcodeType ?? string.Empty },
            { "Location", entry.Location ?? string.Empty },
            { "SearchInfo", entry.SearchInfo ?? string.Empty },
            { "ItemName", entry.ItemName ?? string.Empty },
            { "Description", entry.Description ?? string.Empty },
            { "ImageUrls", imageUrls ?? new List<string>() },
            { "BackgroundColorHex", entry.BackgroundColorHex ?? string.Empty }
        };

                if (backendDoc == null)
                    await collection.AddAsync(document);
                else
                    await backendDoc.Reference.SetAsync(document, SetOptions.Overwrite);
            }

            // Step 7: Sync backend → local database
            foreach (var backend in backendEntries)
            {
                var storageId = backend.Key;
                var doc = backend.Value;

                var imageUrls = doc.ContainsField("ImageUrls") ? doc.GetValue<List<string>>("ImageUrls") : new List<string>();
                var images = await _firebaseStorageService.DownloadImagesAsync(imageUrls);

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

            // Step 8: Delete local entries removed from backend
            foreach (var local in dbStorage)
            {
                var storageId = _sharedMethodService.ConvertToString(local.StorageId);
                if (!backendEntries.ContainsKey(storageId))
                    await _databaseManager.DeleteAsync(local);
            }

            return true;
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
    }
}