namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Models;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
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
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;
        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynManager;

        private string _urlImage = "image_icon";
        private bool _isEnabelStatusOfOrder = false;

        public ObservableCollection<OrderedData> OrderedDatas { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoursOrdersViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public YoursOrdersViewModel(IDatabaseManager databaseManager, IToastService toastService,
            IGeneralDatabaseSynchronizationManager generalDatabaseSynManager, IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _generalDatabaseSynManager = generalDatabaseSynManager;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;

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

            if (!_mauiEssentialsWrapper.IsInternetConnectionAvailable())
            {
                await _toastService.DisplayToast(AppResources.YoursOrdersViewModel_SyncData);
                return;
            }   
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            bool syncResult = (bool)await DialogService.ShowActivityIndicatorAndReturnResult(
                AppResources.Dialog_Processing,
                async () =>
                {
                    await _generalDatabaseSynManager.SyncYourOrdersFromFirebaseAsync();
                    await LoadOrderedDataAsync();
                    return true;
                }
            );

        }

        /// <summary>
        /// Represents the currently add to basket countin the application.
        /// </summary>
        [ObservableProperty]
        private string _basketCount;


        public AsyncRelayCommand<OrderedData> CopyOrderIdCommand => new AsyncRelayCommand<OrderedData>(async (item) =>
        {
            if (item == null) return;
            await Clipboard.Default.SetTextAsync(item.OrderId);
            await _toastService.DisplayToast(AppResources.YoursOrdersViewModel_CopyToClipboard);
        });

        public AsyncRelayCommand<OrderedData> DownloadPDFCommand => new AsyncRelayCommand<OrderedData>(async (item) =>
        {
            if (item == null) return;
            try
            {
                var dbItems = await _databaseManager.GetListAsync<YoursOrderData>();
                var existing = dbItems.FirstOrDefault(x =>
                    x.Title == item.Title &&
                    x.OrderId == item.OrderId
                );
                if (existing != null)
                {

                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = (existing.CodeType == "QRcode") ? $"{timestamp}Your_QR_Codes.pdf"
                        : $"{timestamp}Your_Barcodes.pdf";

                    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    File.WriteAllBytes(filePath, existing.PdfFiles[0]);

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            Title = fileName,
                            File = new ShareFile(filePath)
                        });
                    });
                }

                }
            catch (Exception ex)
            {
                Console.WriteLine($"DatabaseAndBackendStoringAsync::Exception during add: {ex}");
            }

        });


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
                        CodeType = item.CodeType,
                        PageType = item.PageType,
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
            if (title.Contains("SQR") && AppResources.SelectProductViewModel_StandardPack48QRTitle.Contains("SQR"))
            {
                _urlImage = "qr_standerd_pack_1.png";
                _isEnabelStatusOfOrder = false;
            }
            else if (title.Contains("SBR") && AppResources.SelectProductViewModel_StandardPack48BrTitle.Contains("SBR"))
            {
                _urlImage = "br_standerd_pack_1.png";
                _isEnabelStatusOfOrder = false;
            }
            else if (title.Contains("LQR") && AppResources.SelectProductViewModel_LargePack100QRTitle.Contains("LQR"))
            {
                _urlImage = "qr_large_pack_1.png";
                _isEnabelStatusOfOrder = false;
            }  
            else if (title.Contains("GQB") && AppResources.SelectProductViewModel_GenrateOfA4QRcodeTitle.Contains("GQB"))
            {
                _urlImage = "code_pdf_icon.png";
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
                        ? basketData.Count.ToString() : "0";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PaperProductViewModel: Error loading basket data: {ex.Message}");
            }
        }

    }
}