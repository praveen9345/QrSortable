namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.UiFunctionality.Navigation.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Notification;

    /// <summary>
    ///     The view model of the YoursOrdersViewModel screen.
    /// </summary>
    public partial class YoursOrdersViewModel : BaseViewModel
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoursOrdersViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public YoursOrdersViewModel(IDatabaseManager databaseManager, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
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