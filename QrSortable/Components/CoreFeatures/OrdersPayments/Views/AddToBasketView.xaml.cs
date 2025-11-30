namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the AddToBasketView.
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

        private void OnIncreaseQuantityButtonClicked(object sender, EventArgs e)
        {
            DisplayAlert("Clicked!", "You clicked the button.", "OK");
        }
        private void OnDecreaseQuantityButtonClicked(object sender, EventArgs e)
        {
            DisplayAlert("Clicked!", "You clicked the button.", "OK");
        }
    }
}