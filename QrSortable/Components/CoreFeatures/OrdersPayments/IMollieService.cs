namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using Mollie.Api.Models.Payment.Response;

    /// <summary>
    ///     Interface of the service .....................
    /// </summary>
    public interface IMollieService
    {

        /// <summary>
        ///     ................... .....................
        /// </summary>
        Task<object> CreatePaymentOrSubscriptionAsync(
            decimal amount,
            string currency,
            string paymentMethod,
            string description,
            bool isSubscription = false,
            string customerEmail = null);

        /// <summary>
        ///     ................... .....................
        /// </summary>
        Task<PaymentResponse> GetPaymentStatusAsync(string paymentId);

    }
}