namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the code generator view screen.
    /// </summary>
    public partial class CodeGeneratorViewModel : BaseViewModel<Product>
    {
        private readonly IGeneralInformationManager _generalInformationManager;

        private readonly ISharedMethodService _sharedMethodService;

        private readonly IToastService _toastService;

        private const int _priceEachA4Paper = 5;
        private const int _priceEachA5Paper = 3;

        private string _currencySymbol = "€";

        private Product _product;

        /// <summary>
        /// .....................
        /// </summary>
        public ObservableCollection<string> PageSelected { get; set; }

        public ObservableCollection<string> Codes { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CodeGeneratorViewModel" />.
        /// </summary>
        public CodeGeneratorViewModel(IGeneralInformationManager generalInformationManager,
            ISharedMethodService sharedMethodService, IToastService toastService)
        {
            IsBackNavigationEnabled = true;

            _generalInformationManager = generalInformationManager;
            _sharedMethodService = sharedMethodService;
            _toastService = toastService;

            PageSelected = new ObservableCollection<string>
            {
                "A4(12 code)",
                "A5(6 code)"
            };

            Codes = new ObservableCollection<string>
            {
                AppResources.CodeGeneratorViewModel_QRcodeText,
                AppResources.CodeGeneratorViewModel_BarcodeText
            };

        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var language = (await _generalInformationManager.GetGeneralInformationAsync()).SelectedLanguageCode;

            _currencySymbol = _sharedMethodService.GetCurrencySymbol(language);
            TotalAmount = _priceEachA4Paper.ToString() + _currencySymbol;
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;
        }

        /// <summary>
        /// Represents the currently selected demo image in the application.
        /// </summary>
        [ObservableProperty]
        private string _demoImage = "qr_code_demo.png";

        /// <summary>
        /// Represents the currently selected code in the application.
        /// </summary>
        [ObservableProperty]
        private string _selectedCode = AppResources.CodeGeneratorViewModel_QRcodeText;

        /// <summary>
        /// Represents the currently selected tag name in the application.
        /// </summary>
        [ObservableProperty]
        private string _tagName = "";

        /// <summary>
        /// Represents the currently selected color hex code in the application.
        /// </summary>
        [ObservableProperty]
        private string _hexCode = "#000000";

        /// <summary>
        /// Represents the currently color item in the application.
        /// </summary>
        [ObservableProperty]
        private bool _colorVisible = true;

        /// <summary>
        /// Represents the currently number page count in the application.
        /// </summary>
        [ObservableProperty]
        private int _pageCount = 1;


        [ObservableProperty]
        private string _selectedPage = "A4(12 code)";

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;


        public AsyncRelayCommand OnSelectionCodeChangedCommand => new AsyncRelayCommand(async () =>
        {
            if (!string.IsNullOrWhiteSpace(SelectedCode))
            {
                if (SelectedCode == "QRcode")
                {
                    ColorVisible = true;
                    DemoImage = "qr_code_demo.png";

                    PageSelected.Clear();
                    PageSelected.Add("A4(12 code)");
                    PageSelected.Add("A5(6 code)");

                    SelectedPage = "A4(12 code)";
                }
                else if (SelectedCode == "Barcode")
                {
                    ColorVisible = false;
                    DemoImage = "bar_code_demo.png";

                    PageSelected.Clear();
                    PageSelected.Add("A4(18 code)");
                    PageSelected.Add("A5(10 code)");

                    SelectedPage = "A4(18 code)";
                }

            }
        });

        public AsyncRelayCommand OnSelectionPageTypeChangedCommand => new AsyncRelayCommand(async () =>
        {
            if (!string.IsNullOrWhiteSpace(SelectedPage))
            {
                TotalAmount = (PageCount * GetPriceEachPaper()).ToString() + _currencySymbol;
            }
        });


        public AsyncRelayCommand DecreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            if (PageCount > 1)
            {
                PageCount--;

                TotalAmount = (PageCount * GetPriceEachPaper()).ToString() + _currencySymbol;
            }
        });

        public AsyncRelayCommand IncreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            if (PageCount >= 5)
            {
                await _toastService.DisplayToast(AppResources.CodeGeneratorViewModel_MaxPage);
                return;
            }

            PageCount++;
            TotalAmount = (PageCount * GetPriceEachPaper()).ToString() + _currencySymbol;
        });

        public AsyncRelayCommand BuyNowCommand => new AsyncRelayCommand(async () =>
        {

            if (string.IsNullOrWhiteSpace(SelectedCode))
            {
                await DialogService.ShowAlertDialog(AppResources.CodeGeneratorViewModel_MissingTitle,
                    AppResources.CodeGeneratorViewModel_SelectCode, AppResources.Dialog_OK_Text);
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPage))
            {
                await DialogService.ShowAlertDialog(AppResources.CodeGeneratorViewModel_MissingTitle,
                    AppResources.CodeGeneratorViewModel_SelectPage, AppResources.Dialog_OK_Text);
                return;
            }

            _product.CodeType = SelectedCode;
            _product.PageType = SelectedPage;
            _product.TagName = TagName;
            _product.ColorHex = HexCode;
            _product.NumberOfPages = PageCount;
            _product.TotalPrice = PageCount * GetPriceEachPaper();

            await NavigationService.Navigate<PaymentShipmentView>(_product);
        });

        private int GetPriceEachPaper()
        {
            if (SelectedPage.Contains("A5"))
                return _priceEachA5Paper;
            else
                return _priceEachA4Paper;

        }
    }
}