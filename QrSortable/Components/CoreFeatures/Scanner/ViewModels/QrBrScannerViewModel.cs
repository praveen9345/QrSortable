namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using BarcodeScanning;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class QrBrScannerViewModel : BaseViewModel
    {
        private bool _isBarcodeDetected = false;
        public string FlashOnGlyph => "\uF41a";
        public string FlashOffGlyph => "\uF41b";

        /// <summary>
        ///     Initializes a new instance of the <see cref="QrBrScannerViewModel" />.
        /// </summary>
        public QrBrScannerViewModel()
        {
            IsBackNavigationEnabled = true;
           
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
            await Methods.AskForRequiredPermissionAsync();
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
            IsCameraEnabled = true;
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
                string displayValue = result.FirstOrDefault()?.DisplayValue +"," + result.FirstOrDefault()?.BarcodeFormat.ToString();
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