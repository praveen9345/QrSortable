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

        /// <summary>
        /// Returns the shipping cost in EUR for the given country name.
        /// Returns 25.00 for unknown/unrecognized countries (treated as ROW).
        /// Returns 0 if no country is provided.
        /// </summary>
        public decimal GetShippingCost(string? country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return 0m;

            if (!CountryGroups.TryGetValue(country, out string? group))
                return GroupRates["ROW"]; // Unknown country → ROW default

            return GroupRates[group];
        }

        private readonly Dictionary<string, string> CountryGroups = new()
        {
            { "Germany",        "DE" },
            { "United Kingdom", "UK" },
            { "Austria",        "EU" },
            { "Belgium",        "EU" },
            { "Bulgaria",       "EU" },
            { "Croatia",        "EU" },
            { "Cyprus",         "EU" },
            { "Czech Republic", "EU" },
            { "Denmark",        "EU" },
            { "Estonia",        "EU" },
            { "Finland",        "EU" },
            { "France",         "EU" },
            { "Greece",         "EU" },
            { "Hungary",        "EU" },
            { "Ireland",        "EU" },
            { "Italy",          "EU" },
            { "Latvia",         "EU" },
            { "Lithuania",      "EU" },
            { "Luxembourg",     "EU" },
            { "Malta",          "EU" },
            { "Netherlands",    "EU" },
            { "Poland",         "EU" },
            { "Portugal",       "EU" },
            { "Romania",        "EU" },
            { "Slovakia",       "EU" },
            { "Slovenia",       "EU" },
            { "Spain",          "EU" },
            { "Sweden",         "EU" },
            { "Switzerland",    "EU" },
            { "Norway",         "EU" },
            { "United States",  "ROW" },
            { "Canada",         "ROW" },
            { "Australia",      "ROW" },
            { "Other",          "ROW" }
        };

        private readonly Dictionary<string, decimal> GroupRates = new()
        {
            { "DE",  0.00m  },  // Free for Germany
            { "EU",  15.00m },  // EU countries
            { "UK",  20.00m },  // United Kingdom
            { "ROW", 25.00m }   // Rest of world
        };

        
    }
}
