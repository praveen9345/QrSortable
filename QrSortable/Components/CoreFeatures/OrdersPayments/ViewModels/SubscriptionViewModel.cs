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
        
        _ = LoadStateAsync();
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

    private async Task LoadStateAsync()
    {
        try
        {
            await _subscriptionService.LoadAsync();
            UpdateState();
            UpdatePrice();
        }
        catch (Exception ex)
        {
            Console.WriteLine("SubscriptionViewModel ="+ex);
        }
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
                SelectedCurrencyItem == "Euro(€)"
                    ? "€4.99 / month"
                    : "$4.99 / month",

            SubscriptionPlan.Yearly =>
                SelectedCurrencyItem == "Euro(€)"
                    ? "€49.99 / year (Save 20%)"
                    : "$49.99 / year (Save 20%)",

            _ =>
                SelectedCurrencyItem == "Euro(€)"
                    ? "€4.99 / month"
                    : "$4.99 / month"
        };
    }


    #region ⭐ SAFEST PATTERN

    private void PrepareNewPayment(string paymentId)
    {
        StopTimer();

        _paymentId = paymentId;

        lock (_lock)
        {
            _subscriptionProcessed = false;
        }

        SubscriptionStatusText = "Waiting for payment…";

        StartPolling();
    }

    private void StartPolling()
    {
        _timer = _timerService.StartPeriodicTimer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await CheckPaymentStatusAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });
        }, TimeSpan.FromSeconds(10));
    }

    #endregion

    #region Subscribe 


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
                    CustomerEmail, SelectedCurrencyItem, 4.99m);

            if (payment?.Links?.Checkout?.Href == null)
            {
                await DialogService.ShowAlertDialog("Error", "Failed to create payment.", "OK");
                return;
            }

           PrepareNewPayment(payment.Id);

            var browserMode =
                (_mauiWrapper.GetDevicePlatform() == _mauiWrapper.AndroidDevicePlatform)
                    ? BrowserLaunchMode.SystemPreferred
                    : BrowserLaunchMode.External;

            await Browser.Default.OpenAsync(payment.Links.Checkout.Href, browserMode);
        }
        catch (Exception ex)
        {
            await DialogService.ShowAlertDialog("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    

    public async Task HandleMollieRedirect(string paymentId)
    {
        if (string.IsNullOrEmpty(paymentId))
            return;

        // reset state again (important)
        PrepareNewPayment(paymentId);

        await CheckPaymentStatusAsync();
    }

    private async Task CheckPaymentStatusAsync()
    {
        if (string.IsNullOrEmpty(_paymentId))
            return;

        PaymentResponse response;

        try
        {
            response = await _subscriptionService.GetPaymentStatusAsync(_paymentId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return;
        }

        if (response == null)
            return;

        switch (response.Status)
        {
            case "paid":
            {
                lock (_lock)
                {
                    if (_subscriptionProcessed)
                        return;

                    _subscriptionProcessed = true;
                }

                StopTimer();

                await _subscriptionService.FinalizeSubscriptionAsync(
                    CustomerEmail, SelectedCurrencyItem,4.99m);

                UpdateState();

                await DialogService.ShowAlertDialog(
                    "Success",
                    "Premium activated successfully 🎉",
                    "OK");

                break;
            }

            case "open":
            case "pending":
                // keep polling
                break;

            case "canceled":
            case "failed":
            case "expired":
            {
                StopTimer();

                SubscriptionStatusText = "Payment not completed";

                await DialogService.ShowAlertDialog(
                    "Payment not completed",
                    "Your payment was not completed. Please try again.",
                    "OK");

                break;
            }
        }
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
        lock (_lock)
        {
            _subscriptionProcessed = false;
        }

        UpdateState();

        await DialogService.ShowAlertDialog(
            "Cancelled",
            "Your subscription has been cancelled.",
            "OK");
    }

    public enum SubscriptionPlan { Monthly, Yearly }
}