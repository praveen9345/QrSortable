namespace QrSortable.Components.CoreFeatures.Scanner.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the ItemDetailView screen.
    /// </summary>
    public partial class ItemDetailViewModel : BaseViewModel<StorageEntry>
    {
        private readonly IDatabaseManager _databaseManager;
        private StorageEntry _storageData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemDetailViewModel" />.
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public ItemDetailViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = true;
            _databaseManager = databaseManager;
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        [ObservableProperty]
        private string _itemName;

        [ObservableProperty]
        private string _itemDescription;

        /// <summary>
        ///     Prepares the viewmode with an storage raw data.
        /// </summary>
        /// <param name="storageData">The string storage data.</param>
        public override async void Prepare(StorageEntry storageData)
        {
            _storageData = storageData;

            if (_storageData != null) 
            { 
                //TODO: furhter implementaion
            }
            else
            {
                Console.WriteLine("Error:ItemDetailViewModel:Prepare: storageData is null");
            }

        }

        public AsyncRelayCommand AddItemCommand => new AsyncRelayCommand(async () =>
        {

        });

        public AsyncRelayCommand SaveCommand => new AsyncRelayCommand(async () =>
        {

            if (_storageData != null)
            {
                _storageData.CreatedDate = DateTime.Now;


                //_databaseManager.BeginTransaction();

                //var wasAddingItemSuccessful = await _databaseManager.AddAsync(_storageData);

                //if (wasAddingItemSuccessful != null)
                //{
                //    _databaseManager.CommitTransaction();
                //    Console.WriteLine("Error:ItemDetailViewModel:SaveCommand: storageData is Saved");
                //}
                //else
                //{
                //    _databaseManager.Rollback();
                //}
            }
            else
            {
                Console.WriteLine("Error:ItemDetailViewModel:SaveCommand: storageData is null");
            }
        });

    }
}