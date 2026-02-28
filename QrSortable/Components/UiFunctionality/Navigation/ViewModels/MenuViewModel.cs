namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Onboarding.Views;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.CoreFeatures.Settings.Views;
    using QrSortable.Components.UiFunctionality.Navigation.Models;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using System.Collections.ObjectModel;
    using QrSortable.Resources;

    public partial class MenuViewModel : BaseViewModel
    {
        private readonly IMauiEssentialsWrapper _mauiEssentialWrapper;

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public MenuViewModel(IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            IsBackNavigationEnabled = true;
            _mauiEssentialWrapper = mauiEssentialsWrapper;

            BuildMenu();

            LocalizationService.Instance.PropertyChanged += (_, __) =>
            {
                BuildMenu();
            };
        }

        private void BuildMenu()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Icon=IconNames.Multiuser,
                    Title= LocalizationService.Instance["MenuViewModel_MultiuserText"]
                },
                new MenuItem
                {
                    Icon=IconNames.AddToBasket,
                    Title= LocalizationService.Instance["MenuViewModel_BasketText"]
                },
                new MenuItem
                {
                    Icon=IconNames.YourOrders,
                    Title= LocalizationService.Instance["MenuViewModel_YourOrderText"]
                },
                new MenuItem
                {
                    Icon=IconNames.Subscribe,
                    Title= LocalizationService.Instance["MenuViewModel_SubscribeText"],
                    HasBadge=true,
                    Badge="Coming Soon"
                },
                new MenuItem
                {
                    Icon=IconNames.Share,
                    Title= LocalizationService.Instance["MenuViewModel_ShareText"]
                },
                new MenuItem
                {
                    Icon=IconNames.Settings,
                    Title= LocalizationService.Instance["MenuViewModel_SettingText"]
                },
                new MenuItem
                {
                    Icon=IconNames.Feedback,
                    Title= LocalizationService.Instance["MenuViewModel_FeedbackText"]
                }
            };

            OnPropertyChanged(nameof(MenuItems));
        }

        [ObservableProperty]
        private MenuItem _selectedMenuItem;

        public IAsyncRelayCommand<MenuItem> OnSelectionMenuItemChangedCommand =>
            new AsyncRelayCommand<MenuItem>(async (selected) =>
            {
                if (selected == null)
                    return;

                // Compare using resource keys instead of translated text
                var key = selected.Title;

                if (key == LocalizationService.Instance["MenuViewModel_MultiuserText"])
                    await NavigationService.Navigate<OnboardingView>(true);

                else if (key == LocalizationService.Instance["MenuViewModel_BasketText"])
                    await NavigationService.Navigate<AddToBasketView>();

                else if (key == LocalizationService.Instance["MenuViewModel_YourOrderText"])
                    await NavigationService.Navigate<YoursOrdersView>();

                else if (key == LocalizationService.Instance["MenuViewModel_SubscribeText"])
                    await NavigationService.Navigate<SubscriptionView>(false);

                else if (key == LocalizationService.Instance["MenuViewModel_SettingText"])
                    await NavigationService.Navigate<SettingView>();

                else if (key == LocalizationService.Instance["MenuViewModel_FeedbackText"])
                    await NavigationService.Navigate<WebView>("feedback");

                SelectedMenuItem = null;
            });

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