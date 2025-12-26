namespace QrSortable.Components.CoreFeatures.Onboarding.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the OnboardingView.
    /// </summary>
    public partial class OnboardingView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the OnboardingViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The OnboardingViewModel associated with this view.</param>
        public OnboardingView(OnboardingViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

        }
    }
}