namespace QrSortable.Components.PlatformUtils
{
    public interface ISharedMethodService
    {
        decimal ParsePrice(string priceText);

        /// <summary>
        /// Converts any object to a string representation.
        /// Handles null, DateTime, Guid, numeric types, bool, and other objects.
        /// </summary>
        string ConvertToString(object value);

        string GetCurrencySymbol(string languageCode);
    }
}
