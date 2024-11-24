namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class RootViewModel : BaseViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RootViewModel" />.
        /// </summary>
        public RootViewModel()
        {
            IsBackNavigationEnabled = true;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

        }
    }
}