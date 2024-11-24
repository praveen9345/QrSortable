namespace QrSortable.Components.UiFunctionality.Navigation.Views
{
    using ViewModels;

    /// <summary>
    /// The code behind of the RootView view.
    /// </summary>
    public partial class RootView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the RootView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The RootViewModel associated with this view.</param>
        public RootView(RootViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}