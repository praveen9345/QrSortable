namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;

    /// <summary>
    ///     The view model of the  BankTransferViewModel screen.
    /// </summary>
    public partial class BankTransferViewModel : BaseViewModel<Product>
    {
        private readonly IToastService _toastService;

        private Product _product;

        /// <summary>
        ///     Initializes a new instance of the <see cref=" BankTransferViewModel" />.
        /// </summary>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public BankTransferViewModel(IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _toastService = toastService;

            ReferenceCode = GenerateReferenceCode();
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
            var price = _product.TotalPrice;
            TotalAmount = price.ToString("F2") + "€";
            var discounted = (decimal)0.1 * price;
            DiscountAmount = "-" + discounted.ToString("F2") + "€";
            var netPrice = price - discounted;
            NetTotalAmount = netPrice.ToString("F2") + "€";
        }

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>
        /// Represents the currently discount amount in the application.
        /// </summary>
        [ObservableProperty]
        private string _discountAmount;

        /// <summary>
        /// Represents the currently net total amount in the application.
        /// </summary>
        [ObservableProperty]
        private string _netTotalAmount;

        /// <summary>
        /// Represents the currently iban code in the application.
        /// </summary>
        [ObservableProperty]
        private string _ibanCode = "DE89370400440532013000";

        /// <summary>
        /// Represents the currently reference code in the application.
        /// </summary>
        [ObservableProperty]
        private string _referenceCode;


        public AsyncRelayCommand CopyIbanCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(IbanCode);
            await _toastService.DisplayToast("IBAN copied to clipboard!");

        });

        public AsyncRelayCommand CopyReferenceCodeCommand => new AsyncRelayCommand(async () =>
        {

            await Clipboard.Default.SetTextAsync(ReferenceCode);
            await _toastService.DisplayToast("Reference code copied to clipboard!");

        });

        public AsyncRelayCommand PlaceAnOrderCommand => new AsyncRelayCommand(async () =>
        {
            await DialogService.ShowAlertDialog(
                "Confirmation",
                "Thank you for your order. We will send you an email shortly.",
                "OK");

            /*TODO: *send email to user and to me and 
                 *generated code send to the backgend when user placed an order
                 * save all data to the order database and send to backend
             */
        });

        private string GenerateReferenceCode()
        {
            string prefix = "#QS";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return prefix + randomPart; 
        }

    }

}
