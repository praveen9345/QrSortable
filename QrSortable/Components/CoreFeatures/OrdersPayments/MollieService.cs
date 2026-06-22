namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using Mollie.Api.Client;
    using Mollie.Api.Models;
    using Mollie.Api.Models.Customer.Request;
    using Mollie.Api.Models.Payment;
    using Mollie.Api.Models.Payment.Request;
    using Mollie.Api.Models.Payment.Response;
    using Mollie.Api.Models.Subscription.Request;
    using Mollie.Api.Models.Subscription.Response;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Constants;
    using System.Globalization;

    public class MollieService : IMollieService
    {
        private readonly PaymentClient _paymentClient;
        private readonly CustomerClient _customerClient;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly MandateClient _mandateClient;

        //private static readonly string MOLLIE_TEST_API_KEY = "live_uJFHu2bBnBtMcdSW6Aa8TxjvJpGnHJ";
        private static readonly string MOLLIE_TEST_API_KEY = "test_tz55vhktCjqqtdJcRW3afauehgqDbS";

        public MollieService()
        {
            _paymentClient = new PaymentClient(MOLLIE_TEST_API_KEY);
            _customerClient = new CustomerClient(MOLLIE_TEST_API_KEY);
            _subscriptionClient = new SubscriptionClient(MOLLIE_TEST_API_KEY);
            _mandateClient = new MandateClient(MOLLIE_TEST_API_KEY);
        }

        // --------------- One-time Payment ----------------
        public async Task<PaymentResponse> CreatePaymentAsync(decimal amount, string currency, string paymentMethod, string description, string customerEmail = null)
        {
            string customerId = null;
            if (!string.IsNullOrWhiteSpace(customerEmail))
                customerId = await GetOrCreateCustomerIdAsync(customerEmail);


            string? deeplinkId = null;

            if (description == PaymentConstants.PaymentShipmentVmDeeplinkId) 
            {
                deeplinkId = PaymentConstants.PaymentShipmentVmDeeplinkId;
            }
            else if (description == PaymentConstants.SubscriptionVmDeeplinkId)
            {
                deeplinkId = PaymentConstants.SubscriptionVmDeeplinkId;
            }
            

            var paymentRequest = new PaymentRequest
            {
                Amount = new Amount(GetMollieCurrencyType(currency), ToMollieAmount(amount)),
                Description = description,
                RedirectUrl = $"https://www.qrsortable.com/#/mollie-return?id={deeplinkId}",
                Method = GetMolliePaymentMethodType(paymentMethod),
                CustomerId = customerId,
                SequenceType = SequenceType.First
            };

            return await _paymentClient.CreatePaymentAsync(paymentRequest);
        }


        // --------------- Subscription ----------------
        public async Task<SubscriptionResponse> CreateSubscriptionAsync(
         decimal amount,string currency,string customerEmail,string description)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("Customer email required.");

            var customerId = await GetOrCreateCustomerIdAsync(customerEmail);

            // ✅ Check for an existing active subscription first
            var existingSubscription = await GetActiveSubscriptionAsync(customerId);
            if (existingSubscription != null)
            {
                Console.WriteLine(
                    $"[MollieService] Subscription already exists: {existingSubscription.Id}");
                return existingSubscription;
            }

            // OPTIONAL: verify valid mandate exists
            bool hasMandate = await HasValidMandateAsync(customerId);

            if (!hasMandate)
                throw new Exception(
                    "No valid mandate found. Initial payment must be completed first.");

            var request = new SubscriptionRequest
            {
                Amount = new Amount(GetMollieCurrencyType(currency),
                ToMollieAmount(amount)),
                Interval = "1 month",
                Description = description,
                StartDate = DateTime.UtcNow.Date
            };

            return await _subscriptionClient.CreateSubscriptionAsync(customerId, request);
        }


        public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
        {
            return await _paymentClient.GetPaymentAsync(paymentId);
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

        private async Task<string> GetOrCreateCustomerIdAsync(string email)
        {
            var customerList = await _customerClient.GetCustomerListAsync();

            var existingCustomer = customerList.Items
                .FirstOrDefault(c =>
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (existingCustomer != null)
                return existingCustomer.Id;

            var newCustomer = await _customerClient.CreateCustomerAsync(
                new CustomerRequest
                {
                    Email = email
                });

            return newCustomer.Id;
        }


        private string GetMolliePaymentMethodType(string paymentMethod)
        {
            return paymentMethod switch
            {
                "Card" => PaymentMethod.CreditCard,
                "ApplePay" => PaymentMethod.ApplePay,
                "PayPal" => PaymentMethod.PayPal,
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
                "GBP(£)" => "GBP", //british pound
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

        private async Task<bool> HasValidMandateAsync(string customerId)
        {
            var mandates =
                await _mandateClient.GetMandateListAsync(customerId);

            return mandates.Items.Any(m => m.Status == "valid");
        }

        private async Task<SubscriptionResponse?> GetActiveSubscriptionAsync(string customerId)
        {
            try
            {
                var subscriptions = await _subscriptionClient.GetSubscriptionListAsync(customerId);
                return subscriptions?.Items?
                    .FirstOrDefault(s => s.Status == "active" || s.Status == "pending");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[MollieService] GetActiveSubscriptionAsync failed: {ex}");
                return null;
            }
        }
    }
}