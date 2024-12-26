namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using Models;
    using Navigation.ViewModels;

    /// <summary>
    ///     The view model of the confidence dialog.
    /// </summary>
    public class DialogPhotoSelectionViewModel : BaseViewModelResult<PhotoSelectionResponse>
    {
    
        /// <summary>
        ///     Initializes an instance of the <see cref="DialogPhotoSelectionViewModel" /> class.
        /// </summary>
        public DialogPhotoSelectionViewModel()
        {
            IsBackNavigationEnabled = false;
        }

        public AsyncRelayCommand CapturePhotoCommand => new AsyncRelayCommand(async () =>
        {
            NavigationService.CloseDialog(PhotoSelectionResponse.Camera);
        });

        public AsyncRelayCommand GalleryCommand => new AsyncRelayCommand(async () =>
        {
            NavigationService.CloseDialog(PhotoSelectionResponse.Gallery);
        });

    }
}