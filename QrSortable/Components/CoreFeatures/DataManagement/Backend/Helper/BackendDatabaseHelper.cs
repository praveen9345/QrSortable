namespace QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper
{
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using QrSortable.Components.PlatformUtils;

    public class BackendDatabaseHelper : IBackendDatabaseHelper
    {
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly ISharedMethodService _sharedMethodService;

        public BackendDatabaseHelper(IBackendDatabaseManager backendDatabaseManager, ISharedMethodService sharedMethodService)
        {
            _backendDatabaseManager = backendDatabaseManager;
            _sharedMethodService = sharedMethodService;
        }


        public async void SaveToTheBackendAsync(DatabaseEntry backendData)
        {
            _backendDatabaseManager.BeginTransaction();
            var addedItem = await _backendDatabaseManager.AddAsync(backendData);
            if (addedItem != null)
            {
                _backendDatabaseManager.CommitTransaction();
                return;
            }
            else
            {
                _backendDatabaseManager.Rollback();
                return;
            }
        }

        public DtoStorageEntryModel CreateDtoStorageEntryBackendData(dynamic dataDec, string isUpdateData)
        {
            return new DtoStorageEntryModel
            {
                IsUpdateData = isUpdateData,
                StorageId = _sharedMethodService.ConvertToString(dataDec.StorageId) ?? string.Empty,
                Category = dataDec.Category ?? string.Empty,
                CreatedDate = _sharedMethodService.ConvertToString(dataDec.CreatedDate) ?? string.Empty,
                BarcodeValue = dataDec.BarcodeValue ?? string.Empty,
                BarcodeType = dataDec.BarcodeType ?? string.Empty,
                Location = dataDec.Location ?? string.Empty,
                SearchInfo = dataDec.SearchInfo ?? string.Empty,
                ItemName = dataDec.ItemName ?? string.Empty,
                Description = dataDec.Description ?? string.Empty,
                ImageList = dataDec.ImageList ?? new List<byte[]>(),
                BackgroundColorHex = dataDec.BackgroundColorHex ?? string.Empty
            };
        }

        public DtoOrdersModel CreateDtoOrdersBackendData(dynamic dataDec, string isUpdateData)
        {
            return new DtoOrdersModel
            {
                IsUpdateData = isUpdateData,
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
        }

        public DtoSubscriptionEntityModel CreatDtoSubscriptionBackendData(dynamic dataDec, string isUpdateData)
        {
            return new DtoSubscriptionEntityModel
            {
                IsUpdateData = isUpdateData,
                IsSubscribed = dataDec.IsSubscribed,
                CreatedAt = _sharedMethodService.ConvertToString(dataDec.CreatedAt) ?? string.Empty,
                CustomerId = dataDec.CustomerId ?? string.Empty,
                SubscriptionId = dataDec.SubscriptionId ?? string.Empty,
                Email = dataDec.Email ?? string.Empty
            };
        }

    }
}
