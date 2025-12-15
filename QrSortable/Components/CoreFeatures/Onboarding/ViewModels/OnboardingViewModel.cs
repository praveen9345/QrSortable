namespace QrSortable.Components.CoreFeatures.Onboarding.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;


    /// <summary>
    ///     The view model of the OnboardingViewModel screen.
    /// </summary>
    public partial class OnboardingViewModel : BaseViewModel
    {
        private readonly IGeneralInformationManager _generalInformationManager;
        /// <summary>
        ///     Initializes a new instance of the <see cref="OnboardingViewModel" />.
        /// </summary>
        public OnboardingViewModel(IGeneralInformationManager generalInformationManager)
        {
            IsBackNavigationEnabled = true;
            _generalInformationManager = generalInformationManager;
        }

        public async override void ViewAppearing()
        {
            base.ViewAppearing();

            MultiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
        }

        /// <summary>
        /// Represents the currently multiuser identification in the application.
        /// </summary>
        [ObservableProperty]
        private string _multiuserId;

    }
}