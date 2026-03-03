namespace QrSortable.Components.UiFunctionality.Navigation.Views
{
    using ViewModels;

    /// <summary>
    /// The code behind of the MenuView.
    /// </summary>
    public partial class MenuView : BaseView
    {
        private readonly MenuViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the MenuViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The MenuViewModel associated with this view.</param>
        public MenuView(MenuViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnMenuItemTapped(object sender, TappedEventArgs e)
        {
            if (sender is Grid grid && grid.BindingContext is 
                Models.MenuItem menuItem)
            {
                // Animation
                grid.BackgroundColor = Color.FromArgb("#33ffffff");
                await grid.ScaleTo(0.96, 80, Easing.CubicOut);
                await grid.ScaleTo(1.0, 80, Easing.CubicIn);
                grid.BackgroundColor = Colors.Transparent;

                // Trigger navigation command

                if (_viewModel?.OnSelectionMenuItemChangedCommand.CanExecute(menuItem) == true)
                    await _viewModel.OnSelectionMenuItemChangedCommand.ExecuteAsync(menuItem);
            }
        }
    }
}