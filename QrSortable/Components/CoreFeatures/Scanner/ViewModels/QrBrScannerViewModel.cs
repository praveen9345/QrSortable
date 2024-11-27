namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using BarcodeScanning;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class QrBrScannerViewModel : BaseViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QrBrScannerViewModel" />.
        /// </summary>
        public QrBrScannerViewModel()
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
            await Methods.AskForRequiredPermissionAsync();
        }

        public AsyncRelayCommand FlashLightCommand => new AsyncRelayCommand(async () =>
        {
            
        });

    }
}