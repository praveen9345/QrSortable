namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using Microsoft.Maui.Controls;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.IO;
    using ViewModels;

    /// <summary>
    /// The code behind of the AddToBasketView view.
    /// </summary>
    public partial class AddToBasketView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the AddToBasketViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The AddToBasketViewModel associated with this view.</param>
        public AddToBasketView(AddToBasketViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}