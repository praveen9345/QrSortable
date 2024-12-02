namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
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

            //await NavigationService.Navigate<BoxDetailView>();
            var sample = new SampleModel
            {
                Name = "Sample",
                Description = "Sample Description"
            };

            await _backendCommunicationService.InsertSampleModel(sample);
        });
    }  
}