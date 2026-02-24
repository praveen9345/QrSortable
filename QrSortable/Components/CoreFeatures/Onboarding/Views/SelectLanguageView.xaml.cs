namespace QrSortable.Components.CoreFeatures.Onboarding.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the SelectLanguageView.
    /// </summary>
    public partial class SelectLanguageView : BaseView
    {
        private readonly SelectLanguageViewModel _viewModel;
        /// <summary>
        ///  Initializes a new instance of the SelectLanguageViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SelectLanguageViewModel associated with this view.</param>
        public SelectLanguageView(SelectLanguageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

        }

        protected override bool OnBackButtonPressed()
        {
            if (!_viewModel.IsFromApp)
            {
                Application.Current.Quit();

            }
            return base.OnBackButtonPressed();
        }
    }
}