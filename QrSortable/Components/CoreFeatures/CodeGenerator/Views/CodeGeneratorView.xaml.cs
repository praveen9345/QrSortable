namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class CodeGeneratorView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the CodeGeneratorViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The CodeGeneratorViewModel associated with this view.</param>
        public CodeGeneratorView(CodeGeneratorViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }
    }
}