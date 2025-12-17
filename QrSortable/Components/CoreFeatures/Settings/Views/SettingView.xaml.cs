namespace QrSortable.Components.CoreFeatures.Settings.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the SettingView.
    /// </summary>
    public partial class SettingView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the SettingViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SettingViewModel associated with this view.</param>
        public SettingView(SettingViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

        }
    }
}