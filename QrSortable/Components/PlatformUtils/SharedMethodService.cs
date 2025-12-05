using System.Globalization;

namespace QrSortable.Components.PlatformUtils
{
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
    }
}
