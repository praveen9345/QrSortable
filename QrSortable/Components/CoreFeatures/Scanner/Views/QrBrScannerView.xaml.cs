namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the RootView view.
    /// </summary>
    public partial class QrBrScannerView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the QrBrScannerViewView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The QrBrScannerViewModel associated with this view.</param>
        public QrBrScannerView(QrBrScannerViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }

        private void CameraView_OnDetectionFinished(object sender, BarcodeScanning.OnDetectionFinishedEventArg e)
        {
            if (e.BarcodeResults.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.Dispatcher.DispatchAsync(async() => 
                    {
                        await DisplayAlert("Info:", e.BarcodeResults.Count + " ; " + $"{e.BarcodeResults.FirstOrDefault()?.DisplayValue}" + " ; " +
                            e.BarcodeResults.FirstOrDefault()?.BarcodeFormat.ToString(), "Ok");
                    });
                });

            }
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
            infoText.Margin= new Thickness(left: 0, top: height/4, right: 0, bottom: 0);

        }
    }
}