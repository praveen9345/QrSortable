namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the PaperProductView view.
    /// </summary>
    public partial class PaperProductView : BaseView
    {

        private readonly PaperProductViewModel _viewModel;
        /// <summary>
        ///  Initializes a new instance of the PaperProductViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The PaperProductViewModel associated with this view.</param>
        public PaperProductView(PaperProductViewModel viewModel) : base(viewModel)
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