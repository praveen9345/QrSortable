namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the feedback screen.
    /// </summary>
    public partial class HelpViewModel : BaseViewModel
    {
        private readonly IFileManager _fileManager;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;
        private readonly IGeneralInformationManager _generalInformationManager;
        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpViewModel" />.
        /// </summary>
        public HelpViewModel(IFileManager fileManager, IMauiEssentialsWrapper mauiEssentialsWrapper, IGeneralInformationManager generalInformationManager)
        {
            IsBackNavigationEnabled = true;
            _fileManager = fileManager;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;
            _generalInformationManager = generalInformationManager;
        }

        public AsyncRelayCommand UserMaualCommand => new AsyncRelayCommand(async () =>
        {
            try
            {
                var langCode = (await _generalInformationManager.GetGeneralInformationAsync()).SelectedLanguageCode;

                var fileName = langCode switch
                {
                    "de" => "user_manual_de.pdf",// german
                    "es" => "user_manual_es.pdf",// spanish
                    "fr" => "user_manual_fr.pdf",// french
                    _ => "user_manual_en.pdf" // default: English
                };


                if (!await _fileManager.OpenEmbeddedFileAsync(fileName))
                {
                    await ShowOpenFileErrorMessageAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
            }
        });

        public AsyncRelayCommand EmailIdCommand => new AsyncRelayCommand(async () =>
        {
            var recipients = new List<string> { "qrsortable@gmail.com" };
            if (!await _mauiEssentialsWrapper.SendEmailAsync(AppResources.HelpViewModel_EmailTitleText, "", recipients))
            {
                await DialogService.ShowAlertDialog(
               AppResources.Dialog_Error, AppResources.HelpViewModel_EmailSendOutErrorText, AppResources.Dialog_OK_Text);
            }
        });

        private async Task ShowOpenFileErrorMessageAsync()
        {
            await Application.Current.MainPage.DisplayAlert(AppResources.Dialog_Error,
                "File not found error", AppResources.Dialog_OK_Text);
        }

    }
}