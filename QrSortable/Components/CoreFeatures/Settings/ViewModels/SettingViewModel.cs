namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Onboarding.Views;
    using QrSortable.Components.CoreFeatures.Settings.Views;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class SettingViewModel : BaseViewModel
    {

        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;

        private readonly IToastService _toast;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingViewModel" />.
        /// </summary>
        public SettingViewModel(IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager, IToastService toast)
        {
            IsBackNavigationEnabled = true;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _toast = toast;
        }


        [ObservableProperty]
        public string _appVersion;

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            AppVersion = AppInfo.VersionString;
        }

        public AsyncRelayCommand LanguageCommand => new AsyncRelayCommand(async () =>
        {
           await NavigationService.Navigate<SelectLanguageView>(true);
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

            await DialogService.ShowActivityIndicatorAndReturnResult(AppResources.General_UploadingProgress, async () =>
            {
                var result = await _generalDatabaseSynchronizationManager.ClearBackendAndSyncLocalDataAsync();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (result)
                    {
                        await _toast.DisplayToast(AppResources.SettingViewModel_DataSync);
                    }
                    else
                    {
                        await _toast.DisplayToast(AppResources.SettingViewModel_FailedDataSync);
                    }
                });

                return result;
            });
        });

    }
}