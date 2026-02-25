namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Views;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;
    using System.Globalization;

    /// <summary>
    ///     The view model of the Select Product view screen.
    /// </summary>
    public partial class SelectProductViewModel : BaseViewModel
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IGeneralInformationManager _generalInformationManager;
        public ObservableCollection<Product> Products { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectProductViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public SelectProductViewModel(IDatabaseManager databaseManager, IGeneralInformationManager generalInformationManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _generalInformationManager = generalInformationManager;
            
            LoadProducts();
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

        public async override void ViewAppearing()
        {
            base.ViewAppearing();
            try
            {
                var basketData = await _databaseManager.GetListAsync<AddToBasketData>();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BasketCount = basketData != null && basketData.Count > 0
                        ? basketData.Count.ToString() : "0";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SelectProductViewModel:Error loading categories: {ex.Message}");
            }
        }

        /// <summary>
        /// Represents the currently selected item in the application.
        /// </summary>
        [ObservableProperty]
        private Product _selectedItem;

        /// <summary>
        /// Represents the currently add to basket countin the application.
        /// </summary>
        [ObservableProperty]
        private string _basketCount;


        public AsyncRelayCommand OnSelectionChangedCommand => new AsyncRelayCommand(async () =>
        {
            if (SelectedItem!=null)
            {
                SelectedItem.OrderId = GenereateOrderedId();

                if (SelectedItem.Title == "Generate A4 QR or bar codes yourself!")
                {
                    await NavigationService.Navigate<CodeGeneratorView>(SelectedItem);
                }
                else
                {
                    await NavigationService.Navigate<PaperProductView>(SelectedItem);
                }

                SelectedItem = null;
            }
        });

        private async void LoadProducts()
        {
            Products = new ObservableCollection<Product>
            {
                new Product
                {
                    
                    Title = AppResources.SelectProductViewModel_SetOfA4QRcodeTitle,
                    Description = AppResources.SelectProductViewModel_SetOfA4QRcodeDiscription,
                    Price = string.Format(AppResources.SelectProductViewModel_SetOfA4QRcodePrice,
                    await GetFormattedPrice(10)),
                    IsNew = true,
                    ImageUrl = "qr_barcode_icon"
                },
                new Product
                 {
                    Title = "Set of 5 A4 Barcode",
                    Description = "* orange, yellow, green, red, pink",
                    Price = "10€ only",
                    ImageUrl = "bar_code"
                },
                new Product
                {
                    Title = "Generate A4 QR or bar codes yourself!",
                    Description = "* select color",
                    Price = "2€ only",
                    IsNew = true,
                    ImageUrl = "code_pdf_icon.png"
                }
            };
        }
        private string GenereateOrderedId()
        {
            string seg1 = GenerateNumber(6);
            string seg2 = GenerateNumber(4);
            string seg3 = GenerateNumber(3);

            return $"QS-{seg1}-{seg2}-{seg3}";
        }

        private static string GenerateNumber(int digits)
        {
            Random random = new Random();
            int max = (int)Math.Pow(10, digits);
            int min = max / 10;
            return random.Next(min, max).ToString();
        }

        public async Task<string> GetFormattedPrice(decimal price)
        {
            var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
            var selectedLanguage = generalInfo.SelectedLanguageCode;

            string symbol = selectedLanguage == "en" ? "$" : "€";

            return $"{symbol}{price:0}";
        }
    }
}