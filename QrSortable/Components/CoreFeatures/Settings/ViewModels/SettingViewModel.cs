namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;
    using QrSortable.Components.CoreFeatures.Settings.Views;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class SettingViewModel : BaseViewModel
    {

        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;

        private readonly IBackendCommunicationService _backendCommunicationService;

        private readonly IBackendDatabaseManager _backendDatabaseManager;

        private readonly IBackendSynchronizationManager _backendSynchronizationManager;

        private readonly IToastService _toast;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingViewModel" />.
        /// </summary>
        public SettingViewModel(IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager,
            IBackendCommunicationService backendCommunicationService, IBackendDatabaseManager backendDatabaseManager,
            IBackendSynchronizationManager backendSynchronizationManager, IToastService toast)
        {
            IsBackNavigationEnabled = true;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _backendCommunicationService = backendCommunicationService;
            _backendDatabaseManager = backendDatabaseManager;
            _backendSynchronizationManager = backendSynchronizationManager;
            _toast = toast;
        }


        public AsyncRelayCommand LanguageCommand => new AsyncRelayCommand(async () =>
        {

        });

        public AsyncRelayCommand PrivacyPolicyCommand => new AsyncRelayCommand(async () =>
        {
            await NavigationService.Navigate<WebView>("privacyPolicy");
        });

        public AsyncRelayCommand TermsAndConditionsCommand => new AsyncRelayCommand(async () =>
        {
            await NavigationService.Navigate<WebView>("termsAndCondition");
        });

        public AsyncRelayCommand LicenseCommand => new AsyncRelayCommand(async () =>
        {
            await NavigationService.Navigate<WebView>("license");
        });

        public AsyncRelayCommand SyncDataCommand => new AsyncRelayCommand(async () =>
        {

            await DialogService.ShowActivityIndicatorAndReturnResult("Uploading...", async () =>
            {
                var result = await _generalDatabaseSynchronizationManager.ClearBackendAndSyncLocalDataAsync();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (result)
                    {
                        await _toast.DisplayToast("Data synchronized successfully.");
                    }
                    else
                    {
                        await _toast.DisplayToast("Data synchronization failed or no internet connection. Try again later.");
                    }
                });

                return result;
            });
        });

    }
}