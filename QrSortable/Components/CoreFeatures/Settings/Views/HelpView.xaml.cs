namespace QrSortable.Components.CoreFeatures.Settings.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the HelpView.
    /// </summary>
    public partial class HelpView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the HelpViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The HelpViewModel associated with this view.</param>
        public HelpView(HelpViewModel viewModel) : base(viewModel)
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