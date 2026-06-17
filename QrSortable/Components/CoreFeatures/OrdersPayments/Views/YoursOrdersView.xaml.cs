namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the YoursOrdersView.
    /// </summary>
    public partial class YoursOrdersView : BaseView
    {
        private readonly YoursOrdersViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the YoursOrdersViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The YoursOrdersViewModel associated with this view.</param>
        public YoursOrdersView(YoursOrdersViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnCartTapped(object sender, EventArgs e)
        {
            var view = sender as View;

            if (view != null)
            {
                await view.ScaleTo(0.9, 100);
                await view.ScaleTo(1, 100);
            }

            await _viewModel.NavigationService.Navigate<AddToBasketView>();

        }
    }
}