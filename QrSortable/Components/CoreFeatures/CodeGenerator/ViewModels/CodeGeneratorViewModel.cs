namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Google.Rpc;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the code generator view screen.
    /// </summary>
    public partial class CodeGeneratorViewModel : BaseViewModel<Product>
    {
        private const int _priceEachPaper = 5;

        private Product _product;

        /// <summary>
        /// .....................
        /// </summary>
        public ObservableCollection<string> PageSelected { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CodeGeneratorViewModel" />.
        /// </summary>
        public CodeGeneratorViewModel()
        {
            IsBackNavigationEnabled = true;

            PageSelected = new ObservableCollection<string>
            {
                "A4(12 code)",
                "A5(6 code)"
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
        private string _selectedCode = "QR code";

        /// <summary>
        /// Represents the currently selected tag name in the application.
        /// </summary>
        [ObservableProperty]
        private string _tagName="";

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
        private string _totalAmount = _priceEachPaper.ToString() + "€";


        public AsyncRelayCommand OnSelectionCodeChangedCommand => new AsyncRelayCommand(async () =>
        {
            if (!string.IsNullOrWhiteSpace(SelectedCode))
            {
                if (SelectedCode == "QR code")
                {
                    ColorVisible = true;
                    DemoImage = "qr_code_demo.png";

                    PageSelected.Clear();
                    PageSelected.Add("A4(12 code)");
                    PageSelected.Add("A5(6 code)");
                }
                else if (SelectedCode == "Bar code")
                {
                    ColorVisible = false;
                    DemoImage = "bar_code_demo.png";

                    PageSelected.Clear();
                    PageSelected.Add("A4(18 code)");
                    PageSelected.Add("A5(10 code)");

                }

            }
        });
        

        public AsyncRelayCommand DecreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            if (PageCount > 1) 
            { 
                PageCount--;
                TotalAmount = (PageCount * _priceEachPaper).ToString() + "€";
            }
        });

        public AsyncRelayCommand IncreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            PageCount++;
            TotalAmount = (PageCount * _priceEachPaper).ToString() + "€";
        });

        public AsyncRelayCommand BuyNowCommand => new AsyncRelayCommand(async () =>
        {
            _product.CodeType = SelectedCode;
            _product.PageType = SelectedPage;
            _product.TagName = TagName;
            _product.ColorHex = HexCode;
            _product.NumberOfPages = PageCount;
            _product.TotalPrice = PageCount * _priceEachPaper;

            await NavigationService.Navigate<PaymentShipmentView>(_product);
        });

    }
}