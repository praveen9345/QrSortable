namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    public interface ISubscriptionService
    {
        bool IsSubscribed { get; }
        Task ActivateSubscriptionAsync();
        Task CancelSubscriptionAsync();
        Task LoadAsync();
    }
}
