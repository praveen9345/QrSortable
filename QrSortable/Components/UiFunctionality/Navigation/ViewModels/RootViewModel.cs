namespace QrSortable.Components.UiFunctionality.Navigation.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Notification;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the root view screen.
    /// </summary>
    public partial class RootViewModel : BaseViewModel
    {
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IDatabaseManager _databaseManager;
        private readonly IToastService _toastService;

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
        public RootViewModel(IBackendCommunicationService backendCommunicationService, IDatabaseManager databaseManager, IToastService toastService)
        {
            IsBackNavigationEnabled = true;
            _backendCommunicationService = backendCommunicationService;
            _databaseManager = databaseManager;
            _toastService = toastService;

            Categories = new ObservableCollection<StorageGroup>();
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var outcomeObj = (bool)await DialogService.ShowActivityIndicatorAndReturnResult("Loading...",
            async () =>{ return await LoadCategoryAsync();});

            if(!outcomeObj)
            {
                await _toastService.DisplayToast("Failed to load categories or no storage entries found.");        
            }
        }

        public AsyncRelayCommand QrScanCommand => new AsyncRelayCommand(async () =>
        {
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

        private async Task<bool> LoadCategoryAsync()
        {
            try
            {
                var storageList = await _databaseManager.GetListAsync<StorageEntry>();

                if (storageList == null || !storageList.Any())
                {
                    Categories.Clear();
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

                Categories.Clear();
                foreach (var group in grouped) 
                { 
                    Categories.Add(group); 
                }
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