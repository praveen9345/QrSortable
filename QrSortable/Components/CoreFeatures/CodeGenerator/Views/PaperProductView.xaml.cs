namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the PaperProductView view.
    /// </summary>
    public partial class PaperProductView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the PaperProductViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The PaperProductViewModel associated with this view.</param>
        public PaperProductView(PaperProductViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}