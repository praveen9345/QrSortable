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

        private static readonly string MultiuserTitle = AppResources.MenuViewModel_MultiuserText;
        private static readonly string SubscribeTitle = AppResources.MenuViewModel_SubscribeText;
        private static readonly string ShareTitle = AppResources.MenuViewModel_ShareText;
        private static readonly string AddToBasketTitle = AppResources.MenuViewModel_BasketText;
        private static readonly string YourOrdersTitle = AppResources.MenuViewModel_YourOrderText;
        private static readonly string SettingsTitle = AppResources.MenuViewModel_SettingText;
        private static readonly string FeedbackTitle = AppResources.MenuViewModel_FeedbackText;


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
                new MenuItem { Icon=IconNames.Feedback, Title=FeedbackTitle }
            };

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
                    case var _ when selected.Title == MultiuserTitle:
                        await NavigationService.Navigate<OnboardingView>(true);
                        break;

                    case var _ when selected.Title == AddToBasketTitle:
                        await NavigationService.Navigate<AddToBasketView>();
                        break;

                    case var _ when selected.Title == YourOrdersTitle:
                        await NavigationService.Navigate<YoursOrdersView>();
                        break;

                    case var _ when selected.Title == SubscribeTitle:
                        await NavigationService.Navigate<SubscriptionView>(false);
                        break;

                    case var _ when selected.Title == SettingsTitle:
                        await NavigationService.Navigate<SettingView>();
                        break;

                    case var _ when selected.Title == FeedbackTitle:
                        await NavigationService.Navigate<WebView>("feedback");
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
                 await DialogService.ShowAlertDialog(AppResources.Dialog_InternetConnection_Title,
                AppResources.Dialog_InternetConnection_Message, AppResources.Dialog_OK_Text);
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