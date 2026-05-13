namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the feedback screen.
    /// </summary>
    public partial class HelpViewModel : BaseViewModel
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpViewModel" />.
        /// </summary>
        public HelpViewModel()
        {
            IsBackNavigationEnabled = true;
        }
    }
}