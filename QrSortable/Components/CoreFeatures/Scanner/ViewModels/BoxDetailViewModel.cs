namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the box view screen.
    /// </summary>
    public partial class BoxDetailViewModel : BaseViewModel<string>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IConnectivityService _connectivityService;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;

        private string _colorCodeHex;

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
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public BoxDetailViewModel(IDatabaseManager databaseManager, IToastService toastService,
            IBackendCommunicationService backendCommunicationService, IConnectivityService connectivityService, 
            ISharedMethodService sharedMethodService, IBackendDatabaseManager backendDatabaseManager,
            IBackendDatabaseHelper backendDatabaseHelper)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _backendCommunicationService = backendCommunicationService;
            _connectivityService = connectivityService;

            Items = new ObservableCollection<ItemInfo>();
            _sharedMethodService = sharedMethodService;
            _backendDatabaseManager = backendDatabaseManager;
            _backendDatabaseHelper = backendDatabaseHelper;
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
        /// Called when the view appears.
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

            string codeColorData = result[0].Trim();
            string[] parts = codeColorData.Split('#');

            Barcode = parts[0];  // "QR345"
            _colorCodeHex = "#" + parts[1];  // "#F54927"

            BarcodeType = result[1].Trim();
        }

        /// <summary>
        /// Command to add a new item.
        /// </summary>
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
                BarcodeType = BarcodeType,
                BackgroundColorHex = _colorCodeHex
            };

            // Navigate to ItemDetailView with the new instance
            await NavigationService.Navigate<ItemDetailView>(newStorageData);
        });

        /// <summary>
        /// Command triggered when an item selection changes.
        /// </summary>
        public AsyncRelayCommand OnSelectionItemChangedCommand => new AsyncRelayCommand(async () =>
        {

            if (SelectedItem == null) return;

            try
            {
                var allItems = await _databaseManager.GetAllAsync<StorageEntry>();
                var itemSelectedDb = allItems?.FirstOrDefault(i => i.ItemName == SelectedItem.ItemName
                                      && i.BarcodeValue == Barcode && i.BarcodeType == BarcodeType);

                if (itemSelectedDb != null)
                {
                    await NavigationService.Navigate<ItemDetailView>(itemSelectedDb);
                }
                else
                {
                    await DialogService.ShowAlertDialog("The selected item could not be found.", AppResources.Dialog_OK_Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BoxDetailViewModel.OnSelectionItemChangedCommand: Exception: {ex}");
            }

        });

        /// <summary>
        /// Command to delete the selected item.
        /// </summary>
        public AsyncRelayCommand<ItemInfo> DeleteItemCommand => new AsyncRelayCommand<ItemInfo>(async (item) =>
        {
            if (item == null) return;

            try
            {
                var confirm = await DialogService.ShowRequestDialog( AppResources.Dialog_ConfirmationMessage_Text,AppResources.BoxDetailViewModel_DeletItemText,
                    AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm)
                    return;

                // Remove the item from the database and the collection
                var storageList = await _databaseManager.GetListAsync<StorageEntry>();
                var entryToDelete = storageList?.FirstOrDefault(e =>
                    e.ItemName == item.ItemName &&
                    e.BarcodeValue == Barcode &&
                    e.BarcodeType == BarcodeType);

                if (entryToDelete != null)
                {
                    var deleteItem = await _databaseManager.DeleteAsync(entryToDelete);
                    if (deleteItem)
                    {
                        if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                        {
                            var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(entryToDelete, "delete");
                            await _backendDatabaseManager.AddAsync(dto);
                        }
                        else 
                        {
                            await _backendCommunicationService.DeleteAsync(entryToDelete);
                        }
                    }

                    var itemInCollection = Items.FirstOrDefault(i => i.ItemName == item.ItemName);
                    if (itemInCollection != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() => Items.Remove(itemInCollection));
                        await _toastService.DisplayToast("Successfully deleted.");
                    }
                        
                }
                else
                {
                    await DialogService.ShowAlertDialog("Data could not be deleted, try again later.", AppResources.Dialog_OK_Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BoxDetailViewModel.DeleteItemCommand: Exception: {ex}");
                await DialogService.ShowAlertDialog("An unexpected error occurred while deleting the item.", AppResources.Dialog_OK_Text);
            }
        });


        /// <summary>
        /// Command to move the selected item.
        /// </summary>
        public AsyncRelayCommand<ItemInfo> MoveItemCommand => new AsyncRelayCommand<ItemInfo>(async (item) =>
        {
            if (item == null) return;

            try
            {
                var storageList = await _databaseManager.GetListAsync<StorageEntry>();
                var entryTomove = storageList?.FirstOrDefault(e => e.ItemName == item.ItemName 
                && e.BarcodeValue == Barcode 
                && e.BarcodeType == BarcodeType);

                if (entryTomove != null)
                {
                    var result = await DialogService.ShowMoveToDialog(entryTomove);
                    if (!string.IsNullOrWhiteSpace(result))
                    {

                        // If the user selected the same box, just inform and return
                        if (result == entryTomove.BarcodeValue)
                        {
                            await DialogService.ShowAlertDialog("The selected item is already in the chosen box.", AppResources.Dialog_OK_Text);
                            return;
                        }

                        try
                        {
                           
                            var targetToMove = storageList?.FirstOrDefault(e => e.BarcodeValue == result);

                            if (targetToMove == null)
                            {
                                await DialogService.ShowAlertDialog("The target box could not be found.", AppResources.Dialog_OK_Text);
                                return;
                            }


                            var outcomeObj = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Moving...",
                            async () =>
                            {
                                _databaseManager.BeginTransaction();

                                var createdDate = DateTime.UtcNow;

                                var newEntry = new StorageEntry
                                {
                                    Category = targetToMove.Category,
                                    CreatedDate = createdDate,
                                    BarcodeValue = result,
                                    BarcodeType = targetToMove.BarcodeType,
                                    Location = targetToMove.Location,
                                    SearchInfo = $"{targetToMove.Category}|{createdDate}|{result}|{targetToMove.BarcodeType}|{targetToMove.Location}|{entryTomove.ItemName}|{entryTomove.Description}",
                                    ItemName = entryTomove.ItemName,
                                    Description = entryTomove.Description,
                                    ImageList = entryTomove.ImageList != null ? new List<byte[]>(entryTomove.ImageList) : null,
                                    BackgroundColorHex = targetToMove.BackgroundColorHex
                                };

                                // Add new entry
                                var added = await _databaseManager.AddAsync(newEntry);
                                if (added != null)
                                {
                                    _databaseManager.CommitTransaction();

                                    // send to backend (DtoStorageEntryModel)
                                    if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                                    {
                                        var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(newEntry, "false");

                                        _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                                    }
                                    else
                                    {
                                        await _backendCommunicationService.InsertAsync(newEntry);
                                    }
                                }
                                else
                                {
                                    _databaseManager.Rollback();
                                    return false;
                                }
                               

                                // Delete the original entry
                                var delete = await _databaseManager.DeleteAsync(entryTomove);
                                if(delete)
                                {

                                    if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                                    {
                                        var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(entryTomove, "delete");
                                        _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                                    }
                                    else
                                    {
                                        // send to backend (DtoStorageEntryModel)
                                        await _backendCommunicationService.DeleteAsync(entryTomove);
                                    }
                                    return true;
                                }

                                return false;
                            });

                            if (!outcomeObj)
                            {
                                await DialogService.ShowAlertDialog("Data could not be moved, try again later.", AppResources.Dialog_OK_Text);
                                return;
                            }
                            // Perform UI updates on main thread after dialog closed
                            var itemInCollection = Items.FirstOrDefault(i => i.ItemName == item.ItemName);
                            if (itemInCollection != null)
                            {
                                MainThread.BeginInvokeOnMainThread(() => Items.Remove(itemInCollection));
                                await _toastService.DisplayToast("Successfully moved.");
                            }
                               
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"BoxDetailViewModel.MoveItemCommand: Exception: {ex}");
                            await DialogService.ShowAlertDialog("An unexpected error occurred while moving the item.", AppResources.Dialog_OK_Text);
                        }
                    }

                }
                else
                {
                    await DialogService.ShowAlertDialog("Data could not be moved, try again later.", AppResources.Dialog_OK_Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BoxDetailViewModel.DeleteItemCommand: Exception: {ex}");
                await DialogService.ShowAlertDialog("An unexpected error occurred while moving the item.", AppResources.Dialog_OK_Text);
            }
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