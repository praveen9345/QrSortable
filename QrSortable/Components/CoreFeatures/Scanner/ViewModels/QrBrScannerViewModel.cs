namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using BarcodeScanning;
    using Microsoft.Maui.ApplicationModel;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.PlatformUtils.Models;
    using PermissionStatus = PlatformUtils.Models.PermissionStatus;
    using QrSortable.Components.UiFunctionality.Localization;


    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class QrBrScannerViewModel : BaseViewModel
    {
        private readonly IAesHelper _aesHelper;
        private readonly IPermissionService _permissionService;

        private bool _isBarcodeDetected = false;
        public string FlashOnGlyph => "\uF41a";
        public string FlashOffGlyph => "\uF41b";

        /// <summary>
        ///     Initializes a new instance of the <see cref="QrBrScannerViewModel" />.
        /// </summary>
        public QrBrScannerViewModel(IAesHelper aesHelper, IPermissionService permissionService)
        {
            IsBackNavigationEnabled = true;
            _aesHelper = aesHelper;
            _permissionService = permissionService;
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

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            var status = await _permissionService
                .RequestPermissionAsync(Permission.Camera);

            switch (status)
            {
                case PermissionStatus.Granted:
                    IsCameraEnabled = true;
                    break;

                case PermissionStatus.Denied:
                    await HandleDeniedPermission();
                    break;

                case PermissionStatus.Restricted:
                    await DialogService.ShowAlertDialog(
                        AppResources.QrBrScannerViewModel_CameraRestricted,
                        AppResources.Dialog_OK_Text);
                    await NavigationService.Close();
                    break;

                default:
                    await NavigationService.Close();
                    break;
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
                    await DialogService.ShowAlertDialog( AppResources.QrBrScannerViewModel_InvalidCodes,
                        AppResources.Dialog_OK_Text);
                    return;
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

        private async Task HandleDeniedPermission()
        {
            bool openSettings = await DialogService.ShowRequestDialog(
                AppResources.ItemDetailViewModel_CameraPermissionRequired,
                AppResources.Dialog_Cancel_Text, AppResources.ItemDetailViewModel_OpenSettings);

            if (openSettings)
            {
                AppInfo.ShowSettingsUI();   // Opens app settings page
            }

            await NavigationService.Close();
        }

    }
}