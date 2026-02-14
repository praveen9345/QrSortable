namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the ItemDetailView view.
    /// </summary>
    public partial class ItemDetailView : BaseView
    {
        private readonly ItemDetailViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the ItemDetailView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The ItemDetailViewModel associated with this view.</param>
        public ItemDetailView(ItemDetailViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
        }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.IsCameraEnabled = false;
        _viewModel.IsCameraCapture = false; 
        _viewModel.IsCameraCaptureVisable = false;

#if IOS
        // 🔥 HARD STOP FOR iOS
        if (cameraView?.Handler != null)
        {
            cameraView.Handler.DisconnectHandler();
        }
#endif
    }

        private void Button_Clicked(object sender, EventArgs e)
        {
            cameraView.CaptureNextFrame = true;
        }
    }
}