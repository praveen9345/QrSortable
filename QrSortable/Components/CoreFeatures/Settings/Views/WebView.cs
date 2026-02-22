namespace QrSortable.Components.CoreFeatures.Settings.Views
{
    using ViewModels;
    using UiFunctionality.Navigation.Views;

    /// <summary>
    /// The code behind of the web view.
    /// </summary>
    public partial class WebView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the WebView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The WebViewModel associated with this view.</param>
        public WebView(WebViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}