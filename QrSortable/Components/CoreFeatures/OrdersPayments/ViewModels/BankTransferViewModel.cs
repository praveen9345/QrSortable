namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System;

    public partial class BankTransferViewModel : BaseViewModel<Product>
    {
        public readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IConnectivityService _connectivityService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;

        private Product _product;
        private const decimal DiscountRate = 0.1m;

        // ── Currency rates (relative to EUR) ─────────────────────────────────────
        private static readonly Dictionary<string, (decimal Rate, string Symbol)> CurrencyRates = new()
        {
            { "Euro(€)", (1.00m, "€") },
            { "USD($)",  (1.08m, "$") },
            { "GBP(£)",  (0.86m, "£") }
        };

        public BankTransferViewModel(
            IToastService toastService,
            IDatabaseManager databaseManager,
            ISharedMethodService sharedMethodService,
            IBackendCommunicationService backendCommunicationService,
            IConnectivityService connectivityService,
            IBackendDatabaseManager backendDatabaseManager,
            IBackendDatabaseHelper backendDatabaseHelper)
        {
            IsBackNavigationEnabled = true;
            _toastService = toastService;
            _databaseManager = databaseManager;
            _sharedMethodService = sharedMethodService;
            _backendCommunicationService = backendCommunicationService;
            _connectivityService = connectivityService;
            _backendDatabaseManager = backendDatabaseManager;
            _backendDatabaseHelper = backendDatabaseHelper;

            ReferenceCode = GenerateReferenceCode();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;

            if (!CurrencyRates.TryGetValue(_product.CurrencySymbol, out var currency))
                currency = (1.00m, "€");

            var symbol = currency.Symbol;
            var rate = currency.Rate;

            // Convert prices using the currency rate
            var price = _product.TotalPrice * rate;
            var shippingFee = _product.ShippingCost * rate;

            // Subtotal = base price in target currency
            SubtotalAmount = symbol + price.ToString("F0");

            // Discount = 10% on subtotal only
            var discounted = DiscountRate * price;
            DiscountAmount = "-" + symbol + discounted.ToString("F0");

            // Net = subtotal - discount
            var netPrice = price - discounted;
            NetTotalAmount = symbol + netPrice.ToString("F0");

            // Shipping (converted, no discount applied)
            ShippingCost = symbol + shippingFee.ToString("F0");

            // Total = net + shipping
            var total = netPrice + shippingFee;
            TotalAmount = symbol + total.ToString("F0");
        }

        /// <summary>Subtotal before discount.</summary>
        [ObservableProperty]
        private string _subtotalAmount;

        /// <summary>Discount amount (10% of subtotal).</summary>
        [ObservableProperty]
        private string _discountAmount;

        /// <summary>Net total after discount, before shipping.</summary>
        [ObservableProperty]
        private string _netTotalAmount;

        /// <summary>Shipping cost.</summary>
        [ObservableProperty]
        private string _shippingCost;

        /// <summary>Final amount to pay (net + shipping).</summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>IBAN code.</summary>
        [ObservableProperty]
        private string _ibanCode = "DE70100123455220300911";

        /// <summary>IBAN code.</summary>
        [ObservableProperty]
        private string _bicCode  = "TRBKDEBBXXX";

        /// <summary>Reference code.</summary>
        [ObservableProperty]
        private string _referenceCode;

        public AsyncRelayCommand CopyIbanCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(IbanCode);
            await _toastService.DisplayToast(AppResources.BankTransferViewModel_IBANCopiedText);
        });

        public AsyncRelayCommand CopyBicCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(BicCode);
            await _toastService.DisplayToast(AppResources.BankTransferViewModel_BICCopiedText);
        });

        public AsyncRelayCommand CopyReferenceCodeCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(ReferenceCode);
            await _toastService.DisplayToast(AppResources.BankTransferViewModel_ReferenceCopiedText);
        });

        public AsyncRelayCommand PlaceAnOrderCommand => new AsyncRelayCommand(async () =>
        {
            if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
            {
                await DialogService.ShowAlertDialog(
                    AppResources.Dialog_InternetConnection_Title,
                    AppResources.Dialog_InternetConnection_Message,
                    AppResources.Dialog_OK_Text);
                return;
            }

            var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult(
                AppResources.Dialog_Processing,
                async () => { return await DatabaseAndBackendStoringAsync(); });

            if (result)
            {
                var dbItems = await _databaseManager.GetListAsync<AddToBasketData>();
                var match = dbItems.FirstOrDefault(x =>
                    x.Title == _product.Title &&
                    x.OrderId == _product.OrderId);

                if (match != null)
                    await _databaseManager.DeleteAsync(match);

                await DialogService.ShowAlertDialog(
                    AppResources.Dialog_Conformation,
                    AppResources.BankTransferViewModel_EmailSendMssg,
                    AppResources.Dialog_OK_Text);

                await NavigationService.Navigate<RootView>();
            }
            else
            {
                await DialogService.ShowAlertDialog(
                    AppResources.BankTransferViewModel_SaveErrorMsg,
                    AppResources.Dialog_OK_Text);
            }
        });

        private async Task<bool> DatabaseAndBackendStoringAsync()
        {
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

                    if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                    {
                        var dto = _backendDatabaseHelper.CreateDtoOrdersBackendData(orderedItem, "false");
                        _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                    }
                    else
                    {
                        await _backendCommunicationService.InsertAsync(orderedItem);
                    }

                    Console.WriteLine("Successfully placed an order.");
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
                Console.WriteLine($"DatabaseAndBackendStoringAsync::Exception: {ex}");
                return false;
            }
        }

        private string GetCodeAndPageType(string data)
        {
            if (_product.Title == AppResources.SelectProductViewModel_GenrateOfA4QRcodeTitle)
                return data;

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