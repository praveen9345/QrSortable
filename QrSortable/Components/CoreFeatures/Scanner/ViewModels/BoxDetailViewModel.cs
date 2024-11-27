namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the box view screen.
    /// </summary>
    public partial class BoxDetailViewModel : BaseViewModel<string>
    {
        private string _barcode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxDetailViewModel" />.
        /// </summary>
        public BoxDetailViewModel()
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

        /// <summary>
        ///     Prepares the viewmode with an barcode raw data.
        /// </summary>
        /// <param name="barcode">The string barcode data.</param>
        public override async void Prepare(string barcode)
        {
            _barcode = barcode.Trim();

        }


    }
}