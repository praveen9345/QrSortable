namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System;

    /// <summary>
    ///     The view model of the  BankTransferViewModel screen.
    /// </summary>
    public partial class BankTransferViewModel : BaseViewModel<Product>
    {
        public readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IConnectivityService _connectivityService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;

        private Product _product;

        /// <summary>
        ///     Initializes a new instance of the <see cref=" BankTransferViewModel" />.
        /// </summary>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public BankTransferViewModel(IToastService toastService, IDatabaseManager databaseManager, 
            ISharedMethodService sharedMethodService, IBackendCommunicationService backendCommunicationService,
            IConnectivityService connectivityService, IBackendDatabaseManager backendDatabaseManager)
        {
            IsBackNavigationEnabled = true;
            _toastService = toastService;
            _databaseManager = databaseManager;
            _sharedMethodService = sharedMethodService;
            _backendCommunicationService = backendCommunicationService;
            _connectivityService = connectivityService;
            _backendDatabaseManager = backendDatabaseManager;

            ReferenceCode = GenerateReferenceCode();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;
            var price = _product.TotalPrice;
            TotalAmount = price.ToString("F2") + "€";
            var discounted = (decimal)0.1 * price;
            DiscountAmount = "-" + discounted.ToString("F2") + "€";
            var netPrice = price - discounted;
            NetTotalAmount = netPrice.ToString("F2") + "€";
        }

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>
        /// Represents the currently discount amount in the application.
        /// </summary>
        [ObservableProperty]
        private string _discountAmount;

        /// <summary>
        /// Represents the currently net total amount in the application.
        /// </summary>
        [ObservableProperty]
        private string _netTotalAmount;

        /// <summary>
        /// Represents the currently iban code in the application.
        /// </summary>
        [ObservableProperty]
        private string _ibanCode = "DE89370400440532013000";

        /// <summary>
        /// Represents the currently reference code in the application.
        /// </summary>
        [ObservableProperty]
        private string _referenceCode;


        public AsyncRelayCommand CopyIbanCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(IbanCode);
            await _toastService.DisplayToast("IBAN copied to clipboard!");

        });

        public AsyncRelayCommand CopyReferenceCodeCommand => new AsyncRelayCommand(async () =>
        {

            await Clipboard.Default.SetTextAsync(ReferenceCode);
            await _toastService.DisplayToast("Reference code copied to clipboard!");

        });

        public AsyncRelayCommand PlaceAnOrderCommand => new AsyncRelayCommand(async () =>
        {
            if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
            {
                await DialogService.ShowAlertDialog("No Internet Connection",
                    "To place an order, an internet connection is required. Please check your connection and try again.", "OK");
                return;
            }

            var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Processing...",
            async () =>{return await DatabaseAndBackendStoringAsync(); });

            if (result)
            {
                var dbItems = await _databaseManager.GetListAsync<AddToBasketData>();
                var match = dbItems.FirstOrDefault(x =>
                    x.Title == _product.Title &&
                    x.OrderId == _product.OrderId
                );

                if (match != null)
                {
                    await _databaseManager.DeleteAsync(match);
                }

                await DialogService.ShowAlertDialog(
                    "Confirmation 🎉",
                    "Thank you for your order. We will send you an email shortly.",
                    "OK");
                await NavigationService.Navigate<RootView>();
            }
            else
            {
                await DialogService.ShowAlertDialog("An unexpected error occurred while saving the item. Please try again.", AppResources.Dialog_OK_Text);
            }
        });

        private async Task<bool> DatabaseAndBackendStoringAsync() 
        {
            /*TODO: *send email to user and to me and 
            *send saved to the backgend when user placed an order
            */
            try
            {
                var orderedItem = new YoursOrderData
                {
                    OrderId = _product.OrderId,
                    Title = _product.Title,
                    Description = _product.Description,
                    CodeType = GetCodeAndPageType(_product.CodeType),
                    PageType = GetCodeAndPageType(_product.PageType),
                    ProductQuantity = _product.NumberOfPages,
                    DateTime = DateTime.Now,
                    TotalPrice = _sharedMethodService.ParsePrice(NetTotalAmount).ToString(),
                    Name = _product.Name,
                    Street = _product.Street,
                    HouseNo = _product.HouseNo,
                    ZipCode = _product.ZipCode,
                    City = _product.City,
                    Country = _product.Country,
                    Email = _product.Email,
                    ReferenceCode = ReferenceCode,
                    ShipmentTracking = "DHL:",
                    StatusOfOrder = "Pending...",
                    PdfFiles = new List<byte[]>()
                };

                _databaseManager.BeginTransaction();
                var addedItem = await _databaseManager.AddAsync(orderedItem);

                if (addedItem != null)
                {
                    _databaseManager.CommitTransaction();
                   
                    if(!await _connectivityService.CheckInternetConnectionAvailableAsync())
                    {
                        var dto = new DtoOrdersModel
                        {
                            IsUpdateData = "false",
                            OrderId = orderedItem.OrderId ?? string.Empty,
                            Title = orderedItem.Title ?? string.Empty,
                            Description = orderedItem.Description ?? string.Empty,
                            CodeType = orderedItem.CodeType ?? string.Empty,
                            PageType = orderedItem.PageType ?? string.Empty,
                            ProductQuantity = _sharedMethodService.ConvertToString(orderedItem.ProductQuantity) ?? string.Empty,
                            DateTime = _sharedMethodService.ConvertToString(orderedItem.DateTime) ?? string.Empty,
                            TotalPrice = orderedItem.TotalPrice ?? string.Empty,
                            Name = orderedItem.Name ?? string.Empty,
                            Street = orderedItem.Street ?? string.Empty,
                            HouseNo = orderedItem.HouseNo ?? string.Empty,
                            ZipCode = orderedItem.ZipCode ?? string.Empty,
                            City = orderedItem.City ?? string.Empty,
                            Country = orderedItem.Country ?? string.Empty,
                            Email = orderedItem.Email ?? string.Empty,
                            ReferenceCode = orderedItem.ReferenceCode ?? string.Empty,
                            ShipmentTracking = orderedItem.ShipmentTracking ?? string.Empty,
                            StatusOfOrder = orderedItem.StatusOfOrder ?? string.Empty,
                            PdfFiles = orderedItem.PdfFiles ?? new List<byte[]>()
                        };

                        await _backendDatabaseManager.AddAsync(dto);
                    }
                    else
                    {
                        await _backendCommunicationService.InsertAsync(orderedItem);
                    }
                       
                    Console.WriteLine("Successfully placed an ordered.");
                    return true;
                }
                else
                {
                    _databaseManager.Rollback();
                    Console.WriteLine("DatabaseAndBackendStoringAsync: AddAsync returned null - rollback performed.");
                    return false;
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"DatabaseAndBackendStoringAsync::Exception during add: {ex}");
                return false;
            }
        }

        private string GetCodeAndPageType(string data)
        {
            if (_product.Title == "Generate A4 QR or bar code yourself!")
            {
                return data;
            }
            return string.Empty;

        }
        private string GenerateReferenceCode()
        {
            string prefix = "#QS";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return prefix + randomPart; 
        }
    }

}
