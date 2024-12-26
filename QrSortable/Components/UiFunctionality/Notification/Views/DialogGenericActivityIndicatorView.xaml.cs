namespace QrSortable.Components.UiFunctionality.Notification.Views
{
    using Navigation.Views;
    using ViewModels;

    /// <summary>
    ///     The code behind of the generic dialog activity indicator view.
    /// </summary>
    public partial class DialogGenericActivityIndicatorView : BaseView

    {
        /// <summary>
        ///  Initializes a new instance of the DialogGenericActivityIndicatorView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The SDialogGenericActivityIndicatorViewModel associated with this view.</param>
        public DialogGenericActivityIndicatorView(DialogGenericActivityIndicatorViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Updates the width of the dialog to take half of the screen width.
        /// </summary>
        /// <param name="width">The width of the view.</param>
        /// <param name="height">The height of the view.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            DialogLayout.WidthRequest = width / 1.5;
        }
    }
}