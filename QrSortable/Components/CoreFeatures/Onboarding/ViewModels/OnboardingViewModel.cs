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


        partial void OnIsMultiuserFunctionalityEnabledChanged(bool value)
        {
            // Trigger your existing command
            MultiuserFunctionalityToggledCommand.Execute(null);
        }


        public AsyncRelayCommand MultiuserFunctionalityToggledCommand => new AsyncRelayCommand(async () =>
        {
            if (IsMultiuserFunctionalityEnabled)
            {
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);
            }
            else
            {
                if(_displayOnce)
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
               
            }

        });

        public AsyncRelayCommand DoneCommand => new AsyncRelayCommand(async () =>
        {
            var multiuserIdInput = MultiuserIdInput?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(multiuserIdInput))
            {
                var confirmMessage = await DialogService.ShowRequestDialog(AppResources.OnboardingViewModel_CheckValidMultiId,
                AppResources.General_YesText, AppResources.General_NoText);

                if(!confirmMessage)
                {
                    if (!await EnsureSubscriptionAsync())
                        return;

                    await DialogService.ShowAlertDialog(AppResources.Dialog_Conformation, AppResources.General_SucessText,AppResources.Dialog_OK_Text);
                    await NavigationService.Navigate<RootView>();
                }
                return;

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

        public AsyncRelayCommand BackCommand => new AsyncRelayCommand(async () =>
        {
            if (!IsFromApp)
                Application.Current.Quit();

            try
            {
                Console.WriteLine("BackCommand executing...");

                if (!await IsSubscriptionActiveAsync())
                    await SetBackendDisable();

                Console.WriteLine("BackCommand: About to navigate back");
                BackNavigationCommand.Execute(null);
                Console.WriteLine("BackCommand: Navigation initiated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BackCommand error: {ex}");
            }
        });

        private async Task<bool> EnsureSubscriptionAsync()
        {
        
            if (await IsSubscriptionActiveAsync())
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

        private async Task<bool> IsSubscriptionActiveAsync()
        {
            var list = await _databaseManager.GetListAsync<SubscriptionEntity>();
            var subscription = list?.FirstOrDefault();
            return subscription?.IsSubscribed == true;
        }

        private async Task SetBackendDisable()
        {
            _displayOnce = false;
            IsMultiuserFunctionalityEnabled = false;
            await _generalInformationManager.UpdateIsBackendUsedAsync(false);
        }

    }
}