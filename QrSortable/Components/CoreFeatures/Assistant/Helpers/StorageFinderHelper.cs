namespace QrSortable.Components.CoreFeatures.Assistant.Helpers
{
    using System.Globalization;
    using System.Text;

    public class StorageFinderHelper : IStorageFinderHelper
    {

        // 99% Multilingual Support: Converts "Müller" to "muller"
        public string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                // Remove "NonSpacingMark" (accents/diacritics) 
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        // Fuzzy Logic: Calculates similarity between 0.0 (none) and 1.0 (perfect) [1, 5]
        public double CalculateSimilarity(string source, string target)
        {
            if (source == target) return 1.0;
            if (source.Length == 0 || target.Length == 0) return 0.0;

            int distance = LevenshteinDistance(source, target);
            return 1.0 - ((double)distance / Math.Max(source.Length, target.Length));
        }

        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length, m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

    }
}
