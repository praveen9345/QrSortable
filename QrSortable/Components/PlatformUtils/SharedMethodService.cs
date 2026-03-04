namespace QrSortable.Components.PlatformUtils
{
    using System.Globalization;

    public class SharedMethodService : ISharedMethodService
    {
        public decimal ParsePrice(string priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText)) return 0m;
            var cleaned = new string(priceText.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out var value))
                return value;
            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return value;

            return 0m;
        }

        public string ConvertToString(object value)
        {
            if (value == null)
                return string.Empty;

            switch (value)
            {
                case string s:
                    return s;
                case int i:
                    return i.ToString();
                case long l:
                    return l.ToString();
                case float f:
                    return f.ToString("G"); // General format
                case double d:
                    return d.ToString("G");
                case decimal m:
                    return m.ToString();
                case bool b:
                    return b.ToString();
                case DateTime dt:
                    return dt.ToString("o"); // ISO 8601 format
                case Guid g:
                    return g.ToString();
                default:
                    return value.ToString(); // Fallback for any other type
            }
        }

        public string GetCurrencySymbol(string languageCode)
        {
            return (languageCode == "en")? "$" : "€";    
        }
    }
}
