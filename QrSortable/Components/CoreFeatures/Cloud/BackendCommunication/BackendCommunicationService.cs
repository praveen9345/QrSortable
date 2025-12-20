namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using QrSortable.Components.PlatformUtils;
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BackendCommunicationService : IBackendCommunicationService
    {
        private readonly IAesHelper _aesHelper;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private FirestoreDb Db { get; set; }

        private string _multiUserId = string.Empty;

        public BackendCommunicationService(IAesHelper aesHelper, IFirebaseStorageService firebaseStorageService,
            IGeneralInformationManager generalInformationManager, ISharedMethodService sharedMethodService,
            IBackendDatabaseManager backendDatabaseManager)
        {
            Db = FirestoreDb.Create(Configuration.FirebaseConfig.PROJECT_ID);
            _aesHelper = aesHelper;
            _firebaseStorageService = firebaseStorageService;
            _generalInformationManager = generalInformationManager;
            _sharedMethodService = sharedMethodService;
            _backendDatabaseManager = backendDatabaseManager;

            _multiUserId = _generalInformationManager.GetGeneralInformationAsync()
                .GetAwaiter().GetResult().MultiUserId;
            
        }

        /// <summary>
        /// Inserts a DTO into Firestore by appending a new document (auto-generated id).
        /// </summary>
        public async Task<bool> InsertAsync<T>(T data, bool isFrombackendSync = false) 
        {
            try
            {
                dynamic dataDec = data;
                var type = data.GetType();
                var storageEntry = type.GetProperty("Category");
                var orderEntry = type.GetProperty("Title");

                if (storageEntry != null) 
                {
                    var imageUrls = await _firebaseStorageService.UploadImagesAsync(dataDec.ImageList);
                    
                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", _multiUserId ?? string.Empty },
                        { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty},
                        { "Category", dataDec.Category ?? string.Empty},
                        { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty},
                        { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty},
                        { "BarcodeType", dataDec.BarcodeType ?? string.Empty},
                        { "Location", dataDec.Location ?? string.Empty},
                        { "SearchInfo", dataDec.SearchInfo ?? string.Empty},
                        { "ItemName", dataDec.ItemName ?? string.Empty},
                        { "Description", dataDec.Description ?? string.Empty},
                        { "ImageUrls", imageUrls ?? string.Empty},
                        { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty}
                    };
                    try
                    {
                        // Append new document (auto id) instead of using MultiuserId as document id
                        await Db.Collection("StorageEntries").AddAsync(document);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.InsertAsync (Orders append) failed: {ex}");
                        try
                        {
                            if (!isFrombackendSync)
                            {
                                var backendData = new DtoStorageEntryModel
                                {
                                    IsUpdateData = false,
                                    StorageId = _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty,
                                    Category = dataDec.Category ?? string.Empty,
                                    CreatedDate = _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty,
                                    BarcodeValue = dataDec.BarcodeValue ?? string.Empty,
                                    BarcodeType = dataDec.BarcodeType ?? string.Empty,
                                    Location = dataDec.Location ?? string.Empty,
                                    SearchInfo = dataDec.SearchInfo ?? string.Empty,
                                    ItemName = dataDec.ItemName ?? string.Empty,
                                    Description = dataDec.Description ?? string.Empty,
                                    ImageList = dataDec.ImageList ?? string.Empty,
                                    BackgroundColorHex = dataDec.BackgroundColorHex ?? string.Empty
                                };

                                SaveToTheBackendAsync(backendData);
                            }
                        
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"BackendCommunicationService.InsertAsync: DtoStorageEntryModel backend failed: {e}");
                            // as a last resort serialize to local log or drop
                        }
                        return false;
                    }

                }
                else if(orderEntry != null) 
                {
                    var document = new Dictionary<string, object>
                    {
                        { "MultiuserId", _multiUserId ?? string.Empty },
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
                        await Db.Collection("Orders").AddAsync(document);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.InsertAsync (Orders append) failed: {ex}");

                        try 
                        {
                            if (!isFrombackendSync)
                            {
                                var backendData = new DtoOrdersModel
                                {
                                    IsUpdateData = false,
                                    OrderId = dataDec.OrderId ?? string.Empty,
                                    Title = dataDec.Title ?? string.Empty,
                                    Description = dataDec.Description ?? string.Empty,
                                    CodeType = dataDec.CodeType ?? string.Empty,
                                    PageType = dataDec.PageType ?? string.Empty,
                                    ProductQuantity = _sharedMethodService.ConvertToString(dataDec.ProductQuantity) ?? string.Empty,
                                    DateTime = _sharedMethodService.ConvertToString(dataDec.DateTime) ?? string.Empty,
                                    TotalPrice = dataDec.TotalPrice ?? string.Empty,
                                    Name = dataDec.Name ?? string.Empty,
                                    Street = dataDec.Street ?? string.Empty,
                                    HouseNo = dataDec.HouseNo ?? string.Empty,
                                    ZipCode = dataDec.ZipCode ?? string.Empty,
                                    City = dataDec.City ?? string.Empty,
                                    Country = dataDec.Country ?? string.Empty,
                                    Email = dataDec.Email ?? string.Empty,
                                    ReferenceCode = dataDec.ReferenceCode ?? string.Empty,
                                    ShipmentTracking = dataDec.ShipmentTracking ?? string.Empty,
                                    StatusOfOrder = dataDec.StatusOfOrder ?? string.Empty,
                                    PdfFiles = dataDec.PdfFiles ?? new List<byte[]>()
                                };

                                SaveToTheBackendAsync(backendData);
                            }
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"BackendCommunicationService.InsertAsync: DtoOrdersModel backend failed: {e}");
                            // as a last resort serialize to local log or drop
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

        private async void SaveToTheBackendAsync(DatabaseEntry backendData)
        {
            _backendDatabaseManager.BeginTransaction();
            var addedItem = await _backendDatabaseManager.AddAsync(backendData);
            if (addedItem != null)
            {
                _backendDatabaseManager.CommitTransaction();
                Console.WriteLine("BackendCommunicationService.InsertAsync: Successfully add to backend.");
                return;
            }
            else
            {
                _backendDatabaseManager.Rollback();
                Console.WriteLine("BackendCommunicationService.InsertAsync: AddAsync returned null - rollback performed.");
                return;
            }
        } 

        public async Task<bool> UpdateAsync<T>(T data, bool isFrombackendSync = false)
        {

            try
            {
                dynamic dataDec = data;
                var type = data.GetType();
                var storageEntry = type.GetProperty("Category");
                var orderEntry = type.GetProperty("Title");

                if (storageEntry != null)
                {
                    var collection = Db.Collection("StorageEntries");
                    
                    // Step 1: Get all documents matching MultiuserId
                    var querySnapshot = await collection
                        .WhereEqualTo("MultiuserId", _multiUserId)
                        .GetSnapshotAsync();

                    if (querySnapshot.Count == 0)
                    {
                        Console.WriteLine($"No documents found with MultiuserId={_multiUserId}");
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

                            //Delete old images from Firebase Storage
                            var imageUrlsFb = doc.GetValue<List<string>>("ImageUrls")?
                                               .Where(u => !string.IsNullOrWhiteSpace(u))
                                               .ToList();

                            if (imageUrlsFb != null && imageUrlsFb.Count > 0)
                            {
                                await _firebaseStorageService.DeleteImagesAsync(imageUrlsFb);
                            }

                            break;
                        }
                    }

                    if (targetDoc == null)
                    {
                        Console.WriteLine($"No document found with MultiuserId={_multiUserId} and CreatedDate={dataDec.CreatedDate}");
                        return false;
                    }

                    // Step 3: Update the document
                    var imageUrls = await _firebaseStorageService.UploadImagesAsync(dataDec.ImageList);

                    var document = new Dictionary<string, object>
                    {
                       { "MultiuserId", _multiUserId ?? string.Empty },
                       { "StorageId", _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty},
                       { "Category", dataDec.Category ?? string.Empty},
                       { "CreatedDate", _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty},
                       { "BarcodeValue", dataDec.BarcodeValue ?? string.Empty},
                       { "BarcodeType", dataDec.BarcodeType ?? string.Empty},
                       { "Location", dataDec.Location ?? string.Empty},
                       { "SearchInfo", dataDec.SearchInfo ?? string.Empty},
                       { "ItemName", dataDec.ItemName ?? string.Empty},
                       { "Description", dataDec.Description ?? string.Empty},
                       { "ImageUrls", imageUrls ?? string.Empty},
                       { "BackgroundColorHex", dataDec.BackgroundColorHex ?? string.Empty}
                    };

                    try
                    {
                        await collection.Document(targetDoc.Id).SetAsync(document, SetOptions.Overwrite);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to update document {targetDoc.Id}: {ex}");    
                        try
                        {
                            if (!isFrombackendSync)
                            {
                                var backendData = new DtoStorageEntryModel
                                {
                                    IsUpdateData = true,
                                    StorageId = _sharedMethodService.ConvertToString(dataDec.StorageId),
                                    Category = dataDec.Category,
                                    CreatedDate = _sharedMethodService.ConvertToString(dataDec.CreatedDate),
                                    BarcodeValue = dataDec.BarcodeValue,
                                    BarcodeType = dataDec.BarcodeType,
                                    Location = dataDec.Location,
                                    SearchInfo = dataDec.SearchInfo,
                                    ItemName = dataDec.ItemName,
                                    Description = dataDec.Description,
                                    ImageList = dataDec.ImageList,
                                    BackgroundColorHex = dataDec.BackgroundColorHex
                                };

                                await _backendDatabaseManager.UpdateAsync(backendData);
                            }
                        }
                        catch (Exception e)
                        {
                                Console.WriteLine($"BackendCommunicationService.UpdateAsync: DtoStorageEntryModel backend failed: {e}");
                                // as a last resort serialize to local log or drop
                        }
                        return false;
                    }
                }
                else if (orderEntry != null)
                {

                    var document = new Dictionary<string, object>
                    {
                       { "MultiuserId", _multiUserId ?? string.Empty },
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
                        await Db.Collection("Orders")
                                .Document(_multiUserId)
                                .SetAsync(document, SetOptions.Overwrite);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BackendCommunicationService.UpdateAsync (Orders) failed: {ex}");
                        try
                        {
                            if (!isFrombackendSync)
                            {
                                var backendData = new DtoOrdersModel
                                {
                                    IsUpdateData = true,
                                    OrderId = dataDec.OrderId,
                                    Title = dataDec.Title,
                                    Description = dataDec.Description,
                                    CodeType = dataDec.CodeType,
                                    PageType = dataDec.PageType,
                                    ProductQuantity = _sharedMethodService.ConvertToString(dataDec.ProductQuantity),
                                    DateTime = _sharedMethodService.ConvertToString(dataDec.DateTime),
                                    TotalPrice = dataDec.TotalPrice,
                                    Name = dataDec.Name,
                                    Street = dataDec.Street,
                                    HouseNo = dataDec.HouseNo,
                                    ZipCode = dataDec.ZipCode,
                                    City = dataDec.City,
                                    Country = dataDec.Country,
                                    Email = dataDec.Email,
                                    ReferenceCode = dataDec.ReferenceCode,
                                    ShipmentTracking = dataDec.ShipmentTracking,
                                    StatusOfOrder = dataDec.StatusOfOrder,
                                    PdfFiles = dataDec.PdfFiles
                                };
                               await _backendDatabaseManager.UpdateAsync(backendData);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"BackendCommunicationService.UpdateAsync: DtoOrdersModel backend failed: {e}");
                            // as a last resort serialize to local log or drop
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