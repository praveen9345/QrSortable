namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Security.Cryptography;

    /// <summary>
    ///     The view model of the box view screen.
    /// </summary>
    public partial class BoxDetailViewModel : BaseViewModel<string>
    {
        private readonly IDatabaseManager _databaseManager;

        /// <summary>
        /// Gets or sets the collection of items. This collection supports dynamic data binding and 
        /// notifies listeners of changes such as when items are added, removed, or the entire list is refreshed.
        /// </summary>
        public ObservableCollection<ItemInfo> Items { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxDetailViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public BoxDetailViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;

            Items = new ObservableCollection<ItemInfo>();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            CategoryText = "Category:"; 
        }

        /// <summary>
        /// .............................
        /// </summary>
        public async override void ViewAppearing()
        {
            base.ViewAppearing();
            var storageArray = await _databaseManager.GetListAsync<StorageEntry>();

            if (storageArray != null)
            {
                Items.Clear();
                foreach (var storageItem in storageArray)
                {
                    if (storageItem.BarcodeValue == Barcode && storageItem.BarcodeType == BarcodeType)
                    {
                        IsNewBarcode = true;
                        CategoryText = storageItem.Category;
                        LocationText = storageItem.Location;

                        var image = ImageSource.FromFile("image_icon");
                        if (storageItem.ImageList != null && storageItem.ImageList.Count > 0)
                        {
                            image = ConvertToImageSource(storageItem.ImageList[0]);
                        }

                        Items.Add(new ItemInfo
                        {
                            ItemName = storageItem.ItemName,
                            ImageSource = image,
                            FileLayoutBackgroundColor = Colors.Transparent

                        });
                    }
                }
            }
        }

        [ObservableProperty]
        private string _barcode;

        [ObservableProperty]
        private string _barcodeType;

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _locationText;

        [ObservableProperty]
        private string _category;

        [ObservableProperty] 
        private string _categoryText;

        [ObservableProperty]
        private bool _isNewBarcode;

        /// <summary>
        /// Represents the currently selected item in the application.
        /// </summary>
        [ObservableProperty]
        private ItemInfo _selectedItem;


        /// <summary>
        ///     Prepares the viewmode with an barcode raw data or string data.
        /// </summary>
        /// <param name="data">The string data.</param>
        public override async void Prepare(string data)
        {
            //if(data == "itemView")
            //{
            //    IsNewBarcode = true;
            //    CategoryText = Category;
            //    LocationText = Location;
            //}
            //else
            //{
            //    string[] result = data.Split(',');
            //    Barcode = _storageData.BarcodeValue = result[0].Trim();
            //    BarcodeType = _storageData.BarcodeType = result[1].Trim();
            //}

            string[] result = data.Split(',');
            Barcode = result[0].Trim();
            BarcodeType = result[1].Trim();
        }

        public AsyncRelayCommand AddItemCommand => new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(LocationText))
            {
                if (string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(Category))
                {
                    await DialogService.ShowAlertDialog(
                        "Location and Category fields are required and cannot be left empty.", "Ok");
                    return;
                }
            }

            // Create a new StorageEntry for the new item
            var newStorageData = new StorageEntry
            {
                Category = string.IsNullOrWhiteSpace(LocationText) ? Category : CategoryText,
                Location = string.IsNullOrWhiteSpace(LocationText) ? Location : LocationText,
                BarcodeValue = Barcode,
                BarcodeType = BarcodeType
            };

            // Navigate to ItemDetailView with the new instance
            await NavigationService.Navigate<ItemDetailView>(newStorageData);
        });


        public AsyncRelayCommand OnSelectionItemChangedCommand => new AsyncRelayCommand(async () =>
        {

            if (SelectedItem == null) return;

            var allItems = await _databaseManager.GetAllAsync<StorageEntry>();
            var itemSelectedDb = allItems?.FirstOrDefault(i => i.ItemName == SelectedItem.ItemName);

            if(itemSelectedDb != null)
            {
                await NavigationService.Navigate<ItemDetailView>(itemSelectedDb);
            }

        });


        public AsyncRelayCommand DeleatItemCommand => new AsyncRelayCommand(async () =>
        {
            var confirm = await DialogService.ShowRequestDialog(
               AppResources.Dialog_ConfirmationMessage_Text,
               AppResources.BoxDetailViewModel_DeletItemText,
               AppResources.Dialog_Cancel_Text,
              AppResources.Dialog_OK_Text);

            if (!confirm)
                return;

            var storageList = await _databaseManager.GetListAsync<StorageEntry>();
            //var entryToDelete = storageList?.FirstOrDefault(e =>
            //   e.BarcodeValue == Barcode &&
            //   e.BarcodeType == BarcodeType &&
            //   e.ItemName == item.ItemName);


        });

        private ImageSource ConvertToImageSource(object input)
        {
            if (input == null)
                return null;

            Stream stream = input switch
            {
                byte[] jpegBytes when jpegBytes.Length > 0 => new MemoryStream(jpegBytes),
                Stream jpegStream => jpegStream,
                _ => throw new ArgumentException("Unsupported input type", nameof(input))
            };

            if (stream.CanSeek)
                stream.Position = 0;

            return ImageSource.FromStream(() => stream );
        }
    }
}