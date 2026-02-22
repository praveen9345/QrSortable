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
        private async void OnTappedEffect(object sender, EventArgs e)
        {
            Grid? grid = null;

            if (sender is Grid g)
            {
                grid = g;
            }
            else if (sender is TapGestureRecognizer tap && tap.Parent is Grid parentGrid)
            {
                grid = parentGrid;
            }

            if (grid == null)
                return;

            grid.BackgroundColor = (Color)Application.Current.Resources["Gray85"];
            await Task.Delay(100);
            grid.BackgroundColor = Colors.Transparent;
        }
    }
}