namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using BarcodeScanning;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;

    public partial class DialogMoveToViewModel : BaseViewModel<StorageEntry>
    {
        private readonly IDatabaseManager _databaseManager;

        /// <summary>
        /// ...............
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        public DialogMoveToViewModel(IDatabaseManager databaseManager)
        {
            IsBackNavigationEnabled = false;
            _databaseManager = databaseManager;
        }

        // Optional title shown in the view (bind if needed)
        [ObservableProperty]
        private string _title;

        // Optional image shown in the view (bind if needed)
        [ObservableProperty]
        private ImageSource _titleImage;

        [ObservableProperty]
        private ObservableCollection<string> _moveToItems;

        [ObservableProperty]
        private string _selectedMoveTo;

        // Called by the dialog infrastructure. Accepts an IEnumerable<string> or string[] as parameter.
        public async override void Prepare(StorageEntry parameter)
        {

            Title = parameter.ItemName;

            var image = ImageSource.FromFile("image_icon");
            if (parameter.ImageList != null && parameter.ImageList.Count > 0)
            {
                image = ConvertToImageSource(parameter.ImageList[0]);
            }

            TitleImage = image;

            try
            {
                var allItems = await _databaseManager.GetAllAsync<StorageEntry>();
                var barcodes = allItems.Select(x => x.BarcodeValue)
                .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                
                MoveToItems = new ObservableCollection<string>(barcodes);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"DialogMoveToViewModel.Prepare: Exception: {ex}");
            }

        }

        // Confirm selection and close dialog returning the selected string
        public AsyncRelayCommand ConfirmCommand => new AsyncRelayCommand(async () =>
        {
            // CloseDialog expects the dialog result (string)
            if(string.IsNullOrEmpty(SelectedMoveTo))
            {
                await DialogService.ShowAlertDialog("Error", "Please select a Box bar code.", AppResources.Dialog_OK_Text);
                return;
            }
            NavigationService.CloseDialog(SelectedMoveTo);
            await Task.CompletedTask;
        });


        // Command used by the DataTemplate row tap to set selection immediately
        public RelayCommand<object> SelectionChangedCommand => new RelayCommand<object>(item =>
        {
            if (item == null) return;
            SelectedMoveTo = item.ToString();
        });

        // Cancel / close without result
        public AsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {

            NavigationService.CloseDialog(string.Empty);
            await Task.CompletedTask;
        });

        private ImageSource ConvertToImageSource(object input)
        {
            if (input == null)
                return null;

            Stream stream = input switch
            {
                byte[] jpegBytes when jpegBytes.Length > 0 => new MemoryStream(jpegBytes),
                Stream jpegStream => jpegStream,
                _ => throw new ArgumentException("Unsupported input type", nameof(input))
            };

            if (stream.CanSeek)
                stream.Position = 0;

            return ImageSource.FromStream(() => stream);
        }
    }
}

