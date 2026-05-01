namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.Maui.Storage;
    using Mollie.Api.Models.Payment.Response;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.TimeHandling;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the Payment Shipment view screen.
    /// </summary>
    public partial class PaymentShipmentViewModel : BaseViewModel<Product>
    {
        private readonly IMollieService _mollieService;
        private readonly ITimerService _timeService;
        public readonly IDatabaseManager _databaseManager;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly ICodeGeneratorService _codeService;
        private readonly IPdfGeneratorService _pdfService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IConnectivityService _connectivityService;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        private Product _product;
        private string _paymentId;
        private Timer _timer;
        private bool _orderProcessed = false;
        private readonly object _lock = new object();
        private static readonly string CODE_GENERATED_NAME =
            AppResources.SelectProductViewModel_GenrateOfA4QRcodeTitle.ToString();

        // ── Currency rates (relative to EUR) ─────────────────────────────────────
        private static readonly Dictionary<string, (decimal Rate, string Symbol)> CurrencyRates = new()
        {
            { "Euro(€)", (1.00m, "€") },
            { "USD($)",  (1.08m, "$") },
            { "GBP(£)",  (0.86m, "£") }
        };

        public ObservableCollection<string> CurrencyItem { get; } =
        new ObservableCollection<string>
        {
            "Euro(€)",
            "USD($)",
            "GBP(£)"
        };

        public List<string> CountryList { get; } = new()
        {
            "Germany", "United Kingdom", "Austria", "Belgium", "Bulgaria",
            "Croatia", "Cyprus", "Czech Republic", "Denmark", "Estonia",
            "Finland", "France", "Greece", "Hungary", "Ireland", "Italy",
            "Latvia", "Lithuania", "Luxembourg", "Malta", "Netherlands",
            "Poland", "Portugal", "Romania", "Slovakia", "Slovenia",
            "Spain", "Sweden", "Switzerland", "Norway",
            "United States", "Canada", "Australia"
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentShipmentViewModel" />.
        /// </summary>
        public PaymentShipmentViewModel(
            IMollieService mollieService,
            ITimerService timerService,
            IDatabaseManager databaseManager,
            ISharedMethodService sharedMethodService,
            ICodeGeneratorService codeService,
            IPdfGeneratorService pdfGeneratorService,
            IBackendCommunicationService backendCommunicationService,
            IConnectivityService connectivityService,
            IBackendDatabaseManager backendDatabaseManager,
            IBackendDatabaseHelper backendDatabaseHelper,
            IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;

            _mollieService = mollieService;
            _timeService = timerService;
            _databaseManager = databaseManager;
            _sharedMethodService = sharedMethodService;
            _codeService = codeService;
            _pdfService = pdfGeneratorService;
            _backendCommunicationService = backendCommunicationService;
            _connectivityService = connectivityService;
            _backendDatabaseHelper = backendDatabaseHelper;
            _backendDatabaseManager = backendDatabaseManager;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;

            SelectedCurrencyItem = CurrencyItem[0];
        }

        /// <summary>
        /// Initializes the component asynchronously.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;
            ProductTitle = parameter.Title;

            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
                BankTransferVisible = false;
            else
                BankTransferVisible = true;

            // Set initial amounts with default currency (EUR, no country selected yet)
            UpdateTotalAmount();
        }

        // ── Observable properties ─────────────────────────────────────────────────

        /// <summary>Represents the product title.</summary>
        [ObservableProperty]
        private string _productTitle;

        /// <summary>Represents the subtotal (product price only, before shipping).</summary>
        [ObservableProperty]
        private string _subtotalAmount;

        /// <summary>Represents the shipping cost formatted for display e.g. "€15.00" or "Free".</summary>
        [ObservableProperty]
        private string _shippingCostDisplay;

        /// <summary>Represents the grand total (subtotal + shipping).</summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>Represents the visibility of the bank transfer button.</summary>
        [ObservableProperty]
        private bool _bankTransferVisible = true;

        /// <summary>Represents the customer name.</summary>
        [ObservableProperty]
        private string _name;

        /// <summary>Represents the street name.</summary>
        [ObservableProperty]
        private string _streetName;

        /// <summary>Represents the house number.</summary>
        [ObservableProperty]
        private string _houseNumber;

        /// <summary>Represents the ZIP code.</summary>
        [ObservableProperty]
        private string _zipCode;

        /// <summary>Represents the city name.</summary>
        [ObservableProperty]
        private string _cityName;

        /// <summary>Represents the country name (kept in sync with SelectedCountry).</summary>
        [ObservableProperty]
        private string _countryName;

        /// <summary>Represents the email address.</summary>
        [ObservableProperty]
        private string _email;

        /// <summary>Represents the payment message visibility.</summary>
        [ObservableProperty]
        private bool _isPaymentMessageVisible = false;

        /// <summary>Represents the payment status message.</summary>
        [ObservableProperty]
        private string _paymentStatusMessage;

        /// <summary>
        /// Represents the selected currency item.
        /// Auto-recalculates totals when changed via CommunityToolkit partial method hook.
        /// </summary>
        [ObservableProperty]
        private string _selectedCurrencyItem;

        /// <summary>
        /// Auto-called by CommunityToolkit whenever SelectedCurrencyItem changes.
        /// Recalculates subtotal, shipping and grand total in the new currency.
        /// </summary>
        partial void OnSelectedCurrencyItemChanged(string value)
        {
            UpdateTotalAmount();
        }

        /// <summary>
        /// Represents the selected country from the Picker.
        /// Auto-recalculates shipping cost and totals when changed via CommunityToolkit partial method hook.
        /// </summary>
        [ObservableProperty]
        private string _selectedCountry;

        /// <summary>
        /// Auto-called by CommunityToolkit whenever SelectedCountry changes.
        /// Syncs CountryName and recalculates shipping cost + grand total.
        /// </summary>
        partial void OnSelectedCountryChanged(string value)
        {
            // Keep CountryName in sync for AddInformationToProducts()
            CountryName = value;
            UpdateTotalAmount();
        }

        // ── Commands ──────────────────────────────────────────────────────────────

        public AsyncRelayCommand PaymentByBankCommand => new AsyncRelayCommand(async () =>
        {
            if (!ValidateFields()) return;

            AddInformationToProducts();
            IsPaymentMessageVisible = false;

            _product.CurrencySymbol = SelectedCurrencyItem;
            _product.ShippingCost = DetermineShippingCost();
            await NavigationService.Navigate<BankTransferView>(_product);
        });

        public AsyncRelayCommand PaymentByCardCommand => new AsyncRelayCommand(async () =>
        {
            if (!ValidateFields()) return;

            if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
            {
                await DialogService.ShowAlertDialog(
                    AppResources.Dialog_InternetConnection_Title,
                    AppResources.Dialog_InternetConnection_Message,
                    AppResources.Dialog_OK_Text);
                return;
            }

            AddInformationToProducts();
            IsPaymentMessageVisible = true;

            try
            {
                // Grand total in EUR (product base + shipping) sent to Mollie
                decimal grandTotalEur = _product.TotalPrice + DetermineShippingCost();

                var result = await _mollieService.CreatePaymentAsync(
                    grandTotalEur,
                    SelectedCurrencyItem,
                    "Card",
                    "Payment",
                    Email
                );

                if (result is PaymentResponse payment)
                {
                    if (payment.Links?.Checkout != null)
                    {
                        _paymentId = payment.Id;

                        _timer = _timeService.StartPeriodicTimer(_ =>
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await CheckPaymentStatusAsync();
                            });
                        }, TimeSpan.FromSeconds(15));

                        var browserMode = (_mauiEssentialsWrapper.GetDevicePlatform() == _mauiEssentialsWrapper.AndroidDevicePlatform)
                            ? BrowserLaunchMode.SystemPreferred
                            : BrowserLaunchMode.External;

                        await Browser.Default.OpenAsync(payment.Links.Checkout.Href, browserMode);
                    }
                    else
                    {
                        Console.WriteLine("Error: Failed to create payment.");
                        await DialogService.ShowAlertDialog(
                            AppResources.Dialog_Error,
                            AppResources.PaymentShipmentViewModel_FailedPayment,
                            AppResources.Dialog_OK_Text);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Unexpected result type from MollieService.");
                    await DialogService.ShowAlertDialog(
                        AppResources.Dialog_Error,
                        AppResources.PaymentShipmentViewModel_UnexpectedPayment,
                        AppResources.Dialog_OK_Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment Error: {ex.Message}");
                await DialogService.ShowAlertDialog(
                    AppResources.Dialog_Error,
                    ex.Message,
                    AppResources.Dialog_OK_Text);
            }
        });

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Recalculates SubtotalAmount, ShippingCostDisplay and TotalAmount.
        /// Called automatically when SelectedCountry or SelectedCurrencyItem changes.
        /// Shipping cost is fetched via _sharedMethodService.GetShippingCost().
        /// </summary>
        private void UpdateTotalAmount()
        {
            if (_product == null) return;

            var (rate, symbol) = GetCurrentCurrency();

            // 1. Subtotal
            decimal subtotalConverted = _product.TotalPrice * rate;
            SubtotalAmount = $"{symbol}{subtotalConverted:F0}";

            // 2. Shipping
            decimal shippingEur = DetermineShippingCost();
            decimal shippingConverted = shippingEur * rate;
            ShippingCostDisplay = shippingEur == 0
                ? AppResources.PaymentShipmentViewModel_FreeText
                : $"{symbol}{shippingConverted:F0}";

            // 3. Grand total
            decimal grandTotal = subtotalConverted + shippingConverted;
            TotalAmount = $"{symbol}{grandTotal:F0}";
        }

        /// <summary>
        /// Returns the rate and symbol for the currently selected currency.
        /// Falls back to EUR if nothing is selected.
        /// </summary>
        private (decimal Rate, string Symbol) GetCurrentCurrency()
        {
            if (SelectedCurrencyItem != null &&
                CurrencyRates.TryGetValue(SelectedCurrencyItem, out var entry))
            {
                return entry;
            }
            return (1.00m, "€"); // Default EUR
        }

        /// <summary>
        /// Validates all required shipping/billing fields.
        /// Returns false and shows an alert if any field is missing or invalid.
        /// </summary>
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(HouseNumber) ||
                string.IsNullOrWhiteSpace(ZipCode) ||
                string.IsNullOrWhiteSpace(CityName) ||
                string.IsNullOrWhiteSpace(CountryName) ||
                string.IsNullOrWhiteSpace(Email))
            {
                _ = DialogService.ShowAlertDialog(
                    AppResources.CodeGeneratorViewModel_MissingTitle,
                    AppResources.PaymentShipmentViewModel_FillFieldText,
                    AppResources.Dialog_OK_Text);
                return false;
            }

            if (!IsValidEmail(Email))
            {
                _ = DialogService.ShowAlertDialog(
                    AppResources.PaymentShipmentViewModel_InvalidText,
                    AppResources.PaymentShipmentViewModel_EnterEmailText,
                    AppResources.Dialog_OK_Text);
                return false;
            }

            return true;
        }

        // ── Unchanged methods ─────────────────────────────────────────────────────

        public async Task HandleMollieRedirect(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId)) return;

            _paymentId = paymentId;
            StopTimer();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await CheckPaymentStatusAsync();
            });
        }

        private async Task CheckPaymentStatusAsync()
        {
            if (string.IsNullOrEmpty(_paymentId))
            {
                Console.WriteLine("PaymentShipmentViewModel:Error: No payment ID found.");
                return;
            }

            var paymentResponse = await _mollieService.GetPaymentStatusAsync(_paymentId);

            PaymentStatusMessage = paymentResponse.Status switch
            {
                "paid" => AppResources.General_PaidText,
                "pending" => AppResources.General_PendingText,
                "open" => AppResources.General_OpenText,
                "failed" => AppResources.General_FailedText,
                "canceled" => AppResources.General_CancelPaymentText,
                _ => AppResources.General_UnknownPaymentText
            };

            if (paymentResponse.Status != "paid") { return; }

            lock (_lock)
            {
                if (_orderProcessed) return;
                _orderProcessed = true;
            }

            StopTimer();

            PaymentStatusMessage = AppResources.PaymentShipmentViewModel_OrderReceived;

            var pdfFiles = new List<byte[]>();

            var processingMsg = AppResources.Dialog_Processing;
            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
                processingMsg = AppResources.PaymentShipmentViewModel_GeneratedCodeMsg;

            var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult(processingMsg,
            async () =>
            {
                pdfFiles = await GeneratePdfFilesAsync();
                return await DatabaseAndBackendStoringAsync(pdfFiles);
            });

            if (result)
            {
                await RemoveProductFromBasket();

                if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = (_product.CodeType == "QRcode")
                        ? $"{timestamp}Your_QR_Codes.pdf"
                        : $"{timestamp}Your_Barcodes.pdf";

                    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    File.WriteAllBytes(filePath, pdfFiles[0]);

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            Title = fileName,
                            File = new ShareFile(filePath)
                        });
                    });
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.Navigate<RootView>();
                });
            }
            else
            {
                await DialogService.ShowAlertDialog(
                    AppResources.PaymentShipmentViewModel_UnexpectedErrorText,
                    AppResources.Dialog_OK_Text);
            }
        }

        private async Task<bool> DatabaseAndBackendStoringAsync(List<byte[]> pdfFiles)
        {
            try
            {
                var dbItems = await _databaseManager.GetListAsync<YoursOrderData>();
                var existing = dbItems.FirstOrDefault(x =>
                    x.Title == _product.Title &&
                    x.OrderId == _product.OrderId
                );

                if (existing != null)
                {
                    Console.WriteLine("PaymentShipmentViewModel:Error:Order already exists — skipping insert.");
                    return true;
                }

                var orderedItem = new YoursOrderData
                {
                    OrderId = _product.OrderId,
                    Title = _product.Title,
                    Description = _product.Description,
                    CodeType = GetCodeAndPageType(_product.CodeType),
                    PageType = GetCodeAndPageType(_product.PageType),
                    ProductQuantity = _product.NumberOfPages,
                    DateTime = DateTime.Now,
                    TotalPrice = _sharedMethodService.ParsePrice(TotalAmount).ToString(),
                    Name = _product.Name,
                    Street = _product.Street,
                    HouseNo = _product.HouseNo,
                    ZipCode = _product.ZipCode,
                    City = _product.City,
                    Country = _product.Country,
                    Email = _product.Email,
                    ReferenceCode = "PaidByCard",
                    ShipmentTracking = "DHL:",
                    StatusOfOrder = StatusOfOrder(),
                    PdfFiles = pdfFiles
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
                Console.WriteLine($"DatabaseAndBackendStoringAsync::Exception during add: {ex}");
                return false;
            }
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timeService.StopPeriodicTimer(_timer);
                _timer.Dispose();
                _timer = null;
            }
        }

        private string GetCodeAndPageType(string data)
        {
            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
                return data;
            return string.Empty;
        }

        private string StatusOfOrder()
        {
            string status = AppResources.General_PendingStatusText;
            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
                status = AppResources.General_DownloadStatusText;
            return status;
        }

        private async Task<List<byte[]>> GeneratePdfFilesAsync()
        {
            var pdfBytes = new List<byte[]>();

            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
            {
                if (_product.CodeType == AppResources.CodeGeneratorViewModel_QRcodeText)
                {
                    var qrCodesCustom = await _codeService.GenerateQrCodesAsync(
                        tag: _product.TagName,
                        noOfPage: _product.NumberOfPages,
                        hexColor: _product.ColorHex,
                        pageType: _product.PageType
                    );

                    var pdf = new byte[0];

                    if (_product.PageType.Contains("A5"))
                    {
                        pdf = await _pdfService.GenerateQrPdfA5Async(qrCodesCustom);
                    }
                    else
                    {
                        pdf = await _pdfService.GenerateQrPdfA4Async(qrCodesCustom);
                    }
                   
                    pdfBytes.Add(pdf);
                }
                else
                {
                    var barCodesCustom = await _codeService.GenerateBarcodesAsync(
                        tag: _product.TagName,
                        noOfPage: _product.NumberOfPages,
                        pageType: _product.PageType
                    );

                    var pdf = new byte[0];

                    if (_product.PageType.Contains("A5"))
                    {
                        pdf = await _pdfService.GenerateBarcodePdfA5Async(barCodesCustom);
                    }
                    else
                    {
                        pdf = await _pdfService.GenerateBarcodePdfA4Async(barCodesCustom);
                    }

                    pdfBytes.Add(pdf);
                }
            }

            return pdfBytes;
        }

        private async Task RemoveProductFromBasket()
        {
            var dbItems = await _databaseManager.GetListAsync<AddToBasketData>();

            var match = dbItems.FirstOrDefault(x =>
                x.Title == _product.Title &&
                x.OrderId == _product.OrderId
            );

            if (match != null)
                await _databaseManager.DeleteAsync(match);
        }

        private void AddInformationToProducts()
        {
            _product.Name = Name;
            _product.Street = StreetName;
            _product.HouseNo = HouseNumber;
            _product.ZipCode = ZipCode;
            _product.City = CityName;
            _product.Country = CountryName;
            _product.Email = Email;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        } 

        private decimal DetermineShippingCost()
        {
            if (_product.Title.Contains("GQB") && CODE_GENERATED_NAME.Contains("GQB"))
            {
                return _sharedMethodService.GetShippingCost("Germany");
            }
            return _sharedMethodService.GetShippingCost(SelectedCountry);
        }
    }
}
