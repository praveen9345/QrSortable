namespace QrSortable.Components.CoreFeatures.Settings
{
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.Settings.Models;
    using QrSortable.Components.CoreFeatures.Settings.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;

    /// <summary>
    ///     The LanguageProvider to facilitate language selection.
    /// </summary>
    public class LanguageProvider : ILanguageProvider
    {
        private readonly ICultureInfoWrapper _cultureInfoWrapper;
        private readonly ILocalizationService _localizationService;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly IList<string> _availableLanguageCodes = new List<string>() { "en", "de", "fr", "es"};
        private const string DefaultLanguageCode = "en";
        
        private LanguageItem _selectedLanguage;

        /// <summary>
        ///     Initializes a <see cref="LanguageProvider"/> to facilitate language selection.
        /// </summary>
        /// <param name="cultureInfoWrapper">The wrapper for handling culture infos.</param>
        /// <param name="generalInformationManager">The service for providing the GeneralInformation entity.</param>
        public LanguageProvider(ICultureInfoWrapper cultureInfoWrapper, 
            IGeneralInformationManager generalInformationManager, ILocalizationService localizationService)
        {
            _cultureInfoWrapper = cultureInfoWrapper;
            _localizationService = localizationService;
            _generalInformationManager = generalInformationManager;
        }

        public LanguageItem SelectedLanguage
        {
            get => _selectedLanguage ?? new LanguageItem(DefaultLanguageCode);
            set
            {
                if (value != null)
                {
                    _selectedLanguage = value;
                    _localizationService.SetCulture(value.CultureInfo);
                }
            }
        }

        public IList<LanguageItem> AvailableLanguages => 
            _availableLanguageCodes.Select(langCode => new LanguageItem(langCode)).ToList();


        /// <summary>
        ///     Sets the default language for the app.
        /// </summary>
        public void SetDefaultLanguage()
        {
            SelectedLanguage = new LanguageItem(DefaultLanguageCode);
        }

        /// <summary>
        ///     Sets and fully persists the selected language to the database before returning.
        ///     Use this from ViewModels where you must guarantee the save completed before navigating away.
        /// </summary>
        public async Task SetSelectedLanguageAsync(LanguageItem language)
        {
            if (language == null) return;

            // Update UI/Culture immediately
            this.SelectedLanguage = language;

            // Wait for DB persistence
            await _generalInformationManager.SetLanguageAsync(language.LanguageCode);
        }

    }
}