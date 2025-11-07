namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using System.Collections.ObjectModel;
    using BarcodeScanning;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.Maui.Graphics.Platform;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using QrSortable.Components.UiFunctionality.Notification.Models;


    /// <summary>
    ///     The view model of the ItemDetailView screen.
    /// </summary>
    public partial class ItemDetailViewModel : BaseViewModel<StorageEntry>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IFilePickerService _filePickerService;
        private readonly IImageService _imageService;
        private readonly IToastService _toastService;
        
        private List<byte[]> _imageArrayDb = new List<byte[]>();
        private StorageEntry _storageData;

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
        public ItemDetailViewModel(IDatabaseManager databaseManager, IImageService imageService, IFilePickerService filePickerService, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _imageService = imageService;
            _filePickerService = filePickerService;
            _toastService = toastService;
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
            await Methods.AskForRequiredPermissionAsync();
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

            // TODO: Implement logic to load existing data
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
        }

        public AsyncRelayCommand CameraCommand => new AsyncRelayCommand(async () =>
        {
            IsCameraCapture = true;
        });

        public AsyncRelayCommand<object> ImageCapturedCommand => new AsyncRelayCommand<object>(async (image) =>
        {
            if (image is PlatformImage platformImage)
            {
                byte[] jpegCaptureImages= Array.Empty<byte>();
                var result = (bool) await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
                 async () =>
                 {
                     jpegCaptureImages = await _imageService.PlatformImageConvertAsync(platformImage);
                     
                     return true;
                 });

                if (!result || jpegCaptureImages.Length == 0) 
                { 
                    await DialogService.ShowAlertDialog("Could not able to capture the image", "Ok");
                }
                else
                {
                    if (jpegCaptureImages.Length != 0)
                    {
                        _imageArrayDb.Add(jpegCaptureImages);
                        ImageArray.Add(new Images()
                        {
                            Image = ConvertToImageSource(jpegCaptureImages),
                            Rotate = 90
                        });
                    }
                }

            }
            else
            {
                Console.WriteLine("ItemDetailViewModel: Invalid image data. Expected a PlatformImage.");
            }
            IsCameraCapture = IsCameraCaptureVisable = false;
        });

        public AsyncRelayCommand AddItemImagesCommand => new AsyncRelayCommand(async () =>
        {
            var picture = (int)await DialogService.ShowPhotoSelectionDialog();

            if(picture == (int)PhotoSelectionResponse.Camera)
            {
                IsCameraEnabled = IsCameraCaptureVisable = true;
            }

            if (picture == (int)PhotoSelectionResponse.Gallery)
            {
                Stream imageStream = null;
                var result = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
                async () =>
                {
                    imageStream = await _filePickerService.ImageAsync();
                    return true;
                });

                if (!result || imageStream == null)
                {
                    await DialogService.ShowAlertDialog("Could not able to pick the image", "Ok");
                }
                else
                {
                    var byteImage = await _imageService.ConvertToJpegBytes(imageStream);
                    if (byteImage != null && byteImage.Length > 0)
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
        });

        public AsyncRelayCommand SaveCommand => new AsyncRelayCommand(async () =>
        {

            if (string.IsNullOrWhiteSpace(ItemName) || string.IsNullOrWhiteSpace(ItemDescription))
            {
                await DialogService.ShowAlertDialog("ItemName and ItemDescription fields are required and cannot be left empty.", "Ok");
                return;
            }

            if (_storageData != null)
            {
                _storageData.CreatedDate = DateTime.Now;
             
                _storageData.SearchInfo = $"{_storageData.Category}|{_storageData.CreatedDate}|{_storageData.BarcodeValue}|" +
                                            $"{_storageData.BarcodeType}|{_storageData.Location}|{ItemName}|{ItemDescription}";

                _storageData.ItemName = ItemName;
                _storageData.Description = ItemDescription;
                _storageData.ImageList = _imageArrayDb.ToList();

                try
                {
                    _databaseManager.BeginTransaction();

                    var wasAddingItemSuccessful = await _databaseManager.AddAsync(_storageData);

                    if (wasAddingItemSuccessful!= null)    
                    {
                        _databaseManager.CommitTransaction();
                        await _toastService.DisplayToast("Successfully saved.");
                        await NavigationService.Close();
                    }
                    else
                    {
                        // add server-side diagnostics log
                        _databaseManager.Rollback();
                        Console.WriteLine("ItemDetailViewModel: SaveCommand: AddAsync returned null - rollback performed.");
                        await DialogService.ShowAlertDialog("Data could not be saved, try again later.", "Ok");
                        
                       

                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        _databaseManager.Rollback();
                    }
                    catch
                    {
                        // ignore rollback errors but log
                        Console.WriteLine("ItemDetailViewModel: SaveCommand: Rollback failed.");
                    }

                    Console.WriteLine($"ItemDetailViewModel: SaveCommand: Exception while saving: {ex}");
                    await DialogService.ShowAlertDialog("An unexpected error occurred while saving the item. Please try again.", "Ok");
                }


            }
            else
            {
                Console.WriteLine("Error:ItemDetailViewModel:SaveCommand: storageData is null");
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
    }
}