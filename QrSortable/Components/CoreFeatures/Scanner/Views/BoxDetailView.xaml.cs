namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the BoxDetail view.
    /// </summary>
    public partial class BoxDetailView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the BoxDetail class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The BoxDetailModel associated with this view.</param>
        public BoxDetailView(BoxDetailViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }
    }
}