namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using BarcodeScanning;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class QrBrScannerViewModel : BaseViewModel
    {
        private readonly IAesHelper _aesHelper;
        private readonly IPermissionService _permissionService;
        private readonly IToastService _toastService;

        private bool _isBarcodeDetected = false;
        public string FlashOnGlyph => "\uF41a";
        public string FlashOffGlyph => "\uF41b";

        /// <summary>
        ///     Initializes a new instance of the <see cref="QrBrScannerViewModel" />.
        /// </summary>
        public QrBrScannerViewModel(IAesHelper aesHelper, IPermissionService permissionService, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
           _aesHelper = aesHelper;
            _permissionService = permissionService;
            _toastService = toastService;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            CurrentGlyph = FlashOffGlyph;
        }

        [ObservableProperty]
        private bool _isCameraEnabled;

        [ObservableProperty]
        private string _flashLightOnOff;

        [ObservableProperty]
        public bool isFlashOn;

        [ObservableProperty]
        public string _currentGlyph;

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            _ = InitializeCameraAsync();
        }

        private async Task InitializeCameraAsync()
        {
            try
            {
                IsCameraEnabled = false;

                var status = PermissionStatus.Unknown;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var status =
                        await _permissionService.CheckPermissionStatusAsync(Permission.Camera);
                });

                if (status != PermissionStatus.Granted)
                {

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        status = await _permissionService.RequestPermissionAsync(Permission.Camera);
                    });

                }

                if (status == PermissionStatus.Granted)
                {
                    IsCameraEnabled = true;
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await _toastService.DisplayToast("Camera permission is required.");
                        await NavigationService.Close();
                    });

                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Camera init error: {ex}");
                IsCameraEnabled = false;
            }
        }

      

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            _isBarcodeDetected = false;
            IsCameraEnabled = false;
        }


        public AsyncRelayCommand<IReadOnlySet<BarcodeResult>> DetectionFinishedCommand =>
        new AsyncRelayCommand<IReadOnlySet<BarcodeResult>>(async (IReadOnlySet<BarcodeResult> result) =>
        {
            if (result.Count > 0)
            {
                string decryptedValue;
                var firstResult = result.FirstOrDefault()?.DisplayValue;
                try
                {
                    decryptedValue = _aesHelper.Decrypt(firstResult);
                }
                catch
                {
                    // TODO: Handle decryption errors by displaying a message to the user.
                    decryptedValue = "Invalid";
                }
               
                string displayValue = decryptedValue + "," + result.FirstOrDefault()?.BarcodeFormat.ToString();
                Console.WriteLine("Found the BarcodeResult: " + result.Count + " ; " + $"{displayValue}");
                if (!_isBarcodeDetected)
                {
                    _isBarcodeDetected = true;
                    await NavigationService.Navigate<BoxDetailView>(displayValue);
                }
                
            }
        });

    }
}