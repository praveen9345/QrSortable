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

        private const string ProfileTitle = "Profile";
        private const string SubscribeTitle ="Subscribe";
        private const string ShareTitle = "Share";
        private const string AddToBasketTitle = "AddToBasket";
        private const string YourOrdersTitle = "YourOrders";
        private const string SettingsTitle = "Settings";
        private const string FeedbackTitle = "Feedback";
        private const string LogOutTitle = "LogOut";
        private const string ProfileDeleteTitle = "ProfileDelete";
        

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuViewModel" />.
        /// </summary>
        public MenuViewModel()
        {
            IsBackNavigationEnabled = true;
            
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Icon=IconNames.Profile, Title= ProfileTitle },
                new MenuItem { Icon=IconNames.Subscribe, Title=SubscribeTitle },
                new MenuItem { Icon=IconNames.Share, Title=ShareTitle },
                new MenuItem { Icon=IconNames.AddToBasket, Title= AddToBasketTitle },
                new MenuItem { Icon=IconNames.YourOrders, Title=YourOrdersTitle },
                new MenuItem { Icon=IconNames.Settings, Title=SettingsTitle },
                new MenuItem { Icon=IconNames.Feedback, Title=FeedbackTitle },
                new MenuItem { Icon=IconNames.LogOut, Title=LogOutTitle , HasBadge=true, Badge="Coming Soon"},
                new MenuItem { Icon=IconNames.ProfileDelete, Title=ProfileDeleteTitle }
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
                    case AddToBasketTitle:
                        await NavigationService.Navigate<AddToBasketView>();
                        break;
                    case YourOrdersTitle:
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