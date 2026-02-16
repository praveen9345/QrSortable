namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the SubscriptionView view.
    /// </summary>
    public partial class SubscriptionView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the SubscriptionViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SubscriptionViewModel associated with this view.</param>
        public SubscriptionView(SubscriptionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}