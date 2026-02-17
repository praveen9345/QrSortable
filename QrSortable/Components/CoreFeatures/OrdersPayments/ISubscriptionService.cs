using Mollie.Api.Models.Payment.Response;

namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    public interface ISubscriptionService
    {
        bool IsSubscribed { get; }

        Task LoadAsync();

        Task<PaymentResponse> CreateInitialSubscriptionPaymentAsync(
            string email,
            string currency,
            decimal amount);

        Task FinalizeSubscriptionAsync(
            string email,
            string currency,
            decimal amount);

        Task<PaymentResponse> GetPaymentStatusAsync(string paymentId);

        Task CancelSubscriptionAsync();
    }
}
