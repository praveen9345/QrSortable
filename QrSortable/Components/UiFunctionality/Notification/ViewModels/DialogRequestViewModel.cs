namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Models;
    using Navigation.ViewModels;

    /// <summary>
    ///     The view model of the dialog of the type request.
    /// </summary>
    public partial class DialogRequestViewModel : BaseViewModel<DialogRequestData, bool>
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="DialogRequestViewModel" /> class.
        /// </summary>
        public DialogRequestViewModel(IServiceProvider serviceProvider)
        {
            IsBackNavigationEnabled = false;
        }

        /// <summary>
        /// Asynchronously initializes the object by calling the base initialization and
        /// sets the TitleExists property based on the length of DialogTitle.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            TitleExists = DialogTitle.Length > 0;
        }

        /// <summary>
        ///     Gets the title of the dialog.
        /// </summary>
        [ObservableProperty]
        private string _dialogTitle;

        /// <summary>
        ///     Gets the text of the dialog.
        /// </summary>
        [ObservableProperty]
        private string _dialogText;

        /// <summary>
        ///     Gets the text of the cancel button.
        /// </summary>
        [ObservableProperty] 
        private string _dialogCancelButtonText;

        /// <summary>
        ///     Gets the text of the confirm button.
        /// </summary>
        [ObservableProperty] 
        private string _dialogConfirmButtonText;

        /// <summary>
        ///   Gets or sets a value indicating whether the title exists.
        /// </summary>
        [ObservableProperty]
        private bool _titleExists;

        /// <summary>
        ///     Gets the command for the cancel (left) button, which closes the viewmodel and returns false.
        /// </summary>
        public AsyncRelayCommand CancelButtonCommand => new AsyncRelayCommand(async () =>
            NavigationService.CloseDialog(false));

        /// <summary>
        ///     Gets the command for the confirm (right) button, which closes the viewmodel and returns true.
        /// </summary>
        public AsyncRelayCommand ConfirmButtonCommand => new AsyncRelayCommand(async () =>
            NavigationService.CloseDialog(true));

        /// <summary>
        ///     Prepares the viewmodel and sets the properties using the given DialogRequestData.
        /// </summary>
        /// <param name="parameter">The DialogRequestData passed over to this viewmodel.</param>
        public override void Prepare(DialogRequestData parameter)
        {
            DialogTitle = parameter.Title;
            DialogText = parameter.Text;
            DialogCancelButtonText = parameter.CancelButtonText;
            DialogConfirmButtonText = parameter.ConfirmButtonText;
        }

    }
}