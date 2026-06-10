namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.Messaging;
    using QrSortable.Components.CoreFeatures.Assistant;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Views;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.CoreFeatures.Settings.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.Helper;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class RootViewModel : BaseViewModel, IRecipient<AppResumedMessage>
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly IBackendSynchronizationManager _backendSynchronizationManager;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;
        private readonly IStorageVoiceAssistantService _voiceAssistantService;
        private readonly IStorageFinderService _storageFinderService;
        private readonly IVersionCheckService _versionCheckService;
        private readonly ISharedMethodService _sharedMethodService;

        private bool _isInitializeVisible = false;

        // Prevents duplicate sync calls if resume fires multiple times quickly
        private bool _isSyncing = false;

        /// <summary>
        ///     Gets or sets the collection of categories available in the system.
        /// </summary>
        public ObservableCollection<StorageGroup> Categories { get; set; }

        /// <summary>
        ///     Gets or sets the collection of search categories available in the system.
        /// </summary>
        public ObservableCollection<StorageGroup> SearchCategories { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootViewModel" />.
        /// </summary>
        public RootViewModel(
            IDatabaseManager databaseManager,
            IToastService toastService,
            IBackendSynchronizationManager backendSynchronizationManager,
            IGeneralInformationManager generalInformationManager,
            IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager,
            IStorageVoiceAssistantService voiceAssistantService,
            IStorageFinderService storageFinderService,
            IVersionCheckService versionCheckService,
            ISharedMethodService sharedMethodService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _backendSynchronizationManager = backendSynchronizationManager;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _voiceAssistantService = voiceAssistantService;
            _storageFinderService = storageFinderService;
            _versionCheckService = versionCheckService;
            _generalInformationManager = generalInformationManager;
            _sharedMethodService = sharedMethodService;

            Categories = new ObservableCollection<StorageGroup>();
            SearchCategories = new ObservableCollection<StorageGroup>();

            // Register to receive app resume messages sent from App.xaml.cs
            WeakReferenceMessenger.Default.Register(this);
            
        }

        /// <summary>
        /// Called automatically when the app resumes from background (sent by App.xaml.cs).
        /// Re-runs backend sync so data is always fresh on app foregrounding.
        /// </summary>
        public async void Receive(AppResumedMessage message)
        {
            await RunBackendSyncAsync();
        }

        /// <summary>
        /// Initializes the component asynchronously on first cold start.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            try
            {
                if (!await _versionCheckService.IsUsingLatestVersion())
                {
                    var confirmMessage = await DialogService.ShowRequestDialog(
                        AppResources.RootViewModel_UpdateAvailableText,
                        AppResources.RootViewModel_LaterText,
                        AppResources.RootViewModel_UpdateText);

                    if (confirmMessage)
                    {
                        await _versionCheckService.OpenAppInStore();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RootViewModel: Version check failed: {ex.Message}");
            }


            bool isFirstRun = !Preferences.ContainsKey("RootViewInitialized");

            if (isFirstRun)
            {
                Preferences.Set("RootViewInitialized", true);

                var confirm = await DialogService.ShowRequestDialog( AppResources.RootViewModel_InformationForUseText,
                     AppResources.General_NoText, AppResources.General_YesText);
                if (confirm)
                {
#if IOS
                    await NavigationService.Navigate<HelpView>();
#else
                    if (!await _sharedMethodService.OpenUserManualAsync())
                    {
                        await DialogService.ShowAlertDialog(
                       AppResources.Dialog_Error, AppResources.General_FileNotFoundErrorText, AppResources.Dialog_OK_Text);
                    }
#endif

                }
            }

            // Run full backend sync on cold start
            await RunBackendSyncAsync();

            _isInitializeVisible = true;

            bool outcome = false;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                outcome = (bool)await DialogService.ShowActivityIndicatorAndReturnResult(
                    AppResources.General_LoadingText,
                    async () => await LoadCategoryAsync()
                );
            });

            if (!outcome)
            {
                await _toastService.DisplayToast(AppResources.RootViewModel_NoStorageFoundText);
            }
        }

        /// <summary>
        /// Centralized method that runs all backend and data sync operations.
        /// Safe to call from both InitializeAsync (cold start) and Receive (resume).
        /// </summary>
        private async Task RunBackendSyncAsync()
        {
            // Guard against concurrent sync calls
            if (_isSyncing) return;

            _isSyncing = true;

            try
            {
                await _backendSynchronizationManager.SynchronizeStoredObjectsAsync();
                await _generalDatabaseSynchronizationManager.SynchronizeAppDataAsync();

                var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
                await _generalDatabaseSynchronizationManager
                    .SyncSubscriptionFromFirebaseAsync(generalInfo.MultiUserId);

                // Reload categories after sync so UI reflects latest data
                if (_isInitializeVisible)
                {
                    await LoadCategoryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RootViewModel: Backend sync failed: {ex.Message}");
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Represents the currently add to basket count in the application.
        /// </summary>
        [ObservableProperty]
        private string _basketCount;

        /// <summary>
        /// Represents the currently visible of the refresh button in the application.
        /// </summary>
        [ObservableProperty]
        private bool _refreshBtnVisible;

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            var backendUsed = (await _generalInformationManager.GetGeneralInformationAsync()).IsBackendUsed;
            RefreshBtnVisible = backendUsed;

            if (!_isInitializeVisible)
            {
                await LoadCategoryAsync();
            }

            try
            {
                var basketData = await _databaseManager.GetListAsync<AddToBasketData>();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    BasketCount = basketData != null && basketData.Count > 0
                        ? basketData.Count.ToString()
                        : "0";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RootViewModel: Error loading basket: {ex.Message}");
            }
        }

        public AsyncRelayCommand QrScanCommand => new AsyncRelayCommand(async () =>
        {
            _isInitializeVisible = false;
            await NavigationService.Navigate<QrBrScannerView>();
        });

        public AsyncRelayCommand CodeGeneratorCommand => new AsyncRelayCommand(async () =>
        {
            _isInitializeVisible = false;
            await NavigationService.Navigate<SelectProductView>();
        });

        public AsyncRelayCommand RefreshCommand => new AsyncRelayCommand(async () =>
        {
            await DialogService.ShowActivityIndicatorAndReturnResult(
                AppResources.RootViewModel_SyncText,
                async () =>
                {
                    await RunBackendSyncAsync(); 
                    return true;
                });
        });

        public AsyncRelayCommand MenuCommand => new AsyncRelayCommand(async () =>
        {
            _isInitializeVisible = false;
            await NavigationService.Navigate<MenuView>();
        });

        private async Task<bool> LoadCategoryAsync()
        {
            try
            {
                var storageList = await _databaseManager.GetListAsync<StorageEntry>();

                if (storageList == null || !storageList.Any())
                {
                    await MainThread.InvokeOnMainThreadAsync(() => Categories.Clear());
                    return false;
                }

                var grouped = storageList
                    .GroupBy(x => string.IsNullOrEmpty(x.Category) ? "Uncategorized" : x.Category)
                    .Select(g =>
                    {
                        var uniqueItems = g.GroupBy(x => x.BarcodeValue).Select(x => x.First()).ToList();
                        var group = new StorageGroup
                        {
                            Category = g.Key,
                            Items = new ObservableCollection<StorageEntry>(uniqueItems)
                        };
                        const int pageSize = 20;
                        foreach (var item in uniqueItems.Take(pageSize))
                        {
                            group.VisibleItems.Add(item);
                        }
                        group.LoadedItemCount = group.VisibleItems.Count;
                        return group;
                    });

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Categories.Clear();
                    foreach (var group in grouped)
                    {
                        Categories.Add(group);
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RootViewModel: Error loading categories: {ex.Message}");
                return false;
            }
        }

        //--------------------------------------------------Search View---------------------------------------

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _searchVisible;

        public AsyncRelayCommand SearchCommand => new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await DialogService.ShowAlertDialog(AppResources.Dialog_InformationText,
                    AppResources.General_FillAllField, AppResources.Dialog_OK_Text);
                return;
            }

            SearchVisible = true;

            try
            {
                var results = await _storageFinderService.FindGenericAsync(SearchText.Trim());

                if (results == null || !results.Any())
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        SearchCategories.Clear();
                        SearchText = string.Empty;
                        SearchVisible = false;
                        _toastService.DisplayToast(AppResources.RootViewModel_NoResultFoundText);
                    });
                    return;
                }

                await DialogService.ShowActivityIndicatorAndReturnResult(AppResources.General_LoadingText, async () =>
                {
                    var grouped = results
                        .GroupBy(x => string.IsNullOrEmpty(x.Category) ? "Uncategorized" : x.Category)
                        .Select(g =>
                        {
                            var uniqueItems = g.GroupBy(x => x.BarcodeValue).Select(x => x.First()).ToList();
                            var group = new StorageGroup
                            {
                                Category = g.Key,
                                Items = new ObservableCollection<StorageEntry>(uniqueItems)
                            };
                            const int pageSize = 20;
                            foreach (var item in uniqueItems.Take(pageSize))
                            {
                                group.VisibleItems.Add(item);
                            }
                            group.LoadedItemCount = group.VisibleItems.Count;
                            return group;
                        });

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        SearchCategories.Clear();
                        foreach (var group in grouped)
                        {
                            SearchCategories.Add(group);
                        }
                    });

                    return true;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RootViewModel: SearchCommand error: {ex.Message}");
            }
        });

        public AsyncRelayCommand SearchCloseCommand => new AsyncRelayCommand(async () =>
        {
            SearchText = string.Empty;
            SearchVisible = false;
        });
    }
}