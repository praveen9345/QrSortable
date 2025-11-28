namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the BankTransferView view.
    /// </summary>
    public partial class BankTransferView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the BankTransferViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The BankTransferViewModel associated with this view.</param>
        public BankTransferView(BankTransferViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}