namespace QrSortable.Components.CoreFeatures.CodeGenerator.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the PaperProductView screen.
    /// </summary>
    public partial class PaperProductViewModel : BaseViewModel<Product>
    {
        private readonly IDatabaseManager _databaseManager;

        private Product _product;

        private int _amount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaperProductViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public PaperProductViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
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
                Console.WriteLine($"PaperProductViewModel:Error loading categories: {ex.Message}");
            }
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;
            Title = _product.Title;
            Discription = _product.Description;
            string numberString = new string(_product.Price.Where(char.IsDigit).ToArray());
            _amount = int.Parse(numberString);

            TotalAmount = _amount.ToString() + "€";
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
                TotalAmount = (ProductCount * _amount).ToString() + "€";
            }
        });

        public AsyncRelayCommand IncreaseQuantityCommand => new AsyncRelayCommand(async () =>
        {
            ProductCount++;
            TotalAmount = (ProductCount * _amount).ToString() + "€";
        });


        public AsyncRelayCommand AddToBasketCommand => new AsyncRelayCommand(async () =>
        {
            await NavigationService.Navigate<AddToBasketView>();
        });

        public AsyncRelayCommand BuyNowCommand => new AsyncRelayCommand(async () =>
        {
            //await NavigationService.Navigate<PaymentShipmentView>(_product);
        });
    }
}