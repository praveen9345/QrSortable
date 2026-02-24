namespace QrSortable.Components.CoreFeatures.Onboarding.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Threading.Tasks;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class OnboardingViewModel : BaseViewModel<bool>
    {
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly IToastService _toastService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IBackendDatabaseManager _backendDatabaseManager;

        private bool _displayOnce;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnboardingViewModel" />.
        /// </summary>
        public OnboardingViewModel(IGeneralInformationManager generalInformationManager,
            IToastService toastService, IBackendCommunicationService backendCommunicationService,
            IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager,
            IDatabaseManager databaseManager, IBackendDatabaseManager backendDatabaseManager)
        {
            _toastService = toastService;
            _generalInformationManager = generalInformationManager;
            _backendCommunicationService = backendCommunicationService;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _databaseManager = databaseManager;
            _backendDatabaseManager = backendDatabaseManager;

        }

        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            _displayOnce = true;
        }

        /// <summary>
        /// Represents the currently multiuser identification in the application.
        /// </summary>
        [ObservableProperty]
        private string _multiuserId;

        /// <summary>
        /// Represents the currently multiuser id input in the application.
        /// </summary>
        [ObservableProperty]
        private string _multiuserIdInput;

        /// <summary>
        /// Represents the to set the visibility of the component in the application.
        /// </summary>
        [ObservableProperty]
        private bool _isMultiuserFunctionalityEnabled;

        /// <summary>
        /// Represents the to set the visibility of the back in the application.
        /// </summary>
        [ObservableProperty]
        private bool _isFromApp;

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            MultiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;

            if ((await _generalInformationManager.GetGeneralInformationAsync()).IsBackendUsed)
            {
                IsMultiuserFunctionalityEnabled = true;
            }
        }

        /// <summary>
        ///     Prepares the viewmodel with a boolean.
        /// </summary>
        /// <param name="isFromApp">
        ///     True when the view model is opened from the application and false when the view model is opened from onboarding.
        /// </param>
        public override void Prepare(bool isFromApp)
        {
            IsFromApp = isFromApp;
            IsBackNavigationEnabled = isFromApp;
        }


        public AsyncRelayCommand CopyMultiuserCodeCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(MultiuserId);
            await _toastService.DisplayToast(AppResources.OnboardingViewModel_ClipboardText);

        });

        public AsyncRelayCommand MultiuserFunctionalityToggledCommand => new AsyncRelayCommand(async () =>
        {
            if (IsMultiuserFunctionalityEnabled)
            {
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);

                if (!_displayOnce)
                {
                    await DialogService.ShowAlertDialog(AppResources.Dialog_InformationText,
                   AppResources.OnboardingViewModel_MultiIdFromOtherText, AppResources.Dialog_OK_Text);
                }
                _displayOnce = false;
            }
            else
            {
                var confirm = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_DisablingMultiIduserText,
                    AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm)
                {
                    IsMultiuserFunctionalityEnabled = true;
                    return;
                }

                await SetBackendDisable();
            }

        });

        public AsyncRelayCommand DoneCommand => new AsyncRelayCommand(async () =>
        {
            var multiuserIdInput = MultiuserIdInput?.Trim().ToUpperInvariant();

            var confirmMessage = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_CheckValidMultiId,
               AppResources.OnboardingViewModel_CreateOne, AppResources.OnboardingViewModel_DoneButtText);

            if (!confirmMessage)
            {
                if ((await _generalInformationManager.GetGeneralInformationAsync()).IsBackendUsed)
                {
                    var subscribed = await EnsureSubscriptionAsync();
                    if (!subscribed) return;
                }
                else
                {
                    await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.OnboardingCompleted);
                    await NavigationService.Navigate<RootView>();
                    return;
                }

            }
            else
            {

                if (string.IsNullOrWhiteSpace(multiuserIdInput))
                {
                    await DialogService.ShowAlertDialog(AppResources.Dialog_Error,
                   AppResources.OnboardingViewModel_ValidMultiIdError, AppResources.Dialog_OK_Text);
                    await SetBackendDisable();
                    return;
                }

            }

            if (!await _backendCommunicationService.ValidateMultiuserIdAsync(multiuserIdInput))
            {
                await DialogService.ShowAlertDialog(AppResources.Dialog_Error,
                AppResources.OnboardingViewModel_InvalidIdError, AppResources.Dialog_OK_Text);
                await SetBackendDisable();
                return;
            }

            if (multiuserIdInput == MultiuserId)
            {
                var confirm = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_AlreadyIdError,
                AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm)
                {
                    await SetBackendDisable();
                    return;
                }
            }
            else
            {
                //Sync data
                 await _generalDatabaseSynchronizationManager.SyncSubscriptionFromFirebaseAsync(multiuserIdInput);
                //Subscription
                var subscriptionOk = await EnsureSubscriptionAsync();
                if (!subscriptionOk) return;

                var confirm = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_ClearDataSaved,
                 AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm)
                {
                    await SetBackendDisable();
                    return;
                }
            }

            var success = await _generalInformationManager.UpdateTheMultiuserIdAsync(multiuserIdInput);
            if (!success)
            {
                await _toastService.DisplayToast(AppResources.OnboardingViewModel_SettingMultiIdError);
                await SetBackendDisable();
                return;
            }

            MultiuserId = multiuserIdInput;

            var result = await DialogService.ShowActivityIndicatorAndReturnResult(
            AppResources.OnboardingViewModel_DownloadingText,
            async () =>
            {
                await _databaseManager.ClearStorageBasketOrderedAsync();
                await _backendDatabaseManager.ClearDatabaseAsync();
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);

                return await _generalDatabaseSynchronizationManager
                     .SynchronizeAppDataAsync();
            });

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (result is bool successResult && successResult)
                {
                    await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.OnboardingCompleted);
                    await NavigationService.Navigate<RootView>();
                }
                else
                {
                    await SetBackendDisable();
                    await _toastService.DisplayToast(AppResources.OnboardingViewModel_DownloadFailed);
                }
            });


        });

        private async Task<bool> EnsureSubscriptionAsync()
        {
        
            var list = await _databaseManager.GetListAsync<SubscriptionEntity>();
            var subscription = list?.FirstOrDefault();

            if (subscription?.IsSubscribed == true)
                return true;

            var confirm = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_SubscriptionError,
                AppResources.Dialog_Cancel_Text,AppResources.Dialog_OK_Text);

            if (!confirm)
            {
                await SetBackendDisable();
                return false;
            }

            await NavigationService.Navigate<SubscriptionView>(true);
            return false;
        }

        private async Task SetBackendDisable()
        {
            IsMultiuserFunctionalityEnabled = false;
            await _generalInformationManager.UpdateIsBackendUsedAsync(false);
        }

    }
}