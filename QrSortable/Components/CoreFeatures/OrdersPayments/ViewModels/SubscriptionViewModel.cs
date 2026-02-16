namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The view model of the Subscription screen.
    /// </summary>
    public partial class SubscriptionViewModel : BaseViewModel
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionViewModel(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;

            PremiumFeatures = new ObservableCollection<string>
            {
                "Multi-User Sharing",
                "Unlimited Items per Box",
                "Cloud Backup & Sync",
                "Move Items Between Boxes",
                "Multiple Images per Item",
                "No Advertisements"
            };

            SelectedPlan = SubscriptionPlan.Monthly;

            LoadState();
        }

        #region Observable Properties

        [ObservableProperty]
        private bool isSubscribed;

        [ObservableProperty]
        private string subscriptionStatusText;

        [ObservableProperty]
        private string priceText;

        [ObservableProperty]
        private SubscriptionPlan selectedPlan;

        public ObservableCollection<string> PremiumFeatures { get; }

        #endregion

        #region Initialization

        private async void LoadState()
        {
            await _subscriptionService.LoadAsync();
            UpdateState();
            UpdatePrice();
        }

        private void UpdateState()
        {
            IsSubscribed = _subscriptionService.IsSubscribed;

            SubscriptionStatusText = IsSubscribed
                ? "🌟 Premium Active"
                : "Upgrade to unlock premium features";
        }

        partial void OnSelectedPlanChanged(SubscriptionPlan value)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            PriceText = SelectedPlan switch
            {
                SubscriptionPlan.Monthly => "$4.99 / month",
                SubscriptionPlan.Yearly => "$49.99 / year (Save 20%)",
                _ => "$4.99 / month"
            };
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task Subscribe()
        {
            // In real app → trigger store purchase here
            await _subscriptionService.ActivateSubscriptionAsync();
            UpdateState();

            await DialogService.ShowAlertDialog("Success",
                "Premium activated successfully 🎉",
                "OK");
        }

        [RelayCommand]
        private async Task CancelSubscription()
        {
            await _subscriptionService.CancelSubscriptionAsync();
            UpdateState();

            await DialogService.ShowAlertDialog("Cancelled",
                "Your subscription has been cancelled.",
                "OK");
        }

        [RelayCommand]
        private async Task UsePremiumFeature()
        {
            if (!IsSubscribed)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Premium Required",
                    "This feature requires a paid subscription.",
                    "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert(
                "Premium Feature",
                "You are using a premium feature!",
                "OK");
        }

        #endregion
    }

    public enum SubscriptionPlan
    {
        Monthly,
        Yearly
    }
}