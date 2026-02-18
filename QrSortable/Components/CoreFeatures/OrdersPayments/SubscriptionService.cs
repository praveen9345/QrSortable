using Microsoft.Maui.Storage;
using Mollie.Api.Models.Payment.Response;
using QrSortable.Components.CoreFeatures.DataManagement.General;
using QrSortable.Components.CoreFeatures.OrdersPayments;

public class SubscriptionService : ISubscriptionService
{
    private const string SubscriptionKey = "is_subscribed";
    private const string CustomerIdKey = "mollie_customer_id";
    private const string SubscriptionIdKey = "mollie_subscription_id";


    private readonly IMollieService _mollieService;
    private readonly IDatabaseManager _databaseManager;

    public bool IsSubscribed { get; private set; }

    public SubscriptionService(IMollieService mollieService, IDatabaseManager databaseManager)
    {
        _mollieService = mollieService;
        _databaseManager = databaseManager;
    }

    public async Task LoadAsync()
    {
        IsSubscribed = Preferences.Get(SubscriptionKey, false);
        await Task.CompletedTask;
    }

    public async Task<PaymentResponse> CreateInitialSubscriptionPaymentAsync(
        string email,string currency, decimal amount)
    {
        return await _mollieService.CreatePaymentAsync( amount,currency,
            "Card", "Initial Payment for Subscription",email);
    }

    public async Task FinalizeSubscriptionAsync( string email,string currency, decimal amount)
    {
        var subscription = await _mollieService.CreateSubscriptionAsync(
            amount,currency, email,"QrSortable Premium");

        Preferences.Set(SubscriptionKey, true);
        Preferences.Set(CustomerIdKey, subscription.CustomerId);
        Preferences.Set(SubscriptionIdKey, subscription.Id);

        IsSubscribed = true;
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        return await _mollieService.GetPaymentStatusAsync(paymentId);
    }

    public async Task CancelSubscriptionAsync()
    {
        string customerId = Preferences.Get(CustomerIdKey, string.Empty);
        string subscriptionId = Preferences.Get(SubscriptionIdKey, string.Empty);

        if (!string.IsNullOrWhiteSpace(customerId) &&
            !string.IsNullOrWhiteSpace(subscriptionId))
        {
            await _mollieService.CancelSubscriptionAsync(customerId, subscriptionId);
        }

        Preferences.Set(SubscriptionKey, false);
        Preferences.Remove(CustomerIdKey);
        Preferences.Remove(SubscriptionIdKey);

        IsSubscribed = false;
    }
}