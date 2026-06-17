namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using Microsoft.Maui.Controls;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.IO;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class SelectProductView : BaseView
    {
        private readonly SelectProductViewModel _viewModel;
        /// <summary>
        ///  Initializes a new instance of the SelectProductViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SelectProductViewModel associated with this view.</param>
        public SelectProductView(SelectProductViewModel viewModel) : base(viewModel)
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