namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Google.Cloud.Firestore;
    using Microsoft.Maui.Storage;
    using QrSortable.Components.CoreFeatures.CodeGenerator;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
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
    ///     The view model of the Select Product view screen.
    /// </summary>
    public partial class PaymentShipmentViewModel : BaseViewModel<Product>
    {
        private readonly IMollieService _mollieService;
        private readonly ITimerService _timeService;
        public readonly IDatabaseManager _databaseManager;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly ICodeGeneratorService _codeService;
        private readonly IPdfGeneratorService _pdfService;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        private Product _product;
        private string _paymentId;
        private Timer _timer;
        private bool _orderProcessed = false;
        private readonly object _lock = new object();
        private const string CODE_GENERATED_NAME = "Generate A4 QR or bar code yourself!";

        public ObservableCollection<string> CurrencyItem { get; } =
        new ObservableCollection<string>
        {
           "Euro(€)",
           "USD($)"
        };


        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentShipmentViewModel" />.
        /// </summary>
        /// <param name="mollieService">The service handling Mollie payment-related operations.</param>
        /// <param name="timerService">The service managing timing-related operations.</param>
        /// <param name="mauiEssentialsWrapper">An instance of <see cref="IMauiEssentialsWrapper" /> used to access platform-specific features.</param>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public PaymentShipmentViewModel(IMollieService mollieService, ITimerService timerService, IDatabaseManager databaseManager,
            ISharedMethodService sharedMethodService, ICodeGeneratorService codeService, IPdfGeneratorService pdfGeneratorService,
            IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;

            _mollieService = mollieService;
            _timeService = timerService;
            _databaseManager = databaseManager;
            _sharedMethodService = sharedMethodService;
            _codeService = codeService;
            _pdfService = pdfGeneratorService;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;

            SelectedCurrencyItem = CurrencyItem[0];
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
            ProductTitle = parameter.Title;

            TotalAmount = _product.TotalPrice.ToString() + "€";
            if (_product.Title == CODE_GENERATED_NAME) 
            { 
                BankTransferVisible = false; 
            }
            else { BankTransferVisible = true; }
        }

        /// <summary>
        /// Represents the currently product tile in the application.
        /// </summary>
        [ObservableProperty]
        private string _productTitle;

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>
        /// Represents the visisble of the bank transfer button in the application.
        /// </summary>
        [ObservableProperty]
        private bool _bankTransferVisible = true;

        /// <summary>
        /// Represents the currently name in the application.
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Represents the currently street name in the application.
        /// </summary>
        [ObservableProperty]
        private string _streetName;

        /// <summary>
        /// Represents the currently house number in the application.
        /// </summary>
        [ObservableProperty]
        private string _houseNumber;

        /// <summary>
        /// Represents the currently zip code in the application.
        /// </summary>
        [ObservableProperty]
        private string _zipCode;

        /// <summary>
        /// Represents the currently city name in the application.
        /// </summary>
        [ObservableProperty]
        private string _cityName;

        /// <summary>
        /// Represents the currently Country Name in the application.
        /// </summary>
        [ObservableProperty]
        private string _countryName;

        /// <summary>
        /// Represents the currently Email in the application.
        /// </summary>
        [ObservableProperty]
        private string _email;

        /// <summary>
        /// Represents the currently payment message visible in the application.
        /// </summary>
        [ObservableProperty]
        private bool _isPaymentMessageVisible = false;

        /// <summary>
        /// Represents the payment status message in the application.
        /// </summary>
        [ObservableProperty]
        private string _paymentStatusMessage;

        /// <summary>
        /// Represents the currently selected currenc item in the application.
        /// </summary>
        [ObservableProperty]
        private string _selectedCurrencyItem;

        public AsyncRelayCommand OnSelectionChangedCommand => new AsyncRelayCommand(async () =>
        {
            if (SelectedCurrencyItem != null)
            {
                if(SelectedCurrencyItem == CurrencyItem[0])
                {
                    TotalAmount = _product.TotalPrice.ToString() + "€";
                }
                else 
                {
                    TotalAmount = _product.TotalPrice.ToString() + "$";
                }
            }
        });

        public AsyncRelayCommand PaymentByBankCommand => new AsyncRelayCommand(async () =>
        {

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(HouseNumber) ||
                string.IsNullOrWhiteSpace(ZipCode) ||
                string.IsNullOrWhiteSpace(CityName) ||
                string.IsNullOrWhiteSpace(CountryName) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowAlertDialog("Missing Information",
                    "Please fill in all fields before proceeding.", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await DialogService.ShowAlertDialog("Invalid Email",
                    "Please enter a valid email address.", "OK");
                return;
            }

            // Add info to product
            AddInformationToProducts();

            IsPaymentMessageVisible = false;

            // Navigate to BankTransferView
            await NavigationService.Navigate<BankTransferView>(_product);
        });

        public AsyncRelayCommand PaymentByCardCommand => new AsyncRelayCommand(async () =>
        {

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(HouseNumber) ||
                string.IsNullOrWhiteSpace(ZipCode) ||
                string.IsNullOrWhiteSpace(CityName) ||
                string.IsNullOrWhiteSpace(CountryName) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowAlertDialog("Missing Information",
                    "Please fill in all fields before proceeding.", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await DialogService.ShowAlertDialog("Invalid Email",
                    "Please enter a valid email address.", "OK");
                return;
            }

            // Add info to product
            AddInformationToProducts();

            IsPaymentMessageVisible = true;

            var paymentResponse = await _mollieService.CreatePaymentAsync(
              _product.TotalPrice, SelectedCurrencyItem,"Card","Payment");

            if (paymentResponse != null && paymentResponse.Links.Checkout != null)
            {
                _paymentId = paymentResponse.Id;

                _timer = _timeService.StartPeriodicTimer(CheckStatusAsync, TimeSpan.FromSeconds(15));

                await Browser.Default.OpenAsync(paymentResponse.Links.Checkout.Href, BrowserLaunchMode.SystemPreferred);
            }
            else
            {
                Console.WriteLine("PaymentShipmentViewModel:Error: Failed to create payment.");
            }

        });

        private async void CheckStatusAsync(object state)
        {
            if (string.IsNullOrEmpty(_paymentId))
            {
                Console.WriteLine("PaymentShipmentViewModel:Error: No payment ID found.");
                return;
            }

            var paymentResponse = await _mollieService.GetPaymentStatusAsync(_paymentId);

            if (paymentResponse == null)
            {
                Console.WriteLine("PaymentShipmentViewModel:Error: Failed to retrieve payment status.");
                return;
            }

            PaymentStatusMessage = paymentResponse.Status switch
            {
                "paid" => "Your payment was successful!",
                "pending" => "Your payment is pending.",
                "open" => "Your payment is still open.",
                "failed" => "Your payment failed.",
                "canceled" => "Your payment was canceled.",
                _ => "Unknown payment status."
            };

            if (paymentResponse.Status != "paid") { return; }

            lock (_lock)
            {
                if (_orderProcessed)
                    return; // Already handled → exit immediately

                _orderProcessed = true;  // Mark as processed
            }

            _timeService.StopPeriodicTimer(_timer);
            _timer.Dispose();

            PaymentStatusMessage = "Order received! We'll send a confirmation email shortly.! 🎉";

            var pdfFiles = new List<byte[]>();

            var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
            async () =>
            {
                pdfFiles = await GeneratePdfFilesAsync();
                return await DatabaseAndBackendStoringAsync(pdfFiles); 
            });

            if (result)
            {
                await RemoveProductFromBasket();

                if(_product.Title == CODE_GENERATED_NAME)
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = (_product.CodeType == "QR code") ? $"{timestamp}Your_QR_Codes.pdf"
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
                await DialogService.ShowAlertDialog("An unexpected error occurred while saving the item. Please try again.", AppResources.Dialog_OK_Text);
            }
        }

        private async Task<bool> DatabaseAndBackendStoringAsync(List<byte[]> pdfFiles)
        {
            /*TODO: *send email to user and to me and 
            *send saved to the backgend when user placed an order
            */
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
                    CodeType = _product.CodeType,
                    PageType = _product.PageType,
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

        private string StatusOfOrder()
        {
            string status = "Pending...";
            if (_product.Title == CODE_GENERATED_NAME)
            {
                status = "Download";
            }
            return status;
        }

        private async Task<List<byte[]>> GeneratePdfFilesAsync()
        {
            var pdfBytes = new List<byte[]>();

            if (_product.Title == CODE_GENERATED_NAME)
            {
                if (_product.CodeType == "QR code")
                {
                    var qrCodesCustom = await _codeService.GenerateQrCodesAsync(
                        tag: _product.TagName,
                        noOfPage: _product.NumberOfPages,
                        hexColor: _product.ColorHex
                    );

                    var pdf = await _pdfService.GenerateQrPdfAsync(qrCodesCustom);
                    pdfBytes.Add(pdf);
                }
                else
                {
                    var barCodesCustom = await _codeService.GenerateBarcodesAsync(
                        tag: _product.TagName,
                        noOfPage: _product.NumberOfPages
                    );

                    var pdf = await _pdfService.GenerateBarcodePdfAsync(barCodesCustom);
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
            {
                await _databaseManager.DeleteAsync(match);
            }
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


    }
}