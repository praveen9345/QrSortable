namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Views;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
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

        private bool _isInitializeVisible = false;

        /// <summary>
        ///     Gets or sets the collection of categories available in the system.
        /// </summary>
        public ObservableCollection<StorageGroup> Categories { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public RootViewModel(IDatabaseManager databaseManager, IToastService toastService, 
            IBackendSynchronizationManager backendSynchronizationManager, IGeneralInformationManager generalInformationManager,
            IGeneralDatabaseSynchronizationManager generalDatabaseSynchronizationManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
            _toastService = toastService;
            _backendSynchronizationManager = backendSynchronizationManager;
            _generalDatabaseSynchronizationManager = generalDatabaseSynchronizationManager;

            Categories = new ObservableCollection<StorageGroup>();
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

            await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.OnboardingCompleted);
            
            //ensure backend sync
            await _backendSynchronizationManager.SynchronizeStoredObjectsAsync();

            _isInitializeVisible = true;

            bool outcome = false;

            // Ensure we run UI code on the main thread
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                outcome = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
                   async () =>
                   {
                        //ensure all data sync
                        await _generalDatabaseSynchronizationManager.SynchronizeAppDataAsync();
                        return await LoadCategoryAsync();
                   }
                );
            });

            if (!outcome)
            {
                await _toastService.DisplayToast("Failed to load categories or no storage entries found.");
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
                    await _toastService.DisplayToast("Failed to refresh categories.");
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

            //await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
            // async () =>
            //    {
            //      await Task.Delay(1000);
            //      return true;
            //    });

            //var wasCodeSuccessfullyUpdated = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
            //   async () =>
            //       {
            //           await Task.Delay(1000);
            //           return true;
            //       });

            //await DialogService.ShowAlertDialog("error",
            //        "Something went wrong", "lökjyajflj");

            //var isDialogConfirmed = await DialogService.ShowRequestDialog(
            //   "lksajdlk",
            //   "ökdsf",
            //   "Cancel",
            //  "OK");

            //await NavigationService.Navigate<BoxDetailView>();

            //var sample = new SampleModel
            //{
            //    Name = "kumar",
            //    Description = "Sample Description"
            //};

            //await _backendCommunicationService.InsertSampleModel(sample);

            //var userId = await _backendCommunicationService.GetUserUIDByName("Sample");
        });

        public AsyncRelayCommand CodeGeneratorCommand => new AsyncRelayCommand(async () =>
        {
            _isInitializeVisible = false;
            await NavigationService.Navigate<SelectProductView>();

        });

        public AsyncRelayCommand RefreshCommand => new AsyncRelayCommand(async () =>
        {
            await DialogService.ShowActivityIndicatorAndReturnResult("Synchronizing...", async () =>
            {
                var result = await _generalDatabaseSynchronizationManager.SynchronizeAppDataAsync();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (result)
                    {
                        await _toastService.DisplayToast("Data synchronized successfully.");
                    }
                    else
                    {
                        await _toastService.DisplayToast("Data synchronization failed or no internet connection. Try again later.");
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
                            group.VisibleItems.Add(item);
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
                
                await _toastService.DisplayToast($"Error loading categories: {ex.Message}");
                return false;
            }
        }
    }  
}