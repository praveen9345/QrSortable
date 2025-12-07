namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    ///     The view model of the AddToBasketViewModel screen.
    /// </summary>
    public partial class AddToBasketViewModel : BaseViewModel
    {
        public readonly IDatabaseManager _databaseManager;

        public readonly ISharedMethodService _sharedMethodService;

        // Stores the last known hash of the database
        private string _lastBasketHash = string.Empty;

        public ObservableCollection<BasketData> AddToBasketData { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddToBasketViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        ///  <param name="sharedMethodService">An instance of <see cref="ISharedMethodService" /> 
        /// used foracces the global methods.</param>
        public AddToBasketViewModel(IDatabaseManager databaseManager, ISharedMethodService sharedMethodService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _sharedMethodService = sharedMethodService;
            AddToBasketData = new ObservableCollection<BasketData>();

        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();
            await LoadBasketCountAsync();
        }

        public AsyncRelayCommand<BasketData> DeleteAddedOrderCommand => new AsyncRelayCommand<BasketData>(async (item) =>
        {
            if (item == null) return;

            var confirm = await DialogService.ShowRequestDialog("Are you sure you want to remove this item from the basket?",
                AppResources.Dialog_Cancel_Text,AppResources.Dialog_OK_Text);
            
            if (!confirm) return;

            try
            {
                var dbItems = await _databaseManager.GetListAsync<AddToBasketData>();

                var match = dbItems.FirstOrDefault(x =>
                    x.Title == item.Title &&
                    x.DateTime == item.DateTime &&
                    x.OrderId == item.OrderId
                );

                if (match != null)
                {
                    await _databaseManager.DeleteAsync(match);
                }

                // Remove from UI collection
                var uiMatch = AddToBasketData.FirstOrDefault(x =>
                    x.Title == item.Title &&
                    x.DateTime == item.DateTime &&
                     x.OrderId == item.OrderId);

                if (uiMatch != null)
                    AddToBasketData.Remove(uiMatch);

                // Force refresh on next appearance
                _lastBasketHash = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddToBasketViewModel: DeleteAddedOrderCommand:Error updating item in database: {ex.Message}");
            }

        });

        public AsyncRelayCommand<BasketData> BuyNowCommand => new AsyncRelayCommand<BasketData>(async (item) =>
        {
            if (item == null) return;

            var product = new Product
            {
                OrderId = item.OrderId,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                TotalPrice = item.TotalPrice
            };

            await NavigationService.Navigate<PaymentShipmentView>(product);

        });

        private async Task LoadBasketCountAsync()
        {
            try
            {
                var dbItems = await _databaseManager.GetListAsync<AddToBasketData>();

                // Compute checksum of DB
                string newHash = ComputeHash(dbItems);

                // If the DB hasn't changed, skip loading
                if (newHash == _lastBasketHash)
                    return;

                _lastBasketHash = newHash;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AddToBasketData.Clear();

                    foreach (var data in dbItems)
                    {
                        AddToBasketData.Add(new BasketData
                        {
                            OrderId = data.OrderId,
                            Title = data.Title,
                            Description = data.Description,
                            Price = data.Price,
                            ProductQuantity = data.ProductQuantity,
                            DateTime = data.DateTime,
                            TotalPrice = data.TotalPrice
                        });
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"PaperProductViewModel: Error loading basket data: {ex.Message}");
            }
        }

        private string ComputeHash(IEnumerable<AddToBasketData> items)
        {
            using var sha = SHA256.Create();
            var sb = new StringBuilder();

            foreach (var x in items)
            {
                sb.Append($"{x.Title}|{x.Description}|{x.Price}|{x.ProductQuantity}|{x.DateTime.Ticks}|{x.TotalPrice};");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

    }
}