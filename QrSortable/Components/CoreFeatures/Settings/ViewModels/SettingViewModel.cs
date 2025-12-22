namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Threading.Tasks;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class SettingViewModel : BaseViewModel
    {

        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;

        private readonly IBackendCommunicationService _backendCommunicationService;

        private readonly IBackendDatabaseManager _backendDatabaseManager;

        private readonly IBackendSynchronizationManager _backendSynchronizationManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingViewModel" />.
        /// </summary>
        public SettingViewModel(IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager,
            IBackendCommunicationService backendCommunicationService, IBackendDatabaseManager backendDatabaseManager,
            IBackendSynchronizationManager backendSynchronizationManager)
        {
            IsBackNavigationEnabled = true;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _backendCommunicationService = backendCommunicationService;
            _backendDatabaseManager = backendDatabaseManager;
            _backendSynchronizationManager = backendSynchronizationManager;
        }


        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();

            //var dbEntries = await _backendDatabaseManager
            //  .GetAllAsync<DtoStorageEntryModel>();
            //dbEntries = dbEntries.OrderBy(dto => dto.ID);

            //foreach (var dto in dbEntries)
            //{
            //    var entry = dto.Category;
            //}

            //var dbEntries1 = await _backendDatabaseManager
            //  .GetAllAsync<DtoOrdersModel>();
            //dbEntries1 = dbEntries1.OrderBy(dto => dto.ID);

            //foreach (var dto in dbEntries1)
            //{
            //    var entry = dto.City;
            //}

            await _backendSynchronizationManager.SynchronizeStoredObjectsAsync();
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            //var entries = await _backendCommunicationService.GetAllAsync<StorageEntryModel>();
            //for (int i = 0; i < entries.Count; i++)
            //{
            //    var entry = entries[i];
            //}


          
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