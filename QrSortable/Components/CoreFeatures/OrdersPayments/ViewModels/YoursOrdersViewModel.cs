namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the YoursOrdersViewModel screen.
    /// </summary>
    public partial class YoursOrdersViewModel : BaseViewModel
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;

        private string _urlImage = "image_icon";
        private bool _isEnabelStatusOfOrder = false;

        public ObservableCollection<OrderedData> OrderedDatas { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoursOrdersViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public YoursOrdersViewModel(IDatabaseManager databaseManager, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;

            OrderedDatas = new ObservableCollection<OrderedData>();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();
            await LoadBasketCountAsync();
            await LoadOrderedDataAsync();
        }

        /// <summary>
        /// Represents the currently add to basket countin the application.
        /// </summary>
        [ObservableProperty]
        private string _basketCount;

        private async Task LoadOrderedDataAsync()
        {
            try
            {
                
                var dbItems = await _databaseManager.GetListAsync<YoursOrderData>();

                OrderedDatas.Clear();
                foreach (var item in dbItems)
                {
                    ImageUrlForTitle(item.Title);

                    var orderedData = new OrderedData
                    {
                        OrderId = item.OrderId,
                        OrderDateTime = item.DateTime,       
                        Title = item.Title,
                        Description = item.Description,
                        ImageUrl = _urlImage,
                        StatusOfOrder = item.StatusOfOrder,
                        IsEnabelStatusOfOrder = _isEnabelStatusOfOrder,
                        ShipmentTracking = item.ShipmentTracking,
                        TotalPrice = item.TotalPrice
                    };
                    OrderedDatas.Add(orderedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YoursOrdersViewModel: LoadOrderedDataAsync: Error loading ordered data from database: {ex.Message}");
            }
        }

        //TODO: need to adjust with real images
        private void ImageUrlForTitle(string title)
        {
            if (title == "Set of 5 A4 QR code")
            {
                _urlImage = "code_pdf_icon";
                _isEnabelStatusOfOrder= false;
            }
            else if (title == "Set of 5 A4 Bar code")
            {
                _urlImage = "code_pdf_icon";
                _isEnabelStatusOfOrder = false;
            }
            else if (title == "Generate A4 QR or bar code yourself!")
            {
                _urlImage = "code_pdf_icon";
                _isEnabelStatusOfOrder = true;
            }
        }

        private async Task LoadBasketCountAsync()
        {
            try
            {
                var basketData = await _databaseManager.GetListAsync<AddToBasketData>();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BasketCount = basketData != null && basketData.Count > 0
                        ? basketData.Count.ToString()
                        : "0";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PaperProductViewModel: Error loading basket data: {ex.Message}");
            }
        }

    }
}