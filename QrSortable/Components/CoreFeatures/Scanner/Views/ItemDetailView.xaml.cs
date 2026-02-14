namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the ItemDetailView view.
    /// </summary>
    public partial class ItemDetailView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the ItemDetailView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The ItemDetailViewModel associated with this view.</param>
        public ItemDetailView(ItemDetailViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            cameraView.CaptureNextFrame = true;
        }
    }
}