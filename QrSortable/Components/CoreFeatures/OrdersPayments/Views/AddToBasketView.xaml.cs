namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Models;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the AddToBasketView.
    /// </summary>
    public partial class AddToBasketView : BaseView
    {
        private readonly AddToBasketViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the AddToBasketViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The AddToBasketViewModel associated with this view.</param>
        public AddToBasketView(AddToBasketViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

        }

        private async void OnDecreaseQuantityButtonClicked(object sender, EventArgs e)
        {            
            if (sender is Button btn && btn.BindingContext is BasketData item)
            {
                if (item.ProductQuantity <= 1) return;

                item.ProductQuantity -= 1;
                await UpdateItemDatabaseAsync(item);
            }
        }

        private async void OnIncreaseQuantityButtonClicked(object sender, EventArgs e)
        {

            if (sender is Button btn && btn.BindingContext is BasketData item)
            {
                item.ProductQuantity += 1;
                await UpdateItemDatabaseAsync(item);
            }

        }

        private async Task<bool> UpdateItemDatabaseAsync( BasketData item) 
        {
            try
            {
                var dbItems = await _viewModel._databaseManager.GetListAsync<AddToBasketData>();

                var match = dbItems.FirstOrDefault(x => x.DateTime == item.DateTime && x.Title == item.Title);

                if (match is null)
                    return false;

                var unitPrice = _viewModel._sharedMethodService.ParsePrice(match.Price);
                match.ProductQuantity = item.ProductQuantity;
                match.TotalPrice = unitPrice * match.ProductQuantity;

                await _viewModel._databaseManager.UpdateAsync(match);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddToBasketView:UpdateItemDatabaseAsync:Error updating item in database: {ex.Message}");
                return false;
            }
        }

    }
}