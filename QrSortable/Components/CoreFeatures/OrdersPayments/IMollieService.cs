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
        Task<PaymentResponse> CreatePaymentAsync(decimal amount, string currency, string pymentMethod, string description);

        /// <summary>
        ///     ................... .....................
        /// </summary>
        Task<PaymentResponse> GetPaymentStatusAsync(string paymentId);

    }
}