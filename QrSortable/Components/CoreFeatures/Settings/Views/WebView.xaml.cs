namespace QrSortable.Components.CoreFeatures.Settings.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the WebView.
    /// </summary>
    public partial class WebView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the WebViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The WebViewModel associated with this view.</param>
        public WebView(WebViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

        }
       
    }
}