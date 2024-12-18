namespace QrSortable.Components.UiFunctionality.Notification.Views
{
    using Navigation.Views;
    using Microsoft.Maui.Controls.PlatformConfiguration;
    using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
    using ViewModels;

    /// <summary>
    ///     The code behind of the dialog alert view.
    /// </summary>
    public partial class DialogAlertView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the DialogAlertView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">TheDialogAlertViewModel associated with this view.</param>
        public DialogAlertView(DialogAlertViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
        }
    }
}