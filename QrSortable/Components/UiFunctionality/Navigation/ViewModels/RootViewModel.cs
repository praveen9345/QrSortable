namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Assistant;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Views;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class RootViewModel : BaseViewModel
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;
        private readonly IBackendSynchronizationManager _backendSynchronizationManager;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly IGeneralDatabaseSynchronizationManager _generalDatabaseSynchronizationManager;
        private readonly IStorageVoiceAssistantService _voiceAssistantService;
        private readonly IStorageFinderService _storageFinderService;
        private readonly IVersionCheckService _versionCheckService;


        private bool _isInitializeVisible = false;

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
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public RootViewModel(IDatabaseManager databaseManager, IToastService toastService, 
            IBackendSynchronizationManager backendSynchronizationManager, IGeneralInformationManager generalInformationManager,
            IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager, 
            IStorageVoiceAssistantService voiceAssistantService, IStorageFinderService storageFinderService,
            IVersionCheckService versionCheckService)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _backendSynchronizationManager = backendSynchronizationManager;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;
            _voiceAssistantService = voiceAssistantService;
            _storageFinderService = storageFinderService;
            _versionCheckService = versionCheckService;

            Categories = new ObservableCollection<StorageGroup>();
            SearchCategories = new ObservableCollection<StorageGroup>();
            _generalInformationManager = generalInformationManager;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (!await _versionCheckService.IsUsingLatestVersion())
            {
                var confirmMessage = await DialogService.ShowRequestDialog(
                AppResources.RootViewModel_UpdateAvailableText,
                AppResources.RootViewModel_LaterText, AppResources.RootViewModel_UpdateText);

                if (confirmMessage)
                {
                    await _versionCheckService.OpenAppInStore();
                }
            }
                         
            //ensure backend sync
            await _backendSynchronizationManager.SynchronizeStoredObjectsAsync();

            //ensure all data sync
            await _generalDatabaseSynchronizationManager.SynchronizeAppDataAsync();
            
            var multiuserId = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
            await _generalDatabaseSynchronizationManager.SyncSubscriptionFromFirebaseAsync(multiuserId);

            _isInitializeVisible = true;

            bool outcome = false;

            // Ensure we run UI code on the main thread
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                outcome = (bool)await DialogService.ShowActivityIndicatorAndReturnResult(AppResources.General_LoadingText,
                   async () =>
                   {
                        return await LoadCategoryAsync();
                   }
                );
            });

            if (!outcome)
            {
                await _toastService.DisplayToast(AppResources.RootViewModel_NoStorageFoundText);
            }
        }

        /// <summary>
        /// Represents the currently add to basket countin the application.
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
            if (backendUsed) { RefreshBtnVisible = true; } else { RefreshBtnVisible = false; }
            
            if (!_isInitializeVisible)
            {
                var success = await LoadCategoryAsync();
                if (!success)
                {
                    await _toastService.DisplayToast(AppResources.RootViewModel_RefreshText);
                }
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
                Console.WriteLine($"RootViewModel:Error loading categories: {ex.Message}");
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
            await DialogService.ShowActivityIndicatorAndReturnResult(AppResources.RootViewModel_SyncText, async () =>
            {
                var result = await _generalDatabaseSynchronizationManager.SynchronizeAppDataAsync();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (result)
                    {
                        await _toastService.DisplayToast(AppResources.RootViewModel_DataSyncSuccessText);
                    }   
                });

                return result;
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

                // Group by category and remove duplicate BarcodeValues
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
                        // Load first batch
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
                
                await _toastService.DisplayToast($"{ex.Message}");
                return false;
            }
        }


        //--------------------------------------------------Search View---------------------------------------

        /// <summary>
        /// Represents the currently search text the application.
        /// </summary>
        [ObservableProperty]
        private string _searchText;

        /// <summary>
        /// Represents the currently visible of the search button in the application.
        /// </summary>
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
                         // Load first batch
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
                Console.WriteLine($"Rootview:SearchCommand: error message {ex.Message}");
            }
        });

        public AsyncRelayCommand SearchCloseCommand => new AsyncRelayCommand(async () =>
        {
            SearchText = string.Empty;
            SearchVisible = false;
        });
    }  
}
