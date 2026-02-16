namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    public class SubscriptionService : ISubscriptionService
    {
        private const string SubscriptionKey = "is_subscribed";

        public bool IsSubscribed { get; private set; }

        public async Task LoadAsync()
        {
            IsSubscribed = Preferences.Get(SubscriptionKey, false);
            await Task.CompletedTask;
        }

        public async Task ActivateSubscriptionAsync()
        {
            IsSubscribed = true;
            Preferences.Set(SubscriptionKey, true);
            await Task.CompletedTask;
        }

        public async Task CancelSubscriptionAsync()
        {
            IsSubscribed = false;
            Preferences.Set(SubscriptionKey, false);
            await Task.CompletedTask;
        }
    }
}