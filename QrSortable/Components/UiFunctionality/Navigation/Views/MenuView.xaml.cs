namespace QrSortable.Components.UiFunctionality.Navigation.Views
{
    using ViewModels;

    /// <summary>
    /// The code behind of the MenuView.
    /// </summary>
    public partial class MenuView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the MenuViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The MenuViewModel associated with this view.</param>
        public MenuView(MenuViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}