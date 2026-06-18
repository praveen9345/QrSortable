namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using BarcodeScanning;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class QrBrScannerView : BaseView
    {
        private readonly QrBrScannerViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the QrBrScannerViewView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The QrBrScannerViewModel associated with this view.</param>
        public QrBrScannerView(QrBrScannerViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel =viewModel;

            var camera = new CameraView
            {
                TapToFocusEnabled = true,
                ViewfinderMode = true
            };

            camera.SetValue(CameraView.BarcodeSymbologiesProperty, "QRCode,Code128");
            camera.SetBinding(CameraView.CameraEnabledProperty, nameof(viewModel.IsCameraEnabled));
            camera.SetBinding(CameraView.TorchOnProperty, nameof(viewModel.IsFlashOn));
            camera.SetBinding(CameraView.OnDetectionFinishedCommandProperty, nameof(viewModel.DetectionFinishedCommand));

            CameraContainer.Children.Add(camera);

            cameraView = camera;

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
            infoText.Margin= new Thickness(left: 0, top: height/4, right: 0, bottom: 0);

        }

        private void OnFlashOnOffButtonClicked(object sender, EventArgs e)
        {
            _viewModel.IsFlashOn = !_viewModel.IsFlashOn;
            if (_viewModel.IsFlashOn)
            {
                _viewModel.CurrentGlyph = _viewModel.FlashOnGlyph;
            }
            else
            {
                _viewModel.CurrentGlyph = _viewModel.FlashOffGlyph;
            }
        }
    }
}