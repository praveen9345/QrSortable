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

        public BackendCommunicationService(IAesHelper aesHelper, IFirebaseStorageService firebaseStorageService)
        {
            Db = FirestoreDb.Create(Configuration.FirebaseConfig.PROJECT_ID);
            _aesHelper = aesHelper;
            _firebaseStorageService = firebaseStorageService;
        }

        /// <summary>
        /// Inserts a DTO into Firestore by appending a new document (auto-generated id).
        /// The DTO.MultiuserId is written as a document field so it remains available for queries.
        /// </summary>
        public async Task InsertAsync<T>(T data) where T : FirestoreData
        {
            Validate(data);

            // Special-case: append OrdersModel as plain fields (no encryption), keep MultiuserId as a field
            if (data is OrdersModel ordersDto)
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
                    // Append new document (auto id) instead of using MultiuserId as document id
                    await Db.Collection(ordersDto.CollectionName).AddAsync(document);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BackendCommunicationService.InsertAsync (Orders append) failed: {ex}");
                    // fallthrough to enqueue logic below
                }
            }

            // Special-case: append DtoStorageEntryModel as plain fields (or adjust if you want encryption)
            if (data is StorageEntryModel storageDto)
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
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BackendCommunicationService.InsertAsync (Storage append) failed: {ex}");
                    // fallthrough to enqueue logic below
                }
            }

            // Default behaviour: encrypt entire DTO JSON (value) and append as a new document,
            // writing MultiuserId as a field to keep that identifier searchable.
            var encryptedJson = EncryptData(data);

            var doc = new Dictionary<string, object>
            {
                { "MultiuserId", data.MultiuserId ?? string.Empty },
                { "EncryptedData", encryptedJson }
            };

            try
            {
                // Use AddAsync to append (auto-generated id) rather than CreateAsync with a specific id
                await Db.Collection(data.CollectionName).AddAsync(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendCommunicationService.InsertAsync: failed to write to Firestore: {ex}");
                // Automatic enqueue for later retry (persistent)
                try
                {
                    var backendSync = ServiceHelper.GetService<IBackendSynchronizationManager>();
                    if (backendSync != null)
                    {
                        //TODO1
                        //await backendSync.EnqueueAsync(data);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"BackendCommunicationService.InsertAsync: enqueue failed: {e}");
                    // as a last resort serialize to local log or drop
                }
            }
        }

        public async Task<T?> GetAsync<T>(string id) where T : FirestoreData, new()
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
                else
                {
                    return snapshot.ConvertTo<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAsync parse failed ({id}): {ex}");
                return null;
            }
        }

        public async Task UpdateAsync<T>(T data) where T : FirestoreData
        {
            Validate(data);

            // Special-case: OrdersModel
            if (data is OrdersModel ordersDto)
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
                    await Db.Collection(ordersDto.CollectionName)
                            .Document(ordersDto.MultiuserId)
                            .SetAsync(document, SetOptions.Overwrite);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BackendCommunicationService.UpdateAsync (Orders) failed: {ex}");
                }
            }

            if (data is StorageEntryModel storageDto)
            {
                var collection = Db.Collection(storageDto.CollectionName);

                // Step 1: Get all documents matching MultiuserId
                var querySnapshot = await collection
                    .WhereEqualTo("MultiuserId", storageDto.MultiuserId)
                    .GetSnapshotAsync();

                if (querySnapshot.Count == 0)
                {
                    Console.WriteLine($"No documents found with MultiuserId={storageDto.MultiuserId}");
                    return;
                }

                // Step 2: Find the document that matches CreatedDate
                DocumentSnapshot? targetDoc = null;
                foreach (var doc in querySnapshot.Documents)
                {

                    if (doc.ContainsField("CreatedDate") &&
                           doc.ContainsField("BarcodeValue") &&
                           doc.ContainsField("ItemName") &&
                           doc.GetValue<string>("CreatedDate") == storageDto.CreatedDate &&
                           doc.GetValue<string>("BarcodeValue") == storageDto.BarcodeValue &&
                           doc.GetValue<string>("ItemName") == storageDto.ItemName)
                    {
                        targetDoc = doc;

                        //Delete old images from Firebase Storage
                        var imageUrls = doc.GetValue<List<string>>("ImageUrls")?
                                           .Where(u => !string.IsNullOrWhiteSpace(u))
                                           .ToList();

                        if (imageUrls != null && imageUrls.Count > 0)
                        {
                            await _firebaseStorageService.DeleteImagesAsync(imageUrls);
                        }

                        break;
                    }
                }

                if (targetDoc == null)
                {
                    Console.WriteLine($"No document found with MultiuserId={storageDto.MultiuserId} and CreatedDate={storageDto.CreatedDate}");
                    return;
                }

                // Step 3: Update the document
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
                { "ImageUrls", storageDto.ImageUrls ?? new List<string>() },
                { "BackgroundColorHex", storageDto.BackgroundColorHex }
            };

                try
                {
                    await collection
                        .Document(targetDoc.Id)
                        .SetAsync(document, SetOptions.Overwrite);

                    Console.WriteLine($"Document {targetDoc.Id} updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update document {targetDoc.Id}: {ex}");
                }

                return;
            }

            // Fallback for other DTOs (encrypted)
            var encryptedJsonUpdate = EncryptData(data);
            var docUpdate = new Dictionary<string, object>
            {
                { "MultiuserId", data.MultiuserId ?? string.Empty },
                { "EncryptedData", encryptedJsonUpdate }
            };

            try
            {
                await Db.Collection(data.CollectionName)
                        .Document(data.MultiuserId)
                        .SetAsync(docUpdate, SetOptions.Overwrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateAsync: failed to update Firestore: {ex}");
            }
        }

        public async Task DeleteAsync<T>(string id, string collectionName)
        {
            // TODO: delete the storage for this multiuser id as well
            await Db.Collection(collectionName)
                    .Document(id)
                    .DeleteAsync();
        }

        public async Task<List<T>> GetAllAsync<T>() where T : FirestoreData, new()
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

        public async Task<List<T>> GetByMultiuserIdAsync<T>(string multiuserId) where T : FirestoreData, new()
        {
            if (string.IsNullOrWhiteSpace(multiuserId)) throw new ArgumentException(nameof(multiuserId));

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


        #region Helpers

        protected static string EncryptData<T>(T data, Func<string, string>? encryptor = null)
        {
            var json = JsonSerializer.Serialize(data);
            return encryptor != null ? encryptor(json) : json;
        }

        protected static T DecryptData<T>(string payload, Func<string, string>? decryptor = null)
        {
            var json = decryptor != null ? decryptor(payload) : payload;
            return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Failed to deserialize payload.");
        }



        private static void TryAdd<T>(ICollection<T> list, DocumentSnapshot doc) where T : FirestoreData
        {
            try
            {
                if (doc.ContainsField("EncryptedData"))
                {
                    var payload = doc.GetValue<string>("EncryptedData");
                    list.Add(DecryptData<T>(payload));
                }
                else
                {
                    list.Add(doc.ConvertTo<T>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Document parse failed ({doc.Id}): {ex}");
            }
        }

        private static void Validate(FirestoreData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(data.MultiuserId))
                throw new ArgumentException("Document Id is required.");

            if (string.IsNullOrWhiteSpace(data.CollectionName))
                throw new ArgumentException("CollectionName is required.");
        }

       
        #endregion
    }
}