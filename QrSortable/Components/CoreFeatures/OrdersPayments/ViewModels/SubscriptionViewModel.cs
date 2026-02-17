using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mollie.Api.Models.Payment.Response;
using QrSortable.Components.CoreFeatures.OrdersPayments;
using QrSortable.Components.PlatformUtils.Wrappers;
using QrSortable.Components.TimeHandling;
using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
using System.Collections.ObjectModel;

public partial class SubscriptionViewModel : BaseViewModel
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ITimerService _timerService;
    private readonly IMauiEssentialsWrapper _mauiWrapper;

    private string _paymentId;
    private Timer _timer;
    private readonly object _lock = new object();
    private bool _subscriptionProcessed;

    public SubscriptionViewModel( ISubscriptionService subscriptionService, ITimerService timerService,
        IMauiEssentialsWrapper mauiWrapper)
    {
        _subscriptionService = subscriptionService;
        _timerService = timerService;
        _mauiWrapper = mauiWrapper;

        PremiumFeatures = new ObservableCollection<string>
        {
            "Multi-User Sharing",
            "Unlimited Items per Box",
            "Cloud Backup & Sync",
            "Move Items Between Boxes",
            "Multiple Images per Item",
            "No Advertisements"
        };

        SelectedPlan = SubscriptionPlan.Monthly;


        SelectedCurrencyItem = CurrencyItem[0];
        LoadState();
    }

    #region Properties

    [ObservableProperty] 
    private bool _isSubscribed;

    [ObservableProperty] 
    private string _subscriptionStatusText;
    
    [ObservableProperty] 
    private bool _isBusy;

    [ObservableProperty] 
    private string _selectedCurrencyItem;

    [ObservableProperty] 
    private string _customerEmail;

    [ObservableProperty] 
    private string _priceText; 

    [ObservableProperty] 
    private SubscriptionPlan _selectedPlan;

    public ObservableCollection<string> PremiumFeatures { get; }

    public ObservableCollection<string> CurrencyItem { get; } =
        new ObservableCollection<string>
        {
            "Euro(€)",
            "USD($)"
        };

    #endregion

    private async void LoadState()
    {
        await _subscriptionService.LoadAsync();
        UpdateState();
        UpdatePrice();
    }

    private void UpdateState()
    {
        IsSubscribed = _subscriptionService.IsSubscribed;

        SubscriptionStatusText = IsSubscribed
            ? "🌟 Premium Active"
            : "Upgrade to unlock premium features";
    }

    private void UpdatePrice() 
    { 
        PriceText = SelectedPlan switch 
        { 
            SubscriptionPlan.Monthly =>
            SelectedCurrencyItem == "Euro(€)" ? "€4.99 / month" : "$4.99 / month", 
            SubscriptionPlan.Yearly => SelectedCurrencyItem == "Euro(€)" 
            ? "€49.99 / year (Save 20%)" : "$49.99 / year (Save 20%)",
            _ => SelectedCurrencyItem == "Euro(€)" ? "€4.99 / month" : "$4.99 / month" 
        }; 
    }

    #region Subscribe Flow


    public AsyncRelayCommand OnSelectionChangedCommand => new AsyncRelayCommand(async () =>
    {
        UpdatePrice();
    });

    [RelayCommand]
    private async Task Subscribe()
    {
        if (IsBusy) return;
        IsBusy = true;

        if (string.IsNullOrWhiteSpace(CustomerEmail))
        {
            await DialogService.ShowAlertDialog(
                "Error",
                "Please enter your email",
                "OK");

            IsBusy = false;
            return;
        }

        try
        {
            var payment =
                await _subscriptionService.CreateInitialSubscriptionPaymentAsync(
                    CustomerEmail,SelectedCurrencyItem, 4.99m);

            if (payment?.Links?.Checkout != null)
            {
                _paymentId = payment.Id;

                // Start polling
                _timer = _timerService.StartPeriodicTimer(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await CheckPaymentStatusAsync();
                    });
                }, TimeSpan.FromSeconds(10));

                var browserMode =
                    (_mauiWrapper.GetDevicePlatform() == _mauiWrapper.AndroidDevicePlatform)
                    ? BrowserLaunchMode.SystemPreferred
                    : BrowserLaunchMode.External;

                await Browser.Default.OpenAsync(payment.Links.Checkout.Href,browserMode);
            }
            else
            {
                await DialogService.ShowAlertDialog("Error",
                    "Failed to create payment.","OK");
            }
        }
        catch (Exception ex)
        {
            await DialogService.ShowAlertDialog("Error", ex.Message, "OK");
        }

        IsBusy = false;
    }

    public async Task HandleMollieRedirect(string paymentId)
    {
        if (string.IsNullOrEmpty(paymentId))
            return;

        _paymentId = paymentId;

        StopTimer();

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await CheckPaymentStatusAsync();
        });
    }

    private async Task CheckPaymentStatusAsync()
    {
        if (string.IsNullOrEmpty(_paymentId))
            return;

        PaymentResponse response =
            await _subscriptionService.GetPaymentStatusAsync(_paymentId);

        if (response.Status != "paid")
            return;

        lock (_lock)
        {
            if (_subscriptionProcessed)
                return;

            _subscriptionProcessed = true;
        }

        StopTimer();

        await _subscriptionService.FinalizeSubscriptionAsync( CustomerEmail, SelectedCurrencyItem,4.99m);

        UpdateState();

        await DialogService.ShowAlertDialog("Success",
            "Premium activated successfully 🎉","OK");
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timerService.StopPeriodicTimer(_timer);
            _timer.Dispose();
            _timer = null;
        }
    }

    #endregion

    [RelayCommand]
    private async Task CancelSubscription()
    {
        await _subscriptionService.CancelSubscriptionAsync();
        UpdateState();

        await DialogService.ShowAlertDialog(
            "Cancelled","Your subscription has been cancelled.","OK");
    }

    public enum SubscriptionPlan { Monthly, Yearly }
}