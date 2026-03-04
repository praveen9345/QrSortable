namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the PaperProductView screen.
    /// </summary>
    public partial class PaperProductViewModel : BaseViewModel<Product>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly ISharedMethodService _sharedMethodService;

        private Product _product;
        private int _amount;
        private string _curruncy;

        /// <summary>
        /// A collection of images associated with the storage entry.
        /// </summary>
        public ObservableCollection<Images> ImageArray { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaperProductViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public PaperProductViewModel(IDatabaseManager databaseManager, IToastService toastService,
            IGeneralInformationManager generalInformationManager, ISharedMethodService sharedMethodService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;  
            _generalInformationManager = generalInformationManager;
            _sharedMethodService = sharedMethodService;

            ImageArray = new ObservableCollection<Images>();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadImagesAsync();
        }
        public async override void ViewAppearing()
        {
            base.ViewAppearing();
            await LoadBasketCountAsync();
        }

        public override async void Prepare(Product parameter)
        {
            _product = parameter;
            Title = _product.Title;
            Discription = _product.Description;
            string numberString = new string(_product.Price.Where(char.IsDigit).ToArray());
            _amount = int.Parse(numberString);

            var language = (await _generalInformationManager.GetGeneralInformationAsync()).SelectedLanguageCode;

            _curruncy = _sharedMethodService.GetCurrencySymbol(language);

            TotalAmount = _amount.ToString() + _curruncy;
        }

        /// <summary>
        /// Represents the currently title in the application.
        /// </summary>
        [ObservableProperty]
        private string _title;

        /// <summary>
        /// Represents the currently discription in the application.
        /// </summary>
        [ObservableProperty]
        private string _discription;

        /// <summary>
        /// Represents the currently number page count in the application.
        /// </summary>
        [ObservableProperty]
        private int _productCount = 1;

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>
        /// Represents the currently add to basket countin the application.
        /// </summary>
        [ObservableProperty]
        private string _basketCount;

        public AsyncRelayCommand DecreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            if (ProductCount > 1)
            {
                ProductCount--;
                TotalAmount = (ProductCount * _amount).ToString() + _curruncy;
            }
        });

        public AsyncRelayCommand IncreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            ProductCount++;
            TotalAmount = (ProductCount * _amount).ToString() + _curruncy;
        });


        public AsyncRelayCommand AddToBasketCommand => new AsyncRelayCommand(async () =>
        {
            string totalAmountDeciaml = new string(TotalAmount.Where(char.IsDigit).ToArray());
            try
            {
                var basketItem = new AddToBasketData
                {
                    OrderId = _product.OrderId,
                    Title = _product.Title,
                    Description = _product.Description,
                    Price = _product.Price,
                    ProductQuantity = ProductCount,
                    DateTime = DateTime.Now,
                    TotalPrice = decimal.Parse(totalAmountDeciaml)
                };
                _databaseManager.BeginTransaction();
                var addedItem = await _databaseManager.AddAsync(basketItem);

                if (addedItem != null)
                {
                    _databaseManager.CommitTransaction();
                    await LoadBasketCountAsync();
                    await _toastService.DisplayToast(AppResources.PaperProductViewModel_BasketSuccess);
                }
                else
                {
                    _databaseManager.Rollback();
                    await DialogService.ShowAlertDialog(
                         AppResources.PaperProductViewModel_ErrorSaveItem, AppResources.Dialog_OK_Text);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PaperProductViewModel:Error loading categories: {ex.Message}");
            }
        });

        public AsyncRelayCommand BuyNowCommand => new AsyncRelayCommand(async () =>
        {
            string totalAmountDeciaml = new string(TotalAmount.Where(char.IsDigit).ToArray());
            _product.TotalPrice = decimal.Parse(totalAmountDeciaml);
            await NavigationService.Navigate<PaymentShipmentView>(_product);
        });


        private async Task LoadBasketCountAsync()
        {
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
                Console.WriteLine($"PaperProductViewModel: Error loading basket data: {ex.Message}");
            }
        }

        private async Task LoadImagesAsync()
        {
            if (string.IsNullOrWhiteSpace(_product?.Title))
                return;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ImageArray.Clear();

                if (_product.Title == AppResources.SelectProductViewModel_StandardPack48QRTitle)
                {
                    ImageArray.Add(new Images() { Image = "qr_standerd_pack_1.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "qr_standerd_pack_2.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "qr_standerd_pack_3.png", Rotate = 0 });
                }
                else if (_product.Title == AppResources.SelectProductViewModel_StandardPack48BrTitle)
                {
                    ImageArray.Add(new Images() { Image = "br_standerd_pack_1.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "br_standerd_pack_2.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "br_standerd_pack_3.png", Rotate = 0 });
                }
                else if (_product.Title == AppResources.SelectProductViewModel_LargePack100QRTitle)
                {
                    ImageArray.Add(new Images() { Image = "qr_large_pack_1.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "qr_large_pack_2.png", Rotate = 0 });
                    ImageArray.Add(new Images() { Image = "qr_large_pack_3.png", Rotate = 0 });
                }
            });
        }

    }
}