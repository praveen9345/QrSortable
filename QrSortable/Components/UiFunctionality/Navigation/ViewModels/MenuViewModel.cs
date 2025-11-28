namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.Models;
    using QrSortable.Resources;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the MenuViewModel  screen.
    /// </summary>
    public partial class MenuViewModel : BaseViewModel
    {
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuViewModel" />.
        /// </summary>
        public MenuViewModel()
        {
            IsBackNavigationEnabled = true;
            
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Icon=IconNames.Profile, Title="Profile" },
                new MenuItem { Icon=IconNames.Subscribe, Title="Subscribe" },
                new MenuItem { Icon=IconNames.Share, Title="Share" },
                new MenuItem { Icon=IconNames.YourOrders, Title="Your Orders" },
                new MenuItem { Icon=IconNames.Settings, Title="Settings" },
                new MenuItem { Icon=IconNames.Feedback, Title="Feedback" },
                new MenuItem { Icon=IconNames.LogOut, Title="LogOut" , HasBadge=true, Badge="Coming Soon"},
                new MenuItem { Icon=IconNames.ProfileDelete, Title="Delete Account" }
            };

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
        /// Represents the currently selected menu item in the application.
        /// </summary>
        [ObservableProperty]
        private MenuItem _selectedMenuItem;

        public IAsyncRelayCommand<MenuItem> OnSelectionMenuItemChangedCommand =>
            new AsyncRelayCommand<MenuItem>(async (selected) =>
            {
                if (selected == null)
                    return;

                switch (selected.Title)
                {
                    case "Your Orders":
                        await NavigationService.Navigate<YoursOrdersView>();
                        break;
                    case "":
                        // Future Implementation
                        break;
                }


                SelectedMenuItem = null;
            });
        /// <summary>
        ///     The navigation command to close the menu.
        /// </summary>
        public AsyncRelayCommand CloseButtonCommand =>
            new AsyncRelayCommand(() => NavigationService.Close());


    }
}