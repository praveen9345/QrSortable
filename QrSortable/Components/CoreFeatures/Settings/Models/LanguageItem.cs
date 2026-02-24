namespace QrSortable.Components.CoreFeatures.Settings.Models
{
    using System.Globalization;

    /// <summary>
    ///     A model providing information about a language.
    /// </summary>
    public class LanguageItem
    {
        private readonly CultureInfo _cultureInfo;

        /// <summary>
        ///     Initializes a new instance of <see cref="LanguageItem" />.
        /// </summary>
        /// <param name="languageCode">A unique string representing a language, eg. "en" or "de".</param>
        public LanguageItem(string languageCode)
        {
            try
            {
                _cultureInfo = new CultureInfo(languageCode);
            }
            catch
            {
                _cultureInfo = CultureInfo.InvariantCulture;
            }
        }

        /// <summary>
        ///     Gets the native name of the language. Used to display the language within the app.
        /// </summary>
        public string NativeName => _cultureInfo.NativeName;


        /// <summary>
        ///     Gets a unique string representing the language to persistently store the selected language.
        /// </summary>
        public string LanguageCode => _cultureInfo.TwoLetterISOLanguageName;

        /// <summary>
        ///     Gets the culture info associated with the language.
        /// </summary>
        public CultureInfo CultureInfo => _cultureInfo;
    }
}
