namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using Android.Webkit;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Views;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the Select Product view screen.
    /// </summary>
    public partial class SelectProductViewModel : BaseViewModel
    {
        private readonly IDatabaseManager _databaseManager;
        public ObservableCollection<Product> Products { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectProductViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public SelectProductViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            
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
            
            try
            {
                var basketData = await _databaseManager.GetListAsync<AddToBasketData>();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BasketCount = basketData != null && basketData.Count > 0
                        ? basketData.Count.ToString()
                        : "0";
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
                if (SelectedItem.Title == "Generate A4 QR or bar code yourself!")
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




        private void LoadProducts()
        {
            Products = new ObservableCollection<Product>
            {
                new Product
                {

                    Title = "Set of 5 A4 QR code",
                    Description = "* orange, yellow, green, red, pink",
                    Price = "10€ only",
                    IsNew = true,
                    ImageUrl = "qr_barcode_icon"
                },
                new Product
                 {
                    Title = "Set of 5 A4 Bar code",
                    Description = "* orange, yellow, green, red, pink",
                    Price = "10€ only",
                    ImageUrl = "bar_code"
                },
                new Product
                {
                    Title = "Generate A4 QR or bar code yourself!",
                    Description = "* select color",
                    Price = "2€ only",
                    IsNew = true,
                    ImageUrl = "code_pdf_icon.png"
                }
            };
        }
    }
}