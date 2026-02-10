namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.AccessManagement;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Notification;
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BackendCommunicationService : IBackendCommunicationService
    {
        private readonly IAesHelper _aesHelper;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;
        private readonly IToastService _toastService;

        public BackendCommunicationService(IAesHelper aesHelper,
            IGeneralInformationManager generalInformationManager, ISharedMethodService sharedMethodService,
            IBackendDatabaseManager backendDatabaseManager, IBackendDatabaseHelper backendDatabaseHelper, IToastService toastService)
        {
            _aesHelper = aesHelper;
            _generalInformationManager = generalInformationManager;
            _sharedMethodService = sharedMethodService;
            _backendDatabaseManager = backendDatabaseManager;
            _backendDatabaseHelper = backendDatabaseHelper;
            _toastService = toastService;

        }


        public async Task<bool> ValidateMultiuserIdAsync(string multiuserId)
        {
            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);

            var collection = firestoreDb.Collection("StorageEntries");

            var querySnapshot = await collection.WhereEqualTo("MultiuserId",
                multiuserId).GetSnapshotAsync();

            if (querySnapshot.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Inserts a DTO into Firestore by appending a new document (auto-generated id).
        /// </summary>
        public async Task<bool> InsertAsync<T>(T data, bool isFrombackendSync = false)
        {
            dynamic dataDec = data;
            var type = data.GetType();
            var storageEntry = type.GetProperty("Category");
            var orderEntry = type.GetProperty("Title");

            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);

            var multiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;

            try
            {
                if (storageEntry != null)
                {
                    try
                    {
                        var document = new Dictionary<string, object>
                            {
                                { "MultiuserId", multiuserId ?? string.Empty },
                                { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty},
                                { "Category", dataDec.Category ?? string.Empty},
                                { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty},
                                { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty},
                                { "BarcodeType", dataDec.BarcodeType ?? string.Empty},
                                { "Location", dataDec.Location ?? string.Empty},
                                { "SearchInfo", dataDec.SearchInfo ?? string.Empty},
                                { "ItemName", dataDec.ItemName ?? string.Empty},
                                { "Description", dataDec.Description ?? string.Empty},
                                { "ImageUrls", dataDec.ImageList ?? new List<string>()},
                                { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty}
                            };

                        // Append new document (auto id) instead of using MultiuserId as document id
                        await firestoreDb.Collection("StorageEntries").AddAsync(document);
                        return true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.InsertAsync (DtoStorageEntryModel append) failed: {ex}");
                        if (!isFrombackendSync)
                        {
                            var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(dataDec, "false");
                            _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                        }
                        return false;
                    }

                }
                else if (orderEntry != null)
                {
                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", multiuserId ?? string.Empty },
                        { "OrderId", dataDec.OrderId ?? string.Empty},
                        { "Title", dataDec.Title ?? string.Empty},
                        { "Description", dataDec.Description ?? string.Empty},
                        { "CodeType", dataDec.CodeType ?? string.Empty},
                        { "PageType", dataDec.PageType ?? string.Empty},
                        { "ProductQuantity", _sharedMethodService.ConvertToString(dataDec.ProductQuantity) ?? string.Empty},
                        { "DateTime", _sharedMethodService.ConvertToString(dataDec.DateTime) ?? string.Empty},
                        { "TotalPrice", dataDec.TotalPrice ?? string.Empty},
                        { "Name", dataDec.Name ?? string.Empty},
                        { "Street", dataDec.Street ?? string.Empty},
                        { "HouseNo", dataDec.HouseNo ?? string.Empty},
                        { "ZipCode", dataDec.ZipCode ?? string.Empty},
                        { "City", dataDec.City ?? string.Empty},
                        { "Country", dataDec.Country ?? string.Empty},
                        { "Email", dataDec.Email ?? string.Empty},
                        { "ReferenceCode", dataDec.ReferenceCode ?? string.Empty},
                        { "ShipmentTracking", dataDec.ShipmentTracking ?? string.Empty},
                        { "StatusOfOrder", dataDec.StatusOfOrder ?? string.Empty},
                        { "PdfFiles", dataDec.PdfFiles ?? new List<byte[]>() }
                    };
                    try
                    {
                        // Append new document (auto id) instead of using MultiuserId as document id
                        await firestoreDb.Collection("Orders").AddAsync(document);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.InsertAsync (Orders append) failed: {ex}");

                        if (!isFrombackendSync)
                        {
                            var dto = _backendDatabaseHelper.CreateDtoOrdersBackendData(dataDec, "false");
                            _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                        }
                        return false;
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendCommunicationService.InsertAsync: validation failed: {ex}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync<T>(T data, bool isFrombackendSync = false)
        {
            dynamic dataDec = data;
            var type = data.GetType();
            var storageEntry = type.GetProperty("Category");
            var orderEntry = type.GetProperty("Title");
            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);

            var multiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
            try
            {
                if (storageEntry != null)
                {

                    try
                    {
                        var collection = firestoreDb.Collection("StorageEntries");

                        // Step 1: Get all documents matching MultiuserId
                        var querySnapshot = await collection
                            .WhereEqualTo("MultiuserId", multiuserId)
                            .GetSnapshotAsync();

                        if (querySnapshot.Count == 0)
                        {
                            Console.WriteLine($"No documents found with MultiuserId");
                            return false;
                        }

                        // Step 2: Find the document that matches 
                        DocumentSnapshot? targetDoc = null;
                        foreach (var doc in querySnapshot.Documents)
                        {

                            if (doc.ContainsField("CreatedDate") &&
                                   doc.ContainsField("BarcodeValue") &&
                                   doc.ContainsField("ItemName") &&
                                   doc.GetValue<string>("CreatedDate") == _sharedMethodService.ConvertToString(dataDec.CreatedDate) &&
                                   doc.GetValue<string>("BarcodeValue") == dataDec.BarcodeValue &&
                                   doc.GetValue<string>("ItemName") == dataDec.ItemName)
                            {
                                targetDoc = doc;
                                break;
                            }
                        }

                        if (targetDoc == null)
                        {
                            Console.WriteLine($"No document found with MultiuserId and CreatedDate={dataDec.CreatedDate}");
                            return false;
                        }

                        // Step 3: Update the document
                        var document = new Dictionary<string, object>
                            {
                               { "MultiuserId", multiuserId ?? string.Empty },
                               { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty},
                               { "Category", dataDec.Category ?? string.Empty},
                               { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty},
                               { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty},
                               { "BarcodeType", dataDec.BarcodeType ?? string.Empty},
                               { "Location", dataDec.Location ?? string.Empty},
                               { "SearchInfo", dataDec.SearchInfo ?? string.Empty},
                               { "ItemName", dataDec.ItemName ?? string.Empty},
                               { "Description", dataDec.Description ?? string.Empty},
                               { "ImageUrls", dataDec.ImageList ?? new List<string>()},
                               { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty}
                            };

                        await collection.Document(targetDoc.Id).SetAsync(document, SetOptions.Overwrite);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to update document: {ex}");
                        if (!isFrombackendSync)
                        {
                            var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(dataDec, "true");
                            await _backendDatabaseManager.UpdateAsync(dto);
                        }
                        return false;
                    }
                }
                else if (orderEntry != null)
                {

                    var document = new Dictionary<string, object>
                    {
                       { "MultiuserId", multiuserId ?? string.Empty },
                       { "OrderId", dataDec.OrderId ?? string.Empty},
                       { "Title", dataDec.Title ?? string.Empty},
                       { "Description", dataDec.Description ?? string.Empty},
                       { "CodeType", dataDec.CodeType ?? string.Empty},
                       { "PageType", dataDec.PageType ?? string.Empty},
                       { "ProductQuantity", _sharedMethodService.ConvertToString(dataDec.ProductQuantity) ?? string.Empty},
                       { "DateTime", _sharedMethodService.ConvertToString(dataDec.DateTime) ?? string.Empty},
                       { "TotalPrice", dataDec.TotalPrice ?? string.Empty},
                       { "Name", dataDec.Name ?? string.Empty},
                       { "Street", dataDec.Street ?? string.Empty},
                       { "HouseNo", dataDec.HouseNo ?? string.Empty},
                       { "ZipCode", dataDec.ZipCode ?? string.Empty},
                       { "City", dataDec.City ?? string.Empty},
                       { "Country", dataDec.Country ?? string.Empty},
                       { "Email", dataDec.Email ?? string.Empty},
                       { "ReferenceCode", dataDec.ReferenceCode ?? string.Empty},
                       { "ShipmentTracking", dataDec.ShipmentTracking ?? string.Empty},
                       { "StatusOfOrder", dataDec.StatusOfOrder ?? string.Empty},
                       { "PdfFiles", dataDec.PdfFiles ?? new List<byte[]>() }
                    };

                    try
                    {
                        Query query = firestoreDb.Collection("Orders").WhereEqualTo("OrderId", dataDec.OrderId)
                        .Limit(1); // OrderId should be unique

                        QuerySnapshot snapshot = await query.GetSnapshotAsync();

                        if (snapshot.Documents.Count == 0)
                        {
                            return false; // Not found
                        }

                        await firestoreDb.Collection("Orders").Document(snapshot.Documents[0].Id).
                            SetAsync(document, SetOptions.Overwrite);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.UpdateAsync (Orders) failed: {ex}");
                        if (!isFrombackendSync)
                        {
                            var dto = _backendDatabaseHelper.CreateDtoOrdersBackendData(dataDec, "true");
                            await _backendDatabaseManager.UpdateAsync(dto);
                        }

                        return false;
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendCommunicationService.UpdateAsync: validation failed: {ex}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<T>(T data, bool isFrombackendSync = false)
        {
            dynamic dataDec = data;
            var type = data.GetType();
            var storageEntry = type.GetProperty("Category");
            var multiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
            try
            {
                if (storageEntry != null)
                {
                    try
                    {
                        var collection = firestoreDb.Collection("StorageEntries");

                        // Step 1: Get all documents matching MultiuserId
                        var querySnapshot = await collection
                            .WhereEqualTo("MultiuserId", multiuserId)
                            .GetSnapshotAsync();

                        if (querySnapshot.Count == 0)
                        {
                            Console.WriteLine($"No documents found with MultiuserId");
                            return false;
                        }

                        // Step 2: Find the document that matches 
                        DocumentSnapshot? targetDoc = null;
                        foreach (var doc in querySnapshot.Documents)
                        {

                            if (doc.ContainsField("CreatedDate") &&
                                   doc.ContainsField("BarcodeValue") &&
                                   doc.ContainsField("ItemName") &&
                                   doc.GetValue<string>("CreatedDate") == _sharedMethodService.ConvertToString(dataDec.CreatedDate) &&
                                   doc.GetValue<string>("BarcodeValue") == dataDec.BarcodeValue &&
                                   doc.GetValue<string>("ItemName") == dataDec.ItemName)
                            {
                                targetDoc = doc;
                                break;
                            }
                        }

                        if (targetDoc == null)
                        {
                            Console.WriteLine($"No document found with MultiuserId and CreatedDate={dataDec.CreatedDate}");
                            return false;
                        }

                        await collection.Document(targetDoc.Id).DeleteAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to update document: {ex}");
                        if (!isFrombackendSync)
                        {
                            var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(dataDec, "delete");
                            _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackendCommunicationService.UpdateAsync: validation failed: {ex}");
                return false;
            }
        }

        public async Task<List<T>> GetAllAsync<T>() where T : FirestoreData, new()
        {
            var temp = new T();
            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
            var snapshot = await firestoreDb.Collection(temp.CollectionName).GetSnapshotAsync();

            var result = new List<T>();
            foreach (var doc in snapshot.Documents)
            {
                TryAdd(result, doc);
            }

            return result;
        }

        public async Task<T?> GetAsync<T>(string id) where T : FirestoreData, new()
        {
            var temp = new T();
            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
            var docRef = firestoreDb.Collection(temp.CollectionName).Document(id);
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

        public async Task<List<T>> GetByMultiuserIdAsync<T>(string multiuserId) where T : FirestoreData, new()
        {
            if (string.IsNullOrWhiteSpace(multiuserId)) throw new ArgumentException(nameof(multiuserId));

            var firestoreDb = await FirestoreDbFactory.CreateAsync(FirebaseConfig.PROJECT_ID);
            var temp = new T();
            var snapshot = await firestoreDb.Collection(temp.CollectionName)
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
