namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using Mollie.Api.Models.Payment.Response;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Constants;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Notification;

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IMollieService _mollieService;
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly ISharedMethodService _sharedMethodService;
        private readonly IConnectivityService _connectivityService;
        private readonly IBackendDatabaseHelper _backendDatabaseHelper;

        private SubscriptionEntity? _subscription;

        public bool IsSubscribed => _subscription?.IsSubscribed ?? false;

        public SubscriptionService(IMollieService mollieService, IDatabaseManager databaseManager,
            IToastService toastService, IBackendCommunicationService backendCommunicationService,
            IConnectivityService connectivityService, IBackendDatabaseHelper backendDatabaseHelper,
           ISharedMethodService sharedMethodService)
        {
            _mollieService = mollieService;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _backendCommunicationService = backendCommunicationService;
            _connectivityService = connectivityService;
            _backendDatabaseHelper = backendDatabaseHelper;
            _sharedMethodService = sharedMethodService;
        }

        public async Task LoadAsync()
        {
            var list = await _databaseManager.GetListAsync<SubscriptionEntity>();

            _subscription = list?.FirstOrDefault();

            if (_subscription == null)
            {
                _subscription = new SubscriptionEntity
                {
                    IsSubscribed = false,
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = string.Empty,
                    SubscriptionId = string.Empty,
                    Email = string.Empty
                };

            }
        }

        public async Task<PaymentResponse> CreateInitialSubscriptionPaymentAsync(
            string email, string currency, decimal amount)
        {
            return await _mollieService.CreatePaymentAsync(
                amount, currency, "Card", PaymentConstants.SubscriptionVmDeeplinkId, email);
        }

        public async Task FinalizeSubscriptionAsync(string email, string currency, decimal amount)
        {
            if (_subscription?.IsSubscribed == true)
            {
                Console.WriteLine(
                    "[SubscriptionService] Already subscribed locally, skipping finalization.");
                return;
            }

            var subscription = await _mollieService.CreateSubscriptionAsync(
                amount, currency, email, "QrSortable Premium");

            _subscription ??= new SubscriptionEntity();

            _subscription.IsSubscribed = true;
            _subscription.CreatedAt = DateTime.UtcNow;
            _subscription.CustomerId = subscription.CustomerId ?? string.Empty;
            _subscription.SubscriptionId = subscription.Id ?? string.Empty;
            _subscription.Email = email ?? string.Empty;

            if (_subscription.ID == 0)
            {
                SaveToTheDatabaseAndSendToBackendAsync(_subscription);
            }
            else
            {
                UpdateDatabaseAndSubscriptionInBackendAsync(_subscription);
            }

        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
        {
            return await _mollieService.GetPaymentStatusAsync(paymentId);
        }

        public async Task CancelSubscriptionAsync()
        {
            if (_subscription == null) { return; }

            if (!string.IsNullOrWhiteSpace(_subscription.CustomerId) &&
                !string.IsNullOrWhiteSpace(_subscription.SubscriptionId))
            {
                await _mollieService.CancelSubscriptionAsync(_subscription.CustomerId, _subscription.SubscriptionId);
            }

            _subscription.IsSubscribed = false;

            UpdateDatabaseAndSubscriptionInBackendAsync(_subscription);
        }

        private async void SaveToTheDatabaseAndSendToBackendAsync(DatabaseEntry backendData)
        {
            _databaseManager.BeginTransaction();
            var addedItem = await _databaseManager.AddAsync(backendData);
            if (addedItem != null)
            {
                _databaseManager.CommitTransaction();

                if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                {
                    var dto = _backendDatabaseHelper.CreatDtoSubscriptionBackendData(backendData, "false");
                    _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                }
                else
                {
                    await _backendCommunicationService.InsertAsync(backendData);
                }
                return;
            }
            else
            {
                _databaseManager.Rollback();
                return;
            }
        }

        private async void UpdateDatabaseAndSubscriptionInBackendAsync(DatabaseEntry backendData)
        {
            var updatedItem = await _databaseManager.UpdateAsync(backendData);

            if (updatedItem != null) 
            {
                if (!await _connectivityService.CheckInternetConnectionAvailableAsync())
                {
                    var dto = _backendDatabaseHelper.CreatDtoSubscriptionBackendData(backendData, "true");
                    _backendDatabaseHelper.SaveToTheBackendAsync(dto);
                }
                else
                {
                    await _backendCommunicationService.UpdateAsync(backendData);
                }
                
            }
        }
    }
}