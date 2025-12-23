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
            var isInternetConnectionAvailable = await _connectivityService.CheckInternetConnectionAvailableAsync();
            if (!isInternetConnectionAvailable)
            {
                return false;
            }

            var generalInformation = await _generalInfoManager.GetGeneralInformationAsync();
            
            _multiUserId = generalInformation.MultiUserId;

            if (!generalInformation.IsBackendUsed)
            {
                return false;
            }

            var dbStorage = await _databaseManager.GetListAsync<StorageEntry>();

            if (dbStorage is null || !dbStorage.Any())
            {
                return false;
            }

            var Db = FirestoreDb.Create(Configuration.FirebaseConfig.PROJECT_ID);
            var collection = Db.Collection("StorageEntries");

            // Step 1: Get all documents matching MultiuserId
            var querySnapshot = await collection
                .WhereEqualTo("MultiuserId", _multiUserId)
                .GetSnapshotAsync();

            // Convert Firestore documents into lookup dictionary
            var backendEntries = querySnapshot.Documents
               .Where(d => d.ContainsField("StorageId"))
               .ToDictionary( d => d.GetValue<string>("StorageId"),d => d);

            // STEP 2: No backend data → upload everything
            if (querySnapshot.Count == 0)
            {
                foreach (var entry in dbStorage)
                {
                    var imageUrls = await _firebaseStorageService.UploadImagesAsync(entry.ImageList);

                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", _multiUserId ?? string.Empty },
                        { "StorageId", _sharedMethodService.ConvertToString(entry.StorageId) ?? string.Empty},
                        { "Category", entry.Category ?? string.Empty},
                        { "CreatedDate", _sharedMethodService.ConvertToString(entry.CreatedDate) ?? string.Empty},
                        { "BarcodeValue", entry.BarcodeValue ?? string.Empty},
                        { "BarcodeType", entry.BarcodeType ?? string.Empty},
                        { "Location", entry.Location ?? string.Empty},
                        { "SearchInfo", entry.SearchInfo ?? string.Empty},
                        { "ItemName", entry.ItemName ?? string.Empty},
                        { "Description", entry.Description ?? string.Empty},
                        { "ImageUrls", imageUrls ?? new List<string>()},
                        { "BackgroundColorHex", entry.BackgroundColorHex ?? string.Empty}
                    };

                    await collection.AddAsync(document);
                }
                return true;
            }

            // STEP 3. Backend exists → sync new & existing entries
            foreach (var entry in dbStorage)
            {
                var storageId =
                    _sharedMethodService.ConvertToString(entry.StorageId) ?? string.Empty;

                // EXISTING backend entry
                backendEntries.TryGetValue(storageId, out var backendDoc);

                // Delete old images if entry exists
                if (backendDoc != null && backendDoc.ContainsField("ImageUrls"))
                {
                    var oldImageUrls = backendDoc
                        .GetValue<List<string>>("ImageUrls")
                        ?.Where(u => !string.IsNullOrWhiteSpace(u))
                        .ToList();

                    if (oldImageUrls != null && oldImageUrls.Count > 0)
                    {
                        await _firebaseStorageService.DeleteImagesAsync(oldImageUrls);
                    }
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

                // Create or update Firestore document
                if (backendDoc == null)
                {
                    // NEW entry
                    await collection.AddAsync(document);
                }
                else
                {
                    // UPDATE existing entry
                    await backendDoc.Reference.SetAsync(document, SetOptions.Overwrite);
                }
            }

            return true;
        }
    }
}