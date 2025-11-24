namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using Microsoft.Maui.Controls;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.IO;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class SelectProductView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the SelectProductViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SelectProductViewModel associated with this view.</param>
        public SelectProductView(SelectProductViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}