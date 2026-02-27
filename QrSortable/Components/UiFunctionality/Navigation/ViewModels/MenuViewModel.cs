namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Onboarding.Views;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.CoreFeatures.Settings.Views;
    using QrSortable.Components.UiFunctionality.Navigation.Models;
    using QrSortable.Resources;
    using System.Collections.ObjectModel;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;

    /// <summary>
    ///     The view model of the MenuViewModel  screen.
    /// </summary>
    public partial class MenuViewModel : BaseViewModel
    {
        private readonly IMauiEssentialsWrapper _mauiEssentialWrapper;
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        private const string MultiuserTitle = "Multiuser";
        private const string SubscribeTitle = "Subscribe";
        private const string ShareTitle = "Share";
        private const string AddToBasketTitle = "Basket";
        private const string YourOrdersTitle = "Your Orders";
        private const string SettingsTitle = "Settings";
        private const string FeedbackTitle = "Feedback";
        private const string ProfileDeleteTitle = "Profile Delete";


        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuViewModel" />.
        /// </summary>
        /// <param name="mauiEssentialsWrapper">An instance of <see cref="IMauiEssentialsWrapper" /> used to access platform-specific features.</param>
        public MenuViewModel(IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;
            _mauiEssentialWrapper = mauiEssentialsWrapper;

            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Icon=IconNames.Multiuser, Title= MultiuserTitle },
                new MenuItem { Icon=IconNames.AddToBasket, Title= AddToBasketTitle },
                new MenuItem { Icon=IconNames.YourOrders, Title=YourOrdersTitle },
                new MenuItem { Icon=IconNames.Subscribe, Title=SubscribeTitle , HasBadge=true, Badge="Coming Soon"},
                new MenuItem { Icon=IconNames.Share, Title=ShareTitle },
                new MenuItem { Icon=IconNames.Settings, Title=SettingsTitle },
                new MenuItem { Icon=IconNames.Feedback, Title=FeedbackTitle },
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
                    case MultiuserTitle:
                        await NavigationService.Navigate<OnboardingView>(true);
                        break;
                    case AddToBasketTitle:
                        await NavigationService.Navigate<AddToBasketView>();
                        break;
                    case YourOrdersTitle:
                        await NavigationService.Navigate<YoursOrdersView>();
                        break;
                    case SubscribeTitle:
                        await NavigationService.Navigate<SubscriptionView>(false);
                        break;
                    case SettingsTitle:
                        await NavigationService.Navigate<SettingView>();
                        break;
                    case FeedbackTitle:
                        await NavigationService.Navigate<WebView>("feedback");
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

        private async Task<bool> DisplayInternetConcectivityAsync()
        {
            if (!_mauiEssentialWrapper.IsInternetConnectionAvailable())
            {
                 await DialogService.ShowAlertDialog("🌐 Connectivity",
                "Internet Connection is required.", AppResources.Dialog_OK_Text);
                await NavigationService.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}