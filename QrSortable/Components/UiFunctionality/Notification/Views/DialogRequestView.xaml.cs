namespace QrSortable.Components.UiFunctionality.Notification.Views
{
    using Navigation.Views;
    using Microsoft.Maui.Controls.PlatformConfiguration;
    using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
    using Microsoft.Maui.Controls.Xaml;
    using ViewModels;

    /// <summary>
    ///     The code behind of the dialog request view.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogRequestView : BaseView
    {
        /// <summary>
        ///     The default constructor of the dialog request view.
        /// </summary>
        /// <param name="viewModel">The DialogRequestViewModel associated with this view.</param>
        public DialogRequestView(DialogRequestViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
        }

        /// <summary>
        /// Overrides the OnSizeAllocated method to handle changes in the size of the view.
        /// Adjusts the WidthRequest of the RequestBorder, accounting for left and right padding.
        /// </summary>
        /// <param name="width">The width of the view.</param>
        /// <param name="height">The height of the view.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            //The number 30 is the left + right padding of the stack layout 
            RequestBorder.WidthRequest = width - 30;
        }
    }
}