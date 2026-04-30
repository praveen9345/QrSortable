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

                if (SelectedItem.Title == AppResources.SelectProductViewModel_GenrateOfA4QRcodeTitle)
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
                    Title = AppResources.SelectProductViewModel_GenrateOfA4QRcodeTitle,
                    Description = AppResources.SelectProductViewModel_GenrateOfA4QRcodeColor,
                    Price = string.Format(AppResources.SelectProductViewModel_GenrateOfA4QRcodePrice,
                    await GetFormattedPrice(5)),
                    IsNew = false,
                    ImageUrl = "code_pdf_icon.png"
                },
                new Product
                {      
                    Title = AppResources.SelectProductViewModel_StandardPack48QRTitle,
                    Description = AppResources.SelectProductViewModel_StandardPack48QRDiscription,
                    Price = string.Format(AppResources.SelectProductViewModel_StandardPack48QRPrice,
                    await GetFormattedPrice(15)),
                    IsNew = true,
                    ImageUrl = "qr_standerd_pack_1.png"
                },
                new Product
                 {
                    Title = AppResources.SelectProductViewModel_StandardPack48BrTitle,
                    Description = AppResources.SelectProductViewModel_StandardPack48BrDiscription,
                    Price = string.Format(AppResources.SelectProductViewModel_StandardPack48BrPrice,
                    await GetFormattedPrice(15)),
                     IsNew = true,
                    ImageUrl = "br_standerd_pack_1.png"
                },
                new Product
                 {
                    Title = AppResources.SelectProductViewModel_LargePack100QRTitle,
                    Description = AppResources.SelectProductViewModel_LargePack100QRDiscription,
                    Price = string.Format(AppResources.SelectProductViewModel_LargePack100QRPrice,
                    await GetFormattedPrice(30)),
                    IsNew = true,
                    ImageUrl = "qr_large_pack_1.png"
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