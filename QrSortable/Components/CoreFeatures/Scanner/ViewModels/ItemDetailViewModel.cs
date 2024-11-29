namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the ItemDetailView screen.
    /// </summary>
    public partial class ItemDetailViewModel : BaseViewModel
    {
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemDetailViewModel" />.
        /// </summary>
        public ItemDetailViewModel()
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