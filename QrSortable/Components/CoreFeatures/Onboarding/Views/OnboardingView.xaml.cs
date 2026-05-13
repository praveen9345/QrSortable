namespace QrSortable.Components.CoreFeatures.Onboarding.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.Threading.Tasks;
    using ViewModels;

    /// <summary>
    /// The code behind of the OnboardingView.
    /// </summary>
    public partial class OnboardingView : BaseView
    {
        private readonly OnboardingViewModel _viewModel;
        /// <summary>
        ///  Initializes a new instance of the OnboardingViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The OnboardingViewModel associated with this view.</param>
        public OnboardingView(OnboardingViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

        }
        protected override bool OnBackButtonPressed()
        {
           _viewModel.BackCommand.Execute(null);
            return base.OnBackButtonPressed();
        }
    }
}