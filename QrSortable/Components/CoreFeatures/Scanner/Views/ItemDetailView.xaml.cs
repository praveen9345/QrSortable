namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the ItemDetailView view.
    /// </summary>
    public partial class ItemDetailView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the ItemDetailView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The ItemDetailViewModel associated with this view.</param>
        public ItemDetailView(ItemDetailViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }
    }
}