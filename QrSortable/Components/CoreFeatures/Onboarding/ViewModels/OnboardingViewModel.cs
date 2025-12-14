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


        }


        private string GenereatedMultiuserId()
        {
            string seg1 = GenerateNumber(4);
            string seg2 = GenerateNumber(5);

            return $"QS{seg1}{seg2}";
        }

        private static string GenerateNumber(int digits)
        {
            Random random = new Random();
            int max = (int)Math.Pow(10, digits);
            int min = max / 10;
            return random.Next(min, max).ToString();
        }


        /// <summary>
        /// Represents the currently multiuser identification in the application.
        /// </summary>
        [ObservableProperty]
        private string _multiuserId;

    }
}