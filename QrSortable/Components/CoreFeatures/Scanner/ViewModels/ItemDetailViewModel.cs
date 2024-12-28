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
    using QrSortable.Components.UiFunctionality.Notification.Models;


    /// <summary>
    ///     The view model of the ItemDetailView screen.
    /// </summary>
    public partial class ItemDetailViewModel : BaseViewModel<StorageEntry>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IFilePickerService _filePickerService;
        private readonly IImageService _imageService;
        
        private StorageEntry _storageData;
        public ObservableCollection<Images> ImageArray { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemDetailViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public ItemDetailViewModel(IDatabaseManager databaseManager, IImageService imageService, IFilePickerService filePickerService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _imageService = imageService;
            _filePickerService = filePickerService;
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
        public override async void Prepare(StorageEntry storageData)
        {
            _storageData = storageData;

            if (_storageData != null) 
            { 
                //TODO: furhter implementaion
            }
            else
            {
                Console.WriteLine("Error:ItemDetailViewModel:Prepare: storageData is null");
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
        }

        public AsyncRelayCommand CameraCommand => new AsyncRelayCommand(async () =>
        {
            IsCameraCapture = true;
        });

        public AsyncRelayCommand<object> ImageCapturedCommand => new AsyncRelayCommand<object>(async (image) =>
        {
            if (image is PlatformImage platformImage)
            {
                byte[] jpegCaptureImages= new byte[0];
                var result = (bool) await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
                 async () =>
                 {
                     jpegCaptureImages = await _imageService.PlatformImageConvertAsync(platformImage);
                     
                     return true;
                 });

                if (result == false) 
                { 
                    await DialogService.ShowAlertDialog("Could not able to capture the image", "Ok");
                }
                else
                {
                    if (jpegCaptureImages.Length != 0)
                    {
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

            if(picture == (int)PhotoSelectionResponse.Gallery)
            {
                var imageStream = await _filePickerService.ImageAsync();
                
                if(imageStream != null)
                {
                    ImageArray.Add(new Images()
                    {
                        Image = ImageSource.FromStream(() => imageStream),
                        Rotate = 0
                    });
                }
                else
                {
                    Console.WriteLine("ItemDetailViewModel: Image not picked..");
                }
            }
        });

        public AsyncRelayCommand SaveCommand => new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(ItemName) || string.IsNullOrWhiteSpace(ItemDescription))
            {
                Console.WriteLine("Error:ItemDetailViewModel:AddItemCommand: ItemName & ItemDescription are null or empty");
                return;
            }

            if (_storageData != null)
            {
                _storageData.CreatedDate = DateTime.Now;
                _storageData.SearchInfo = _storageData.Category + "|" + _storageData.CreatedDate + "|" +
                                          _storageData.BarcodeValue + "|" + _storageData.BarcodeType + "|" +
                                          _storageData.Location + "|" + ItemName +"|"+ ItemDescription;
                
                //_storageData.Items.Add(new Item
                //    {
                //        ItemName = ItemName,
                //        Description = ItemDescription,
                //        ImagesFilePath = ItemImagesFilePath.ToList()
                //    });

                //_databaseManager.BeginTransaction();

                //var wasAddingItemSuccessful = await _databaseManager.AddAsync(_storageData);

                //if (wasAddingItemSuccessful != null)
                //{
                //    _databaseManager.CommitTransaction();
                //    Console.WriteLine("Error:ItemDetailViewModel:SaveCommand: storageData is Saved");
                //}
                //else
                //{
                //    _databaseManager.Rollback();
                //}
            }
            else
            {
                Console.WriteLine("Error:ItemDetailViewModel:SaveCommand: storageData is null");
            }
        });

         private ImageSource ConvertToImageSource(byte[] jpegBytes)
         {
            if (jpegBytes == null || jpegBytes.Length == 0)
                return null;

            // Convert byte array to a MemoryStream
            var stream = new MemoryStream(jpegBytes);

            // Create a StreamImageSource from the MemoryStream
            return ImageSource.FromStream(() => stream);
         }


    }
}