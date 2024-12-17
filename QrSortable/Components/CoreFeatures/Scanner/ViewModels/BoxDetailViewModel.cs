namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the box view screen.
    /// </summary>
    public partial class BoxDetailViewModel : BaseViewModel<string>
    {
        private readonly IDatabaseManager _databaseManager;
        private StorageEntry _storageData = new StorageEntry();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxDetailViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public BoxDetailViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var storageArray = await _databaseManager.GetListAsync<StorageEntry>();

            if(storageArray != null)
            {
                //populate the item name and item image from already stored data from database
            }
        }

        [ObservableProperty]
        private string _barcode;

        [ObservableProperty]
        private string _barcodeType;

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _category;

        /// <summary>
        ///     Prepares the viewmode with an barcode raw data.
        /// </summary>
        /// <param name="barcode">The string barcode data.</param>
        public override async void Prepare(string barcode)
        {
            string[] result = barcode.Split(',');
            Barcode = _storageData.BarcodeValue = result[0].Trim();
            BarcodeType = _storageData.BarcodeType = result[1].Trim();

        }

        public AsyncRelayCommand AddItemCommand => new AsyncRelayCommand(async () =>
        {
            _storageData.Location = Location;
            _storageData.Category = Category;
            await NavigationService.Navigate<ItemDetailView>(_storageData, false);
        });

    }
}