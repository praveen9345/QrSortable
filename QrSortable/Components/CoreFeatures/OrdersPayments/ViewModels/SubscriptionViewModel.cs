namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.TimeHandling;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.Collections.ObjectModel;

    public partial class SubscriptionViewModel : BaseViewModel<bool>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITimerService _timerService;
        private readonly IDatabaseManager _databaseManager;
        private readonly IMauiEssentialsWrapper _mauiWrapper;
        private readonly IGeneralInformationManager _generalInformationManager;

        private string _paymentId;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _subscriptionProcessed;
        private string _email;
        private bool _isFromOnboarding;
        private bool _isChecking;

        private const string PrefKeyPaymentId = "paymentId";

        public SubscriptionViewModel(ISubscriptionService subscriptionService, ITimerService timerService,
            IMauiEssentialsWrapper mauiWrapper, IDatabaseManager databaseManager,
            IGeneralInformationManager generalInformationManager)
        {
            _subscriptionService = subscriptionService;
            _timerService = timerService;
            _mauiWrapper = mauiWrapper;
            _databaseManager = databaseManager;
            _generalInformationManager = generalInformationManager;

            PremiumFeatures = new ObservableCollection<string>
        {
            AppResources.SubscriptionViewModel_SharingMsgText,
            AppResources.SubscriptionViewModel_UnlimitedText,
            AppResources.SubscriptionViewModel_CloudBackText,
            AppResources.SubscriptionViewModel_MoveItemText,
            AppResources.SubscriptionViewModel_MultipleImagesText,
            AppResources.SubscriptionViewModel_AdvertisementsText
        };

            SelectedPlan = SubscriptionPlan.Monthly;
            SelectedCurrencyItem = CurrencyItem[0];

            _ = LoadStateAsync();
        }

        #region Properties

        //[ObservableProperty] private string _deeplinkId;
        [ObservableProperty] private bool _isSubscribed;
        [ObservableProperty] private string _subscriptionStatusText;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _selectedCurrencyItem;
        [ObservableProperty] private string _customerEmail;
        [ObservableProperty] private string _priceText;
        [ObservableProperty] private SubscriptionPlan _selectedPlan;

        public ObservableCollection<string> PremiumFeatures { get; }

        public ObservableCollection<string> CurrencyItem { get; } = new ObservableCollection<string>
    {
        "Euro(€)", "USD($)", "GBP(£)"
    };

        public override async void Prepare(bool isFromOnboarding)
        {
            _isFromOnboarding = isFromOnboarding;
        }

        #endregion

        // ──────────────────────────────────────────────────────────────────
        // State loading
        // ──────────────────────────────────────────────────────────────────

        private async Task LoadStateAsync()
        {
            try
            {
                await _subscriptionService.LoadAsync();

                var subscription = (await _databaseManager.GetListAsync<SubscriptionEntity>())?.FirstOrDefault();
                _email = subscription?.Email ?? string.Empty;

                await UpdateState();
                UpdatePrice();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SubscriptionViewModel LoadStateAsync = " + ex);
            }
        }

        private async Task UpdateState()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                IsSubscribed = _subscriptionService.IsSubscribed;

                SubscriptionStatusText = IsSubscribed
                    ? AppResources.SubscriptionViewModel_PremiumActiveMsg
                    : AppResources.SubscriptionViewModel_UnlockFeaturesText;

                CustomerEmail = IsSubscribed ? _email : string.Empty;
            });

            if (_subscriptionService.IsSubscribed)
                await _generalInformationManager.UpdateIsBackendUsedAsync(true);
        }

        private void UpdatePrice()
        {
            PriceText = SelectedPlan switch
            {
                SubscriptionPlan.Monthly => SelectedCurrencyItem switch
                {
                    "Euro(€)" => AppResources.SubscriptionViewModel_MonthSelectedEuroCurrencyText,
                    "USD($)" => AppResources.SubscriptionViewModel_MonthSelectedDollerCurrencyText,
                    "GBP(£)" => AppResources.SubscriptionViewModel_MonthSelectedPoundCurrencyText,
                    _ => AppResources.SubscriptionViewModel_MonthSelectedEuroCurrencyText
                },
                SubscriptionPlan.Yearly => SelectedCurrencyItem switch
                {
                    "Euro(€)" => "€49.99 / year (Save 20%)",
                    "USD($)" => "$49.99 / year (Save 20%)",
                    "GBP(£)" => "£49.99 / year (Save 20%)",
                    _ => "€49.99 / year (Save 20%)"
                },
                _ => AppResources.SubscriptionViewModel_MonthSelectedEuroCurrencyText
            };
        }

        // ──────────────────────────────────────────────────────────────────
        // Polling
        // ──────────────────────────────────────────────────────────────────

        private void PrepareNewPayment(string paymentId)
        {
            StopTimer();

            _paymentId = paymentId;

            lock (_lock)
            {
                _subscriptionProcessed = false;
                _isChecking = false;
            }

            SubscriptionStatusText = AppResources.SubscriptionViewModel_WaitingPaymentText;

            StartPolling();
        }

        private void StartPolling()
        {
            _timer = _timerService.StartPeriodicTimer(async _ =>
            {
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(CheckPaymentStatusAsync);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Polling tick error: " + ex);
                }
            }, TimeSpan.FromSeconds(10));
        }

        // ──────────────────────────────────────────────────────────────────
        // Subscribe command
        // ──────────────────────────────────────────────────────────────────

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
                    AppResources.Dialog_Error,
                    AppResources.SubscriptionViewModel_EnterEmailText,
                    AppResources.Dialog_OK_Text);

                IsBusy = false;
                return;
            }

            try
            {
                var payment = await _subscriptionService.CreateInitialSubscriptionPaymentAsync(
                    CustomerEmail, SelectedCurrencyItem, 4.99m);

                if (payment?.Links?.Checkout?.Href == null)
                {
                    await DialogService.ShowAlertDialog(
                        AppResources.Dialog_Error,
                        AppResources.SubscriptionViewModel_FailedPaymentText,
                        AppResources.Dialog_OK_Text);
                    return;
                }

                Preferences.Set(PrefKeyPaymentId, payment.Id);
                PrepareNewPayment(GetSavedPaymentId());

                var browserMode = (_mauiWrapper.GetDevicePlatform() == _mauiWrapper.AndroidDevicePlatform)
                    ? BrowserLaunchMode.SystemPreferred
                    : BrowserLaunchMode.External;

                await Browser.Default.OpenAsync(payment.Links.Checkout.Href, browserMode);
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertDialog(
                    AppResources.Dialog_Error, ex.Message, AppResources.Dialog_OK_Text);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // Deeplink / redirect handling
        // ──────────────────────────────────────────────────────────────────
        public async Task HandleMollieRedirect()
        {
            var paymentId = GetSavedPaymentId();
            if (string.IsNullOrEmpty(paymentId)) return;

            StopTimer();
            _paymentId = paymentId;

            lock (_lock)
            {
                _subscriptionProcessed = false;
                _isChecking = false;
            }

            SubscriptionStatusText = AppResources.SubscriptionViewModel_WaitingPaymentText;

            await CheckPaymentStatusAsync();

            if (!_subscriptionProcessed)
                StartPolling();
        }

        // ──────────────────────────────────────────────────────────────────
        // Core payment check — called both from the timer and from deeplink
        // ──────────────────────────────────────────────────────────────────

        private async Task CheckPaymentStatusAsync()
        {
            if (string.IsNullOrEmpty(_paymentId)) return;

            lock (_lock)
            {
                if (_isChecking) return;
                _isChecking = true;
            }

            string paymentStatus = null;

            try
            {
                await DialogService.ShowActivityIndicatorAndReturnResult(
                    AppResources.SubscriptionViewModel_VerifyingPaymentText,
                    async () =>
                    {
                        try
                        {
                            var response = await _subscriptionService.GetPaymentStatusAsync(_paymentId);
                            if (response == null) return false;

                            paymentStatus = response.Status;

                            switch (response.Status)
                            {
                                case "paid":
                                    {
                                        bool alreadyProcessed;
                                        lock (_lock)
                                        {
                                            alreadyProcessed = _subscriptionProcessed;
                                            if (!alreadyProcessed)
                                                _subscriptionProcessed = true;
                                        }

                                        StopTimer();

                                        if (!alreadyProcessed)
                                        {
                                            await _subscriptionService.FinalizeSubscriptionAsync(
                                                CustomerEmail, SelectedCurrencyItem, 4.99m);

                                            await _subscriptionService.LoadAsync();

                                            var sub = (await _databaseManager.GetListAsync<SubscriptionEntity>())
                                                ?.FirstOrDefault();
                                            _email = sub?.Email ?? CustomerEmail;
                                        }
                                        return true;
                                    }

                                case "open":
                                case "pending":
                                    return false;

                                case "canceled":
                                case "failed":
                                case "expired":
                                    StopTimer();
                                    return false;

                                default:
                                    return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("CheckPaymentStatusAsync inner: " + ex);
                            return false;
                        }
                    }
                );
            }
            finally
            {
                lock (_lock)
                {
                    _isChecking = false;
                }
            }

            switch (paymentStatus)
            {
                case "paid":
                    {
                        await UpdateState();

                        await DialogService.ShowAlertDialog(
                            AppResources.General_SucessText,
                            AppResources.SubscriptionViewModel_ActivePremiumText,
                            AppResources.Dialog_OK_Text);

                        if (_isFromOnboarding)
                        {
                            await _generalInformationManager.UpdateOnboardingProgressAsync(
                                OnboardingProgress.OnboardingCompleted);
                            await NavigationService.Navigate<RootView>();
                        }
                        break;
                    }

                case "canceled":
                case "failed":
                case "expired":
                    // Update status text on main thread.
                    SubscriptionStatusText = AppResources.General_FailedText;
                    break;

                    // "open" / "pending" / null — timer keeps running, nothing to show.
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // Cancel subscription
        // ──────────────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task CancelSubscription()
        {
            await _subscriptionService.CancelSubscriptionAsync();

            lock (_lock)
            {
                _subscriptionProcessed = false;
            }

            await UpdateState();

            await DialogService.ShowAlertDialog(
                AppResources.Dialog_InformationText,
                AppResources.SubscriptionViewModel_SubscriptiontCancelText,
                AppResources.Dialog_OK_Text);
        }

        // ──────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timerService.StopPeriodicTimer(_timer);
                _timer.Dispose();
                _timer = null;
            }
        }

        private string GetSavedPaymentId() =>
            Preferences.Get(PrefKeyPaymentId, string.Empty);

        public enum SubscriptionPlan { Monthly, Yearly }
    }
}