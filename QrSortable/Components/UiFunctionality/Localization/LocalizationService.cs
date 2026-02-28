namespace QrSortable.Components.UiFunctionality.Localization
{
    using System.ComponentModel;
    using System.Globalization;

    public class LocalizationService : ILocalizationService, INotifyPropertyChanged
    {
        public static LocalizationService Instance { get; } = new LocalizationService();

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            AppResources.Culture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;

            // Notify all bindings
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public string Get(string key)
        {
            return AppResources.ResourceManager.GetString(key, AppResources.Culture);
        }

        public string this[string key] => Get(key);
    }
}
