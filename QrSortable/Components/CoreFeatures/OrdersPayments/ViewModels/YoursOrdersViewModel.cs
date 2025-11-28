namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.UiFunctionality.Navigation.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the YoursOrdersViewModel screen.
    /// </summary>
    public partial class YoursOrdersViewModel : BaseViewModel
    {


        /// <summary>
        ///     Initializes a new instance of the <see cref="YoursOrdersViewModel" />.
        /// </summary>
        public YoursOrdersViewModel()
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