namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
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
        private readonly IToastService _toastService;

        /// <summary>
        /// ...............
        /// </summary>
        /// <param name="databaseManager">An instance of <see cref="IDatabaseManager" /> 
        /// used for managing database operations.</param>
        /// <param name="toastService">The IToastService instance used for displaying toast notifications.</param>
        public DialogMoveToViewModel(IDatabaseManager databaseManager, IToastService toastService)
        {
            IsBackNavigationEnabled = false;
            _databaseManager = databaseManager;
            _toastService = toastService;
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
                var barcodes = allItems
                 .Select(x => x.BarcodeValue)
                 .Where(x => !string.IsNullOrEmpty(x) && x != parameter.BarcodeValue)
                 .Distinct().ToList();

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
                await _toastService.DisplayToast(AppResources.DialogMoveToViewModel_SelectBarcode);
                return;
            }
            NavigationService.CloseDialog(SelectedMoveTo);
            await Task.CompletedTask;
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

