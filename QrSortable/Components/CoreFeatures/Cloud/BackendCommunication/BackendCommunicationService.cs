namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.PlatformUtils;
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BackendCommunicationService : IBackendCommunicationService
    {
        private readonly IAesHelper _aesHelper;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private FirestoreDb Db { get; set; }

        public BackendCommunicationService(
            IAesHelper aesHelper,
            IFirebaseStorageService firebaseStorageService)
        {
            Db = FirestoreDb.Create(Configuration.Constants.PROJECT_ID);
            _aesHelper = aesHelper;
            _firebaseStorageService = firebaseStorageService;
        }

        // ============================================================
        // INSERT (RETURNS TRUE / FALSE)
        // ============================================================
        public async Task<bool> InsertAsync<T>(T data) where T : DtoFirestoreData
        {
            Validate(data);

            // ============================
            // ORDERS
            // ============================
            if (data is DtoOrdersModel ordersDto)
            {
                var document = new Dictionary<string, object>
                {
                    { "MultiuserId", ordersDto.MultiuserId ?? string.Empty },
                    { "OrderId", ordersDto.OrderId },
                    { "Title", ordersDto.Title },
                    { "Description", ordersDto.Description },
                    { "CodeType", ordersDto.CodeType },
                    { "PageType", ordersDto.PageType },
                    { "ProductQuantity", ordersDto.ProductQuantity },
                    { "DateTime", ordersDto.DateTime },
                    { "TotalPrice", ordersDto.TotalPrice },
                    { "Name", ordersDto.Name },
                    { "Street", ordersDto.Street },
                    { "HouseNo", ordersDto.HouseNo },
                    { "ZipCode", ordersDto.ZipCode },
                    { "City", ordersDto.City },
                    { "Country", ordersDto.Country },
                    { "Email", ordersDto.Email },
                    { "ReferenceCode", ordersDto.ReferenceCode },
                    { "ShipmentTracking", ordersDto.ShipmentTracking },
                    { "StatusOfOrder", ordersDto.StatusOfOrder },
                    { "PdfFiles", ordersDto.PdfFiles ?? new List<byte[]>() }
                };

                try
                {
                    await Db.Collection(ordersDto.CollectionName).AddAsync(document);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Insert Orders failed: {ex}");
                    return false;
                }
            }

            // ============================
            // STORAGE ENTRIES
            // ============================
            if (data is DtoStorageEntryModel storageDto)
            {
                var document = new Dictionary<string, object>
                {
                    { "MultiuserId", storageDto.MultiuserId ?? string.Empty },
                    { "StorageId", storageDto.StorageId },
                    { "Category", storageDto.Category },
                    { "CreatedDate", storageDto.CreatedDate },
                    { "BarcodeValue", storageDto.BarcodeValue },
                    { "BarcodeType", storageDto.BarcodeType },
                    { "Location", storageDto.Location },
                    { "SearchInfo", storageDto.SearchInfo },
                    { "ItemName", storageDto.ItemName },
                    { "Description", storageDto.Description },
                    { "ImageUrls", storageDto.ImageUrls },
                    { "BackgroundColorHex", storageDto.BackgroundColorHex }
                };

                try
                {
                    await Db.Collection(storageDto.CollectionName).AddAsync(document);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Insert Storage failed: {ex}");
                    return false;
                }
            }

            // ============================
            // FALLBACK (ENCRYPTED DTO)
            // ============================
            try
            {
                var encryptedJson = EncryptData(data);

                var doc = new Dictionary<string, object>
                {
                    { "MultiuserId", data.MultiuserId ?? string.Empty },
                    { "EncryptedData", encryptedJson }
                };

                await Db.Collection(data.CollectionName).AddAsync(doc);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertAsync failed: {ex}");

                // enqueue for retry
                try
                {
                    var backendSync = ServiceHelper.GetService<IBackendSynchronizationManager>();
                    if (backendSync != null)
                    {
                        await backendSync.EnqueueAsync(data);
                    }
                }
                catch (Exception enqueueEx)
                {
                    Console.WriteLine($"Enqueue failed: {enqueueEx}");
                }

                return false;
            }
        }

        // ============================================================
        // GET
        // ============================================================
        public async Task<T?> GetAsync<T>(string id) where T : DtoFirestoreData, new()
        {
            var temp = new T();
            var docRef = Db.Collection(temp.CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return null;

            try
            {
                if (snapshot.ContainsField("EncryptedData"))
                {
                    var encryptedJson = snapshot.GetValue<string>("EncryptedData");
                    return DecryptData<T>(encryptedJson);
                }

                return snapshot.ConvertTo<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAsync failed: {ex}");
                return null;
            }
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public async Task<bool> UpdateAsync<T>(T data) where T : DtoFirestoreData
        {
            Validate(data);

            try
            {
                if (data is DtoOrdersModel ordersDto)
                {
                    var collection = Db.Collection(ordersDto.CollectionName);

                    var querySnapshot = await collection
                        .WhereEqualTo("MultiuserId", ordersDto.MultiuserId)
                        .WhereEqualTo("OrderId", ordersDto.OrderId)
                        .GetSnapshotAsync();

                    if (querySnapshot.Count != 1)
                        return false;

                    await collection
                        .Document(querySnapshot.Documents[0].Id)
                        .SetAsync(ordersDto, SetOptions.Overwrite);

                    return true;
                }

                if (data is DtoStorageEntryModel storageDto)
                {
                    var collection = Db.Collection(storageDto.CollectionName);

                    var querySnapshot = await collection
                        .WhereEqualTo("MultiuserId", storageDto.MultiuserId)
                        .GetSnapshotAsync();

                    if (querySnapshot.Count == 0)
                        return false;

                    await collection
                        .Document(querySnapshot.Documents[0].Id)
                        .SetAsync(storageDto, SetOptions.Overwrite);

                    return true;
                }

                var encryptedJson = EncryptData(data);

                await Db.Collection(data.CollectionName)
                        .Document(data.MultiuserId)
                        .SetAsync(new Dictionary<string, object>
                        {
                            { "MultiuserId", data.MultiuserId },
                            { "EncryptedData", encryptedJson }
                        }, SetOptions.Overwrite);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateAsync failed: {ex}");
                return false;
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task DeleteAsync<T>(string id, string collectionName)
        {
            //TDOD: delete the associated images from Firebase Storage too uing multuserId and id
            await Db.Collection(collectionName).Document(id).DeleteAsync();
        }

        // ============================================================
        // GET LISTS
        // ============================================================
        public async Task<List<T>> GetAllAsync<T>() where T : DtoFirestoreData, new()
        {
            var temp = new T();
            var snapshot = await Db.Collection(temp.CollectionName).GetSnapshotAsync();

            var result = new List<T>();
            foreach (var doc in snapshot.Documents)
            {
                TryAdd(result, doc);
            }

            return result;
        }

        public async Task<List<T>> GetByMultiuserIdAsync<T>(string multiuserId)
            where T : DtoFirestoreData, new()
        {
            var temp = new T();
            var snapshot = await Db.Collection(temp.CollectionName)
                .WhereEqualTo("MultiuserId", multiuserId)
                .GetSnapshotAsync();

            var result = new List<T>();
            foreach (var doc in snapshot.Documents)
            {
                TryAdd(result, doc);
            }

            return result;
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private static string EncryptData<T>(T data)
        {
            return JsonSerializer.Serialize(data);
        }

        private static T DecryptData<T>(string payload)
        {
            return JsonSerializer.Deserialize<T>(payload)
                   ?? throw new InvalidOperationException("Deserialization failed.");
        }

        private static void TryAdd<T>(ICollection<T> list, DocumentSnapshot doc)
            where T : DtoFirestoreData
        {
            try
            {
                if (doc.ContainsField("EncryptedData"))
                {
                    list.Add(DecryptData<T>(doc.GetValue<string>("EncryptedData")));
                }
                else
                {
                    list.Add(doc.ConvertTo<T>());
                }
            }
            catch { }
        }

        private static void Validate(DtoFirestoreData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(data.MultiuserId))
                throw new ArgumentException("MultiuserId is required.");

            if (string.IsNullOrWhiteSpace(data.CollectionName))
                throw new ArgumentException("CollectionName is required.");
        }
    }
}
