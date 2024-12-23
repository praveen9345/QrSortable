namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class RootViewModel : BaseViewModel
    {
        private readonly IBackendCommunicationService _backendCommunicationService;
        /// <summary>
        ///     Initializes a new instance of the <see cref="RootViewModel" />.
        /// </summary>
        public RootViewModel(IBackendCommunicationService backendCommunicationService)
        {
            IsBackNavigationEnabled = true;
            _backendCommunicationService = backendCommunicationService;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

        }

        public AsyncRelayCommand QrScanCommand => new AsyncRelayCommand(async () =>
        {
            //await NavigationService.Navigate<QrBrScannerView>();

            //var wasCodeSuccessfullyUpdated = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
            //   async () =>
            //       {
            //           await Task.Delay(1000);
            //           return true;
            //       });

            //await DialogService.ShowAlertDialog("error",
            //        "Something went wrong", "lökjyajflj");

            //var isDialogConfirmed = await DialogService.ShowRequestDialog(
            //   "lksajdlk",
            //   "ökdsf",
            //   "Cancel",
            //  "OK");

            //await NavigationService.Navigate<BoxDetailView>();
            //var sample = new SampleModel
            //{
            //    Name = "kumar",
            //    Description = "Sample Description"
            //};

            //await _backendCommunicationService.InsertSampleModel(sample);

            //var userId = await _backendCommunicationService.GetUserUIDByName("Sample");
        });

       
    }  
}