using System.Globalization;
using System.Text;

namespace QrSortable.Components.CoreFeatures.Scanner.Models
{
    public static class SuggestionText
    {
        /// <summary>lower-case, trimmed, diacritics stripped, ß→ss, single spaces.</summary>
        public static string Norm(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            s = s.Trim().ToLowerInvariant().Replace("ß", "ss");

            bool ascii = true;
            foreach (char c in s)
            {
                if (c > 127) { ascii = false; break; }
            }

            if (!ascii)
            {
                try
                {
                    var d = s.Normalize(NormalizationForm.FormD);
                    var sb = new StringBuilder(d.Length);
                    foreach (char c in d)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                            sb.Append(c);
                    }
                    s = sb.ToString().Normalize(NormalizationForm.FormC);
                }
                catch
                {
                    // Normalization not available (invariant globalization): keep lower-cased text.
                }
            }

            if (s.Contains("  "))
                s = string.Join(' ', s.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            return s;
        }
    }
}
