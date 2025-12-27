namespace QrSortable.Components.CoreFeatures.Onboarding.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Threading.Tasks;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class OnboardingViewModel : BaseViewModel
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
            IsBackNavigationEnabled = true;
            _toastService = toastService;
            _generalInformationManager = generalInformationManager;
            _backendCommunicationService = backendCommunicationService;
            _generalDatabaseSynchronizationManager  = generalDatabaseSynchronizationManager;
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

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            MultiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
            
            if ((await _generalInformationManager.GetGeneralInformationAsync()).IsBackendUsed)
            {
                IsMultiuserFunctionalityEnabled = true;
            }
        }

        public AsyncRelayCommand CopyMultiuserCodeCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(MultiuserId);
            await _toastService.DisplayToast("MultiuserId copied to clipboard!");

        });

        public AsyncRelayCommand MultiuserFunctionalityToggledCommand => new AsyncRelayCommand(async () =>
        {
            if (IsMultiuserFunctionalityEnabled)
            {
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);

                if(!_displayOnce)
                {
                    await DialogService.ShowAlertDialog("Information",
                   "Use the displayed multi-user ID on another QRSortable App device to connect.", AppResources.Dialog_OK_Text);
                }
                _displayOnce = false;
            }
            else
            {
                var confirm = await DialogService.ShowRequestDialog("Disabling multi-user functionality will stop synchronization with other devices.",
                    AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm) 
                {
                    IsMultiuserFunctionalityEnabled = true;
                    return; 
                }

                await _generalInformationManager.UpdateIsBackendUsedAsync(false);
                IsMultiuserFunctionalityEnabled = false;
            }
           
        });

        public AsyncRelayCommand DoneCommand => new AsyncRelayCommand(async () =>
        {

            var confirmMessage = await DialogService.ShowRequestDialog("To use multi-user features, enter a valid multi-user ID. If you don’t have one, we’ll create a new account for you.",
               "Create One", "Done");
            if (!confirmMessage)
            {
                await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.SignUp);
                await NavigationService.Navigate<RootView>();
                return;
            }
            else 
            {
                if (string.IsNullOrWhiteSpace(MultiuserIdInput))
                {
                    await DialogService.ShowAlertDialog("Error",
                   "Please enter a valid multi-user ID to proceed.", AppResources.Dialog_OK_Text);
                    return;
                }

            }

            if(!await _backendCommunicationService.ValidateMultiuserIdAsync(MultiuserIdInput))
            {
                await DialogService.ShowAlertDialog("Error",
               "The entered multi-user ID is invalid or at least you need one have one saved data in given multi-user ID QRSortable App. Please check and try again.", 
               AppResources.Dialog_OK_Text);
                return;
            }

            if(MultiuserIdInput == MultiuserId)
            {
                var confirm = await DialogService.ShowRequestDialog("The multi-user ID you entered is the same as the one already used on this device.If you press OK, all data saved on this device will be deleted and re-downloaded from the server.",
                AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if(!confirm) return;
            }
            else
            {
                var confirm = await DialogService.ShowRequestDialog("Are you sure you want to clear the data saved on this device?",
                 AppResources.Dialog_Cancel_Text, AppResources.Dialog_OK_Text);

                if (!confirm) { return; }
            }

            var success = await _generalInformationManager.UpdateTheMultiuserIdAsync(MultiuserIdInput);
            if(!success)
            {
                await _toastService.DisplayToast("An error occurred while setting the multi-user ID. Please try again.");
                return;
            }
           
            MultiuserId = MultiuserIdInput;

            var result = await DialogService.ShowActivityIndicatorAndReturnResult(
            "Downloading your data. This may take a few moments…",
            async () =>
            {
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);
                await _databaseManager.ClearDatabaseAsync();
                 await _backendDatabaseManager.ClearDatabaseAsync();

                 return await _generalDatabaseSynchronizationManager
                     .SynchronizeAppDataAsync();
            });

            if (result is bool successResult && successResult)
            {

                await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.SignUp);
                await NavigationService.Navigate<RootView>();
            }
            else
            {
                await _toastService.DisplayToast(
                    "Download failed. Please check your internet connection and try again.");
            }

        });

    }
}