namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the Select Product view screen.
    /// </summary>
    public partial class PaymentShipmentViewModel : BaseViewModel<Product>
    {
        private Product _product;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentShipmentViewModel" />.
        /// </summary>
        public PaymentShipmentViewModel()
        {
            IsBackNavigationEnabled = true;
           
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
            TotalAmount = _product.TotalPrice.ToString() + "€";
            if (_product.Title == "Generate A4 QR or bar code yourself!") 
            { 
                BankTransferVisible = false; 
            }
            else { BankTransferVisible = true; }
        }

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

        public AsyncRelayCommand SelectQrCommand => new AsyncRelayCommand(async () =>
        {

          

        });

        public AsyncRelayCommand SelectBarcodeCommand => new AsyncRelayCommand(async () =>
        {



        });

    }
}