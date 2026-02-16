using Mollie.Api.Client;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Mollie.Api.Models.Customer.Request;
using Mollie.Api.Models.Subscription.Request;
using Mollie.Api.Models.Subscription.Response;
using System.Globalization;

namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    public class MollieService : IMollieService
    {
        private readonly PaymentClient _paymentClient;
        private readonly CustomerClient _customerClient;
        private readonly SubscriptionClient _subscriptionClient;

        private static readonly string MOLLIE_TEST_API_KEY = "test_a4BaGmytRmSv6J2xSxp8j6ypATxEdf";

        public MollieService()
        {
            _paymentClient = new PaymentClient(MOLLIE_TEST_API_KEY);
            _customerClient = new CustomerClient(MOLLIE_TEST_API_KEY);
            _subscriptionClient = new SubscriptionClient(MOLLIE_TEST_API_KEY);
        }

        // ---------------- One-time Payment ----------------
        public async Task<PaymentResponse> CreatePaymentAsync(decimal amount, string currency, string paymentMethod, string description)
        {
            var paymentRequest = new PaymentRequest
            {
                Amount = new Mollie.Api.Models.Amount(GetMollieCurrencyType(currency), ToMollieAmount(amount)),
                Description = description,
                RedirectUrl = $"myapp://payment-return?id={{paymentId}}",
                Method = GetMolliePaymentMethodType(paymentMethod)
            };

            return await _paymentClient.CreatePaymentAsync(paymentRequest);
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
        {
            return await _paymentClient.GetPaymentAsync(paymentId);
        }

        // ---------------- One-time or Subscription (Integrated) ----------------
        public async Task<object> CreatePaymentOrSubscriptionAsync(
            decimal amount, string currency,string paymentMethod,
            string description, bool isSubscription = false,string customerEmail = null)
        {
            if (!isSubscription)
            {
                // One-time payment
                return await CreatePaymentAsync(amount, currency, paymentMethod, description);
            }
            else
            {
                // Subscription
                if (string.IsNullOrWhiteSpace(customerEmail))
                    throw new ArgumentException("Customer email is required for subscriptions.");

                // 1. Create or fetch customer
                var customer = await _customerClient.CreateCustomerAsync(new CustomerRequest
                {
                    Email = customerEmail
                });

                // 2. Create subscription
                var subscriptionRequest = new SubscriptionRequest
                {
                    Amount = new Mollie.Api.Models.Amount(GetMollieCurrencyType(currency), ToMollieAmount(amount)),
                    Interval = "1 month",
                    Description = description,
                    StartDate = DateTime.UtcNow
                };

                return await _subscriptionClient.CreateSubscriptionAsync(customer.Id, subscriptionRequest);
            }
        }

        // ---------------- Subscription Management ----------------
        public async Task CancelSubscriptionAsync(string customerId, string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("Customer ID is required.");

            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new ArgumentException("Subscription ID is required.");

            // Cancel subscription (returns void)
            await _subscriptionClient.CancelSubscriptionAsync(customerId, subscriptionId);
        }

        // ---------------- Helper Methods ----------------
        private string GetMolliePaymentMethodType(string paymentMethod)
        {
            return paymentMethod switch
            {
                "Card" => Mollie.Api.Models.Payment.PaymentMethod.CreditCard,
                "ApplePay" => Mollie.Api.Models.Payment.PaymentMethod.ApplePay,
                "PayPal" => Mollie.Api.Models.Payment.PaymentMethod.PayPal,
                _ => ""
            };
        }

        private string GetMollieCurrencyType(string currency)
        {
            return currency switch
            {
                "Euro(€)" => "EUR",
                "USD($)" => "USD",
                "Australian dollar" => "AUD",
                "Canadian dollar" => "CAD",
                "Swiss franc" => "CHF",
                "British pound" => "GBP",
                _ => "EUR"
            };
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