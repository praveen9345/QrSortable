namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Models;
    using Navigation.ViewModels;
    using QrSortable.Core.Components.UiFunctionality.Notification.Models;

    /// <summary>
    ///     The view model of the dialog of the type alert.
    /// </summary>
    public partial class DialogAlertViewModel : BaseViewModel<DialogAlertData, bool>
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="DialogAlertViewModel"/> class.
        /// </summary>
        public DialogAlertViewModel(IServiceProvider serviceProvider)
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
        ///     Gets the text of the button.
        /// </summary>
        [ObservableProperty]
        private string _dialogButtonText;

        /// <summary>
        ///   Gets or sets a value indicating whether the title exists.
        /// </summary>
        [ObservableProperty]
        private bool _titleExists;

        /// <summary>
        ///     Gets the command to close the dialog view belonging to this view model.
        /// </summary>
        public AsyncRelayCommand CloseDialogCommand => new AsyncRelayCommand(async () =>
        {
            NavigationService.CloseDialog(true);
        });

        /// <summary>
        ///     Prepares the viewmodel and sets the properties using the given DialogAlertData.
        /// </summary>
        /// <param name="parameter">The DialogAlertData passed over to this viewmodel.</param>
        public override void Prepare(DialogAlertData parameter)
        {
            DialogTitle = parameter.Title;
            DialogText = parameter.Text;
            DialogButtonText = parameter.ButtonText;
           
        }

    }
}
