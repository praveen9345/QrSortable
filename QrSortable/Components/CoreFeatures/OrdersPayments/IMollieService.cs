namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using Mollie.Api.Models.Payment.Response;
    using Mollie.Api.Models.Subscription.Response;

    /// <summary>
    ///     Interface of the service .....................
    /// </summary>
    public interface IMollieService
    {

        Task<PaymentResponse> CreatePaymentAsync(decimal amount, string currency, string paymentMethod, 
            string description, string customerEmail = null);
        /// <summary>
        ///     ................... .....................
        /// </summary>
        Task<SubscriptionResponse> CreateSubscriptionAsync(decimal amount, string currency, string customerEmail, string description);

        Task CancelSubscriptionAsync(string customerId, string subscriptionId);

        /// <summary>
        ///     ................... .....................
        /// </summary>
        Task<PaymentResponse> GetPaymentStatusAsync(string paymentId);

    }
}