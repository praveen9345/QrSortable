namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using Mollie.Api.Client;
    using Mollie.Api.Models.Payment;
    using Mollie.Api.Models.Payment.Request;
    using Mollie.Api.Models.Payment.Response;
    using System.Globalization;

    /// <summary>
    ///     Implementation of the service providing navigation functionality.
    /// </summary>
    public class MollieService : IMollieService
    {
        private readonly PaymentClient _paymentClient;
        private readonly CustomerClient _customerClient;
        private readonly SubscriptionClient _subscriptionClient;
        private static readonly string  MOLLIE_TEST_API_KEY = "test_a4BaGmytRmSv6J2xSxp8j6ypATxEdf";
      
        /// <summary>
        ///     Initializes a new instance of the <see cref="MollieService"/> clss.
        /// </summary>
        
        public MollieService()
        {
            _paymentClient = new PaymentClient(MOLLIE_TEST_API_KEY);
            _customerClient = new CustomerClient(MOLLIE_TEST_API_KEY);
            _subscriptionClient = new SubscriptionClient(MOLLIE_TEST_API_KEY);
        }

        public async Task<object> CreatePaymentOrSubscriptionAsync(
            decimal amount, string currency, string paymentMethod,string description,
            bool isSubscription = false, string customerEmail = null)
        {
            if (!isSubscription)
            {
                return await _paymentClient.CreatePaymentAsync(new PaymentRequest
                {
                    Amount = new Mollie.Api.Models.Amount(GetMollieCurrencyType(currency), ToMollieAmount(amount)),
                    Description = description,
                    RedirectUrl = $"myapp://payment-return?id={{paymentId}}",
                    Method = GetMolliePaymentMethodType(paymentMethod)
                });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(customerEmail))
                    throw new ArgumentException("Customer email is required for subscriptions.");

                var customer = await _customerClient.CreateCustomerAsync(new Mollie.Api.Models.Customer.Request.CustomerRequest
                {
                    Email = customerEmail
                });

                var subscriptionRequest = new Mollie.Api.Models.Subscription.Request.SubscriptionRequest
                {
                    Amount = new Mollie.Api.Models.Amount(GetMollieCurrencyType(currency), ToMollieAmount(amount)),
                    Interval = "1 month",
                    Description = description,
                    StartDate = DateTime.UtcNow
                };

                return await _subscriptionClient.CreateSubscriptionAsync(customer.Id, subscriptionRequest);
            }
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
        {
            return await _paymentClient.GetPaymentAsync(paymentId);
        }

        private string GetMolliePaymentMethodType(string paymentMethod) 
        {
            var method = ""; 
            switch (paymentMethod) 
            {
                case "Card":
                    method = Mollie.Api.Models.Payment.PaymentMethod.CreditCard;
                    break;
                case "ApplePay":
                    method = Mollie.Api.Models.Payment.PaymentMethod.ApplePay;
                    break;
                case "PayPal":
                    method = Mollie.Api.Models.Payment.PaymentMethod.PayPal;
                    break;
            }
            return method;
        }

        private string GetMollieCurrencyType(string currency)
        {
            var currencyType = "";
            switch (currency)
            {
                case "Euro(€)":
                    currencyType = "EUR";
                    break;
                case "USD($)":
                    currencyType = "USD";
                    break;
                case "Australian dollar":
                    currencyType = "AUD";
                    break;
                case "Canadian dollar":
                    currencyType = "CAD";
                    break;
                case "Swiss franc":
                    currencyType = "CHF";
                    break;
                case "British pound":
                    currencyType = "GBP";
                    break;
            }
            return currencyType;
        }

        private string ToMollieAmount(decimal amount)
        {
            // Check how many decimal places the value has
            int[] bits = decimal.GetBits(amount);
            int decimalPlaces = (bits[3] >> 16) & 31;

            if (decimalPlaces == 2)
            {
                // Already 2 decimals → just format safely for Mollie
                return amount.ToString("0.00", CultureInfo.InvariantCulture);
            }

            // Not 2 decimals → round and format
            amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

            return amount.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}