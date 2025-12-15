namespace QrSortable.Components.CoreFeatures.Onboarding.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using PdfSharpCore.Drawing.BarCodes;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class OnboardingViewModel : BaseViewModel
    {
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly IToastService _toastService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnboardingViewModel" />.
        /// </summary>
        public OnboardingViewModel(IGeneralInformationManager generalInformationManager, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _toastService = toastService;
            _generalInformationManager = generalInformationManager;
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            MultiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
        }

        public AsyncRelayCommand CopyMultiuserCodeCommand => new AsyncRelayCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(MultiuserId);
            await _toastService.DisplayToast("MultiuserId copied to clipboard!");

        });

        /// <summary>
        /// Represents the currently multiuser identification in the application.
        /// </summary>
        [ObservableProperty]
        private string _multiuserId;

    }
}