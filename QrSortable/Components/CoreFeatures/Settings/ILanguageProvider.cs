namespace QrSortable.Components.CoreFeatures.Settings
{
    using QrSortable.Components.CoreFeatures.Settings.Models;
    /// <summary>
    ///     The LanguageProvider to facilitate language selection.
    /// </summary>
    public interface ILanguageProvider
    {
        /// <summary>
        ///     Gets and sets the currently selected language.
        /// </summary>
        LanguageItem SelectedLanguage { get; set; }

        /// <summary>
        ///     Returns a list of language items, one for each available language.
        /// </summary>
        IList<LanguageItem> AvailableLanguages { get; }

        /// <summary>
        ///     Sets the default language for the app.
        /// </summary>
        void SetDefaultLanguage();

        /// <summary>
        ///     Sets and fully persists the language before returning.
        ///     Prefer this over the setter whenever the caller needs to await the DB write.
        /// </summary>
        Task SetSelectedLanguageAsync(LanguageItem language);
    }
}