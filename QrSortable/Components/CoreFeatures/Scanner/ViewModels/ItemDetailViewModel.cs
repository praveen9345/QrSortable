namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.Maui.Graphics;
    using Microsoft.Maui.Graphics.Platform;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Models;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using QrSortable.Components.UiFunctionality.Notification.Models;
    using System.Collections.ObjectModel;


    /// <summary>
    ///     The view model of the ItemDetailView screen.
    /// </summary>
    public partial class ItemDetailViewModel : BaseViewModel<StorageEntry>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IFilePickerService _filePickerService;
        private readonly IImageService _imageService;
        private readonly IToastService _toastService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IConnectivityService _connectivityService;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;
        private readonly IPermissionService _permissionService;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        private List<byte[]> _imageArrayDb = new List<byte[]>();
        private StorageEntry _storageData;
        private bool _isUpdateItem;
        private Guid _storageUpdateItemId;

        /// <summary>
        /// A collection of images associated with the storage entry.
        /// </summary>
        public ObservableCollection<Images> ImageArray { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemDetailViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public ItemDetailViewModel(IDatabaseManager databaseManager, IImageService imageService, 
            IFilePickerService filePickerService, IToastService toastService, IBackendCommunicationService backendCommunicationService,
            IBackendDatabaseManager backendDatabaseManager, IConnectivityService connectivityService, ISharedMethodService sharedMethodService, 
            IBackendDatabaseHelper backendDatabaseHelper, IPermissionService permissionService, IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _imageService = imageService;
            _filePickerService = filePickerService;
            _toastService = toastService;
            _backendCommunicationService = backendCommunicationService;
            _backendDatabaseManager = backendDatabaseManager;
            _connectivityService = connectivityService;
            _sharedMethodService = sharedMethodService;
            _backendDatabaseHelper = backendDatabaseHelper;
            _permissionService = permissionService;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;

            ImageArray = new ObservableCollection<Images>();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            // CAMERA PERMISSION
            var cameraStatus = await _permissionService.CheckPermissionStatusAsync(Permission.Camera);
            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await _permissionService.RequestPermissionAsync(Permission.Camera);
                if (cameraStatus == PermissionStatus.Denied || cameraStatus == PermissionStatus.Restricted)
                {
                    await HandleDeniedPermission();
                    IsCameraEnabled = false;
                    return;
                }
            }

            // NOTIFICATION PERMISSION (optional)
            //var notificationStatus = await _permissionService.CheckPermissionStatusAsync(Permission.Notification);
            //if (notificationStatus != PermissionStatus.Granted)
            //{
            //    await _permissionService.RequestPermissionAsync(Permission.Notification);
            //}

            IsCameraEnabled = true;
        }

        [ObservableProperty]
        private string _itemName;

        [ObservableProperty]
        private string _itemDescription;

        [ObservableProperty]
        private bool _isCameraEnabled;

        [ObservableProperty]
        private bool _isCameraCapture;

        [ObservableProperty]
        private bool _isCameraCaptureVisable;


        /// <summary>
        ///     Prepares the viewmode with an storage raw data.
        /// </summary>
        /// <param name="storageData">The string storage data.</param>
        public override void Prepare(StorageEntry storageData)
        {
            _storageData = storageData;

            if (_storageData == null)
            {
                Console.WriteLine("Error: ItemDetailViewModel.Prepare: storageData is null");
                return;
            }

            if(!string.IsNullOrWhiteSpace(_storageData.ItemName) && !string.IsNullOrWhiteSpace(_storageData.Description))
            {
                _isUpdateItem = true;
                ItemName = _storageData.ItemName;
                ItemDescription = _storageData.Description;
                _storageUpdateItemId = _storageData.StorageId;

                if (_storageData.ImageList != null && _storageData.ImageList.Count > 0)
                {
                    foreach (var imageBytes in _storageData.ImageList)
                    {
                        _imageArrayDb.Add(imageBytes);
                        ImageArray.Add(new Images()
                        {
                            Image = ConvertToImageSource(imageBytes),
                            Rotate = 0
                        });
                    }
                }
            }
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            IsCameraEnabled = true;
            IsCameraCapture = false;
            IsCameraCaptureVisable = false;
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            IsCameraEnabled = false;
            IsCameraCapture = false;
            IsCameraCaptureVisable = false;
        }

        public AsyncRelayCommand CameraCommand => new AsyncRelayCommand(async () =>
        {
            if (!IsCameraEnabled)
            {
                await DialogService.ShowAlertDialog(
                    "Camera permission is required to capture images.",
                    AppResources.Dialog_OK_Text
                );
                return;
            }

            IsCameraCapture = true;
            IsCameraCaptureVisable = true;
        });

        public AsyncRelayCommand<object> ImageCapturedCommand => new AsyncRelayCommand<object>(async (image) =>
        {
            if (!IsCameraEnabled) return;

            IsCameraCapture = false;
            IsCameraCaptureVisable = false;

            await Task.Delay(200); // allow AVCaptureSession to close

            if (image is PlatformImage platformImage)
            {
                byte[] jpegCaptureImages = Array.Empty<byte>();
                var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
                 async () =>
                 {
                     jpegCaptureImages = await _imageService.PlatformImageConvertAsync(platformImage);

                     return true;
                 });

                if (!result || jpegCaptureImages.Length == 0)
                {
                    await DialogService.ShowAlertDialog("Could not able to capture the image", AppResources.Dialog_OK_Text);
                }
                else
                {
                    // 1. Compress the camera capture first
                    jpegCaptureImages = CompressAndResizeImage(jpegCaptureImages);

                    // 2. Check total 1MB limit
                    if (await IsWithinSizeLimit(jpegCaptureImages))
                    {
                        _imageArrayDb.Add(jpegCaptureImages);
                        MainThread.BeginInvokeOnMainThread(() => {
                            ImageArray.Add(new Images()
                            {
                                Image = ConvertToImageSource(jpegCaptureImages),
                                Rotate = 90
                            });
                        });
                       
                    }
                }

            }
            else
            {
                Console.WriteLine("ItemDetailViewModel: Invalid image data. Expected a PlatformImage.");
            }
        });

        public AsyncRelayCommand AddItemImagesCommand => new AsyncRelayCommand(async () =>
        {

            if (_imageArrayDb != null && _imageArrayDb.Count >= 4)
            {
                await DialogService.ShowAlertDialog(
                    "You can add a maximum of 4 images only.",
                    AppResources.Dialog_OK_Text
                );
                return;
            }

            var picture = (int)await DialogService.ShowPhotoSelectionDialog();

            if(picture == (int)PhotoSelectionResponse.Camera)
            {

                var cameraPermission = await _permissionService.RequestPermissionAsync(Permission.Camera);
                if (cameraPermission != PermissionStatus.Granted)
                {
                    await HandleDeniedPermission();
                    return;
                }

                IsCameraEnabled = IsCameraCaptureVisable = true;
            }

            if (picture == (int)PhotoSelectionResponse.Gallery)
            {

                Stream photo = await _filePickerService.ImageAsync();

                if(photo != null)
                {
                    var byteImage = await _imageService.ConvertToJpegBytes(photo);

                    if (byteImage != null && byteImage.Length > 0)
                    {
                        byteImage = CompressAndResizeImage(byteImage);

                        if (await IsWithinSizeLimit(byteImage))
                        {
                            _imageArrayDb.Add(byteImage);
                            ImageArray.Add(new Images()
                            {
                                Image = ConvertToImageSource(byteImage),
                                Rotate = 0
                            });
                        }
                    }
                }
                else
                {
                    await DialogService.ShowAlertDialog("Could not able to pick the image", "Ok");
                }    
            }
        });

        public AsyncRelayCommand SaveCommand => new AsyncRelayCommand(async () =>
        {
            // ===========================
            // VALIDATION
            // ===========================
            if (string.IsNullOrWhiteSpace(ItemName) || string.IsNullOrWhiteSpace(ItemDescription))
            {
                await DialogService.ShowAlertDialog(
                    "ItemName and ItemDescription fields are required and cannot be left empty.",
                    AppResources.Dialog_OK_Text
                );
                return;
            }

            // Require at least 1 image
            if (_imageArrayDb == null || _imageArrayDb.Count < 1)
            {
                await DialogService.ShowAlertDialog(
                    "Please add at least one image before saving the item.",
                    AppResources.Dialog_OK_Text
                );
                return;
            }

            try
            {
                var allItems = await _databaseManager.GetAllAsync<StorageEntry>();
                if (allItems == null)
                {
                    await DialogService.ShowAlertDialog("Could not retrieve items from the database.", AppResources.Dialog_OK_Text);
                    return;
                }

                // ===========================
                // UPDATE EXISTING ITEM
                // ===========================
                if (_isUpdateItem)
                {
                    var item = allItems.FirstOrDefault(i => i.StorageId == _storageUpdateItemId);
                    if (item == null)
                    {
                        Console.WriteLine("Error: SaveCommand: Could not find the item to update.");
                        await DialogService.ShowAlertDialog("Item not found. Please refresh and try again.", AppResources.Dialog_OK_Text);
                        return;
                    }

                    // Prevent changing the item name
                    if (item.ItemName != ItemName)
                    {
                        await DialogService.ShowAlertDialog(
                            $"Item name '{ItemName}' cannot be modified. Please try again.",
                            AppResources.Dialog_OK_Text
                        );
                        ItemName = item.ItemName;
                        return;
                    }

                    try
                    {
                        // Prepare updated fields
                        item.Description = ItemDescription;
                        item.ImageList = _imageArrayDb.ToList();
                        item.SearchInfo =
                            $"{item.Category}|{item.CreatedDate}|{item.BarcodeValue}|{item.BarcodeType}|{item.Location}|{ItemName}|{ItemDescription}";

                        // Perform update (no explicit transaction needed)
                        var updatedItem = await _databaseManager.UpdateAsync(item);
                        if (updatedItem != null)
                        {
                            if(!await _connectivityService.CheckInternetConnectionAvailableAsync())
                            {
                                var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(item, "true");
                                await _backendDatabaseManager.UpdateAsync(dto);
                            }
                            else
                            {
                                await _backendCommunicationService.UpdateAsync(item);
                            }

                            await _toastService.DisplayToast("Successfully updated.");
                            await NavigationService.Close();
                        }
                        else
                        {
                            await DialogService.ShowAlertDialog("Data could not be updated, try again later.", AppResources.Dialog_OK_Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SaveCommand: Exception during update: {ex}");
                        await DialogService.ShowAlertDialog(
                            "An unexpected error occurred while updating the item.",
                            AppResources.Dialog_OK_Text
                        );
                    }

                    return;
                }

                // ===========================
                // ADD NEW ITEM
                // ===========================
                if (_storageData == null)
                {
                    Console.WriteLine("Error: SaveCommand: _storageData is null");
                    await DialogService.ShowAlertDialog(
                        "Unexpected error: storage data is missing.",
                        AppResources.Dialog_OK_Text
                    );
                    return;
                }

                // Check for duplicate item name
                var isDuplicate = allItems.Any(i => i.ItemName == ItemName);
                if (isDuplicate)
                {
                    await DialogService.ShowAlertDialog(
                        $"An item with the name '{ItemName}' already exists. Please choose a different name.",
                        AppResources.Dialog_OK_Text
                    );
                    ItemName = string.Empty;
                    return;
                }

                try
                {
                    _databaseManager.BeginTransaction();

                    _storageData.CreatedDate = DateTime.Now;
                    _storageData.Category = _storageData.Category.Trim().ToUpperInvariant();
                    _storageData.ItemName = ItemName;
                    _storageData.Description = ItemDescription;
                    _storageData.ImageList = _imageArrayDb.ToList();
                    _storageData.SearchInfo =
                        $"{_storageData.Category}|{_storageData.CreatedDate}|{_storageData.BarcodeValue}|{_storageData.BarcodeType}|{_storageData.Location}|{ItemName}|{ItemDescription}";

                    var addedItem = await _databaseManager.AddAsync(_storageData);

                    if (addedItem != null)
                    {
                        _databaseManager.CommitTransaction();

                        if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                        {
                            var dto = _backendDatabaseHelper.CreateDtoStorageEntryBackendData(addedItem, "false");
                            _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                        }
                        else
                        {
                            // send to backend (DtoStorageEntryModel)
                            await _backendCommunicationService.InsertAsync(_storageData);
                        }
                        await _toastService.DisplayToast("Successfully saved.");
                        await NavigationService.Close();

                    }
                    else
                    {
                        _databaseManager.Rollback();
                        Console.WriteLine("SaveCommand: AddAsync returned null - rollback performed.");
                        await DialogService.ShowAlertDialog("Data could not be saved, try again later.", AppResources.Dialog_OK_Text);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"SaveCommand: Exception during add: {ex}");
                    await DialogService.ShowAlertDialog(
                        "An unexpected error occurred while saving the item. Please try again.",
                        AppResources.Dialog_OK_Text
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveCommand: Unexpected exception: {ex}");
                await DialogService.ShowAlertDialog(
                    "An unexpected error occurred while processing your request.",
                    AppResources.Dialog_OK_Text
                );
            }
        });

        private ImageSource ConvertToImageSource(object input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            Stream stream = input switch
            {
                byte[] jpegBytes when jpegBytes.Length > 0 => new MemoryStream(jpegBytes),
                Stream jpegStream => jpegStream,
                _ => throw new ArgumentException("Unsupported input type", nameof(input))
            };

            if (stream.CanSeek)
                stream.Position = 0;

            return ImageSource.FromStream(() => stream ?? throw new InvalidOperationException("Stream cannot be null"));
        }

        public static byte[] CompressAndResizeImage(byte[] imageBytes)
        {
            int maxBytes = 524288; // 0.5 MB
            if (imageBytes.Length <= maxBytes) return imageBytes;

            // 1. Load the image using the platform-native engine
            IImage image = PlatformImage.FromStream(new MemoryStream(imageBytes));

            float quality = 0.9f;
            byte[] result = imageBytes;
            float scale = 0.9f;

            // 2. Loop to reduce size (scaling and compression)
            while (result.Length > maxBytes && (quality > 0.2f || scale > 0.1f))
            {
                // Resize the image natively
                var newWidth = (int)(image.Width * scale);
                var newHeight = (int)(image.Height * scale);

                using var resizedImage = image.Resize(newWidth, newHeight, ResizeMode.Fit);

                using (var ms = new MemoryStream())
                {
                    // Save as JPEG with the specified quality
                    resizedImage.Save(ms, ImageFormat.Jpeg, quality);
                    result = ms.ToArray();
                }

                // Gradually drop scale and quality
                quality -= 0.1f;
                scale -= 0.1f;
            }

            return result;
        }

        private async Task<bool> IsWithinSizeLimit(byte[] newImage)
        {
            long currentTotalSize = _imageArrayDb.Sum(x => (long)x.Length);
            long maxAllowedSize = 1024 * 1024; // 1MB

            if (currentTotalSize + newImage.Length > maxAllowedSize)
            {
                await DialogService.ShowAlertDialog(
                    "Adding this image would exceed the 1MB total limit for this item.",
                    "Ok"
                );
                return false;
            }
            return true;
        }

        private async Task HandleDeniedPermission()
        {
            bool openSettings = await DialogService.ShowRequestDialog(
                "Camera permission is required", "Please enable it in Settings.",
                "Cancel",
                "Open Settings");

            if (openSettings)
            {
                AppInfo.ShowSettingsUI();   // Opens app settings page
            }

        }
    }
}