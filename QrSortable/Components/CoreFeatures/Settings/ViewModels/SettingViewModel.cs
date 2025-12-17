namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Threading.Tasks;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class SettingViewModel : BaseViewModel
    {

        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;

        private readonly IBackendCommunicationService _backendCommunicationService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingViewModel" />.
        /// </summary>
        public SettingViewModel(IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager, IBackendCommunicationService backendCommunicationService)
        {
            IsBackNavigationEnabled = true;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _backendCommunicationService = backendCommunicationService;
        }


        public override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            var entries = await _backendCommunicationService.GetAllAsync<DtoStorageEntryModel>();
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
            }
        }

        public AsyncRelayCommand SyncDataCommand => new AsyncRelayCommand(async () =>
        {

            await DialogService.ShowActivityIndicatorAndReturnResult("Uploading...", async () =>
            {
                var result = await _generalDatabaseSynchronizationManager.UploadAllAsync();
                return result;
            });
        });

    }
}