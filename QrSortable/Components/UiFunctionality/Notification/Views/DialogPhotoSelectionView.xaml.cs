namespace QrSortable.Components.UiFunctionality.Notification.Views
{
    using Microsoft.Maui.Controls.PlatformConfiguration;
    using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification.Models;
    using QrSortable.Components.UiFunctionality.Notification.ViewModels;

    /// <summary>
    ///     The code behind of the Dialog Photo Selection View.
    /// </summary>
    public partial class DialogPhotoSelectionView : BaseView
    {
        private readonly DialogPhotoSelectionViewModel _viewModel;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="DialogPhotoSelectionView"/> class.
        /// </summary>
        /// <param name="viewModel">The DialogPhotoSelectionViewModel associated with this view.</param>
        public DialogPhotoSelectionView(DialogPhotoSelectionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            boaderLayout.WidthRequest = width-100;
            boaderLayout.Margin = new Thickness(0, ((height/2)-40), 0, 0);
        }

        private void OnLayoutTapped(object sender, EventArgs e)
        {
            _viewModel.NavigationService.CloseDialog(PhotoSelectionResponse.Cancelled);
        }
    }
}