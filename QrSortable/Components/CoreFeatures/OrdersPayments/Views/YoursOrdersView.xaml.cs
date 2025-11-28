namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the YoursOrdersView.
    /// </summary>
    public partial class YoursOrdersView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the YoursOrdersViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The YoursOrdersViewModel associated with this view.</param>
        public YoursOrdersView(YoursOrdersViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}