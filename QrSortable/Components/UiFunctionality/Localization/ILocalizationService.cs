namespace QrSortable.Components.UiFunctionality.Localization
{
    using System.ComponentModel;
    using System.Globalization;

    public interface ILocalizationService
    {
        string Get(string key);
        void SetCulture(CultureInfo culture);
        event PropertyChangedEventHandler PropertyChanged;
    }
}
