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
    }
}