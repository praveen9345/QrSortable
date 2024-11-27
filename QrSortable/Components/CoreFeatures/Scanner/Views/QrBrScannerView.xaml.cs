namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
            infoText.Margin= new Thickness(left: 0, top: height/4, right: 0, bottom: 0);

        }
    }
}