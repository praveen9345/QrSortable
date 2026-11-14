namespace QrSortable.Components.CoreFeatures.Scanner
{
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Models;
    using System.Globalization;

    public class SuggestionService : ISuggestionService
    {
        private const int MaxResults = 6;
        private const int MaxRemarkLength = 40;

        private readonly IDatabaseManager _databaseManager;
        private readonly IGeneralInformationManager _generalInformationManager;
        private volatile string? _languageCode;   // loaded from the saved app settings
        private readonly object _gate = new();
        private readonly SemaphoreSlim _buildLock = new(1, 1);
        private Data _data = new();          // seed-only until the DB build finishes
        private volatile bool _built;

        public SuggestionService(IDatabaseManager databaseManager,
            IGeneralInformationManager generalInformationManager)
        {
            _databaseManager = databaseManager;
            _generalInformationManager = generalInformationManager;
        }

        /// <summary>Reads the language the user selected in the app settings.</summary>
        private async Task RefreshLanguageAsync()
        {
            try
            {
                var settings = await _generalInformationManager.GetGeneralInformationAsync();
                _languageCode = settings?.SelectedLanguageCode ?? "en";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SuggestionService.RefreshLanguageAsync: {ex}");
            }
        }

        // ------------------------------------------------------------------
        // Build
        // ------------------------------------------------------------------
        public async Task EnsureBuiltAsync()
        {
            await RefreshLanguageAsync();   // cheap; picks up a changed app language
            if (_built) return;

            await _buildLock.WaitAsync();
            try
            {
                if (_built) return;

                var rows = await _databaseManager.GetListAsync<StorageEntry>();

                var data = await Task.Run(() =>
                {
                    var d = new Data();
                    if (rows != null)
                    {
                        foreach (var e in rows)
                        {
                            d.Add(e, Convert.ToDateTime((object?)e.CreatedDate));
                        }
                    }
                    return d;
                });

                lock (_gate)
                {
                    // keep anything recorded while the build was running
                    _data = data;
                    _built = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SuggestionService.EnsureBuiltAsync: {ex}");
            }
            finally
            {
                _buildLock.Release();
            }
        }

        // ------------------------------------------------------------------
        // Incremental update
        // ------------------------------------------------------------------
        public void RecordSaved(StorageEntry entry)
        {
            if (entry == null) return;
            lock (_gate)
            {
                _data.Add(entry, DateTime.UtcNow);
            }
        }

        // ------------------------------------------------------------------
        // Query
        // ------------------------------------------------------------------
        public IReadOnlyList<string> Suggest(SuggestionField target, string? typed,
            params (SuggestionField Field, string? Value)[] context)
        {
            string q = SuggestionText.Norm(typed);
            int lang = LangIndex();
            var now = DateTime.UtcNow;

            lock (_gate)
            {
                var d = _data;

                // FIX 2: resolve each context value to its own key AND to its "parent" seed key,
                // e.g. "Küche - unteres Regal" -> ["kuche - unteres regal" (1.0), "kitchen" (0.6)]
                var ctx = new List<(SuggestionField F, string K, double W)>();
                if (context != null)
                {
                    foreach (var c in context)
                    {
                        var (key, parent) = d.ResolveKeys(c.Field, c.Value);
                        if (key != null) ctx.Add((c.Field, key, 1.0));
                        if (parent != null && parent != key) ctx.Add((c.Field, parent, 0.6));
                    }
                }

                var cands = new List<(double Score, bool Related, Term T)>();

                foreach (var t in d.Terms[(int)target].Values)
                {
                    double score = 0;

                    if (q.Length > 0)
                    {
                        double m = Match(q, t.Aliases);
                        if (m <= 0) continue;       // no match, or exact match (nothing to suggest)
                        score += m;
                    }

                    double rel = 0;
                    foreach (var c in ctx)
                    {
                        if (d.Pairs.TryGetValue(new PairKey(target, t.Key, c.F, c.K), out double pc))
                            rel += c.W * 3.0 * Math.Log(1 + pc);
                    }
                    score += rel;

                    score += Math.Log(1 + t.Count);

                    if (t.Count > 0)
                        score += 1.0 / (1.0 + (now - t.LastUsed).TotalDays / 30.0);
                    else if (t.IsSeed)
                        score += 0.3 / (1 + t.SeedOrder);

                    // anything related to the context always ranks above unrelated terms
                    bool related = rel > 0;
                    if (related) score += 10;

                    cands.Add((score, related, t));
                }

                IEnumerable<(double Score, bool Related, Term T)> pool = cands;

                // Item names with nothing typed yet: show ONLY items that belong to this
                // category/location. If nothing is related, fall back to items the user
                // has really saved before. Never dump unrelated seed items.
                if (target == SuggestionField.ItemName && q.Length == 0)
                {
                    var related = cands.Where(x => x.Related).ToList();
                    pool = related.Count > 0 ? related : cands.Where(x => x.T.Count > 0);
                }

                return pool
                    .OrderByDescending(x => x.Score)
                    .Take(MaxResults)
                    .Select(x => x.T.DisplayFor(lang))
                    .ToArray();
            }
        }

        // 4 = prefix, 2.5 = word-start, 1 = contains, 0 = none, -1 = exact (skip)
        private static double Match(string q, string[] aliases)
        {
            double best = 0;
            foreach (var a in aliases)
            {
                if (a == q) return -1;
                if (a.StartsWith(q, StringComparison.Ordinal)) best = Math.Max(best, 4);
                else if (a.Contains(' ' + q, StringComparison.Ordinal)) best = Math.Max(best, 2.5);
                else if (a.Contains(q, StringComparison.Ordinal)) best = Math.Max(best, 1);
            }
            return best;
        }

        private int LangIndex()
        {
            string code = (_languageCode ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName).ToLowerInvariant();
            return code switch { "de" => 1, "fr" => 2, "es" => 3, _ => 0 };
        }


        // ------------------------------------------------------------------
        // Index data structures
        // ------------------------------------------------------------------
        private sealed class Term
        {
            public string Key = string.Empty;
            public string[] Aliases = Array.Empty<string>();
            public string[]? SeedDisplays;               // en, de, fr, es
            public string Display = string.Empty;        // for user-learned terms
            public int SeedOrder = int.MaxValue;
            public int Count;
            public DateTime LastUsed = DateTime.MinValue;

            public bool IsSeed => SeedDisplays != null;

            public string DisplayFor(int lang) =>
                SeedDisplays != null ? SeedDisplays[Math.Min(lang, SeedDisplays.Length - 1)] : Display;
        }

        private readonly record struct PairKey(SuggestionField A, string KA, SuggestionField B, string KB);

        private sealed class Data
        {
            public readonly Dictionary<string, Term>[] Terms = new Dictionary<string, Term>[4];
            private readonly Dictionary<string, string>[] _aliasToKey = new Dictionary<string, string>[4];
            public readonly Dictionary<PairKey, double> Pairs = new();
            private const double SeedWeight = 0.5;   // a seed relation counts as half a real save

            public Data()
            {
                for (int i = 0; i < 4; i++)
                {
                    Terms[i] = new Dictionary<string, Term>(StringComparer.Ordinal);
                    _aliasToKey[i] = new Dictionary<string, string>(StringComparer.Ordinal);
                }

                Seed(SuggestionField.Location, SuggestionSeed.Locations);
                Seed(SuggestionField.Category, SuggestionSeed.Categories);
                Seed(SuggestionField.Description, SuggestionSeed.Remarks);
                SeedRelations();
            }

            private void SeedRelations()
            {
                var catItems = new Dictionary<string, List<string>>();

                // Category -> item names
                foreach (var (catName, groups) in SuggestionSeed.ItemsByCategory)
                {
                    var ck = ResolveKey(SuggestionField.Category, catName);
                    if (ck == null) continue;
                    var list = new List<string>();
                    foreach (var g in groups)
                    {
                        var ik = AddSeedTerm(SuggestionField.ItemName, g);
                        list.Add(ik);
                        Bump(SuggestionField.Category, ck, SuggestionField.ItemName, ik, SeedWeight);
                    }
                    catItems[ck] = list;
                }

                // Category -> remarks
                foreach (var (catName, groups) in SuggestionSeed.RemarksByCategory)
                {
                    var ck = ResolveKey(SuggestionField.Category, catName);
                    if (ck == null) continue;
                    foreach (var g in groups)
                    {
                        var rk = AddSeedTerm(SuggestionField.Description, g);
                        Bump(SuggestionField.Category, ck, SuggestionField.Description, rk, SeedWeight);
                    }
                }

                // Location -> categories (and, through them, item names)
                foreach (var (locName, cats) in SuggestionSeed.CategoriesByLocation)
                {
                    var lk = ResolveKey(SuggestionField.Location, locName);
                    if (lk == null) continue;
                    foreach (var catName in cats)
                    {
                        var ck = ResolveKey(SuggestionField.Category, catName);
                        if (ck == null) continue;
                        Bump(SuggestionField.Location, lk, SuggestionField.Category, ck, SeedWeight);

                        if (catItems.TryGetValue(ck, out var items))
                            foreach (var ik in items)
                                Bump(SuggestionField.Location, lk, SuggestionField.ItemName, ik, SeedWeight * 0.5);
                    }
                }
            }

            /// <summary>Adds a seed term (or returns the existing one, e.g. "Kids" shared by many categories).</summary>
            private string AddSeedTerm(SuggestionField f, string[] group)
            {
                var aliases = group.Select(SuggestionText.Norm).Distinct().ToArray();
                var key = aliases[0];
                var dict = Terms[(int)f];
                if (!dict.ContainsKey(key))
                {
                    dict[key] = new Term
                    {
                        Key = key,
                        Aliases = aliases,
                        SeedDisplays = group,
                        SeedOrder = dict.Count
                    };
                    foreach (var a in aliases) _aliasToKey[(int)f][a] = key;
                }
                return key;
            }

            private void Seed(SuggestionField f, string[][] groups)
            {
                int order = 0;
                foreach (var g in groups)
                {
                    var aliases = g.Select(SuggestionText.Norm).Distinct().ToArray();
                    var key = aliases[0];
                    Terms[(int)f][key] = new Term
                    {
                        Key = key,
                        Aliases = aliases,
                        SeedDisplays = g,
                        SeedOrder = order++
                    };
                    foreach (var a in aliases) _aliasToKey[(int)f][a] = key;
                }
            }

            public string? ResolveKey(SuggestionField f, string? raw)
            {
                var n = SuggestionText.Norm(raw);
                if (n.Length == 0) return null;
                return _aliasToKey[(int)f].TryGetValue(n, out var k) ? k : n;
            }

            /// <summary>
            /// Key of the seed term a custom text STARTS with, in any language:
            /// "Küche - unteres Regal" / "Keller, links" / "kitchen shelf" -> kitchen / basement.
            /// For Location, Category and ItemName ("Hammer 500 g" -> hammer). Returns null when there is none.
            /// </summary>
            private string? ParentKey(SuggestionField f, string normalized)
            {
                if (f != SuggestionField.Location && f != SuggestionField.Category && f != SuggestionField.ItemName) return null;

                string? best = null;
                int bestLen = 0;
                foreach (var kv in _aliasToKey[(int)f])
                {
                    var a = kv.Key;
                    if (a.Length <= bestLen || normalized.Length <= a.Length) continue;
                    if (!normalized.StartsWith(a, StringComparison.Ordinal)) continue;
                    if (char.IsLetterOrDigit(normalized[a.Length])) continue;   // whole word only
                    best = kv.Value;
                    bestLen = a.Length;
                }
                return best;
            }

            public (string? Key, string? Parent) ResolveKeys(SuggestionField f, string? raw)
            {
                var n = SuggestionText.Norm(raw);
                if (n.Length == 0) return (null, null);
                var key = _aliasToKey[(int)f].TryGetValue(n, out var k) ? k : n;
                var parent = key == n ? ParentKey(f, n) : null;   // exact seed terms have no parent
                return (key, parent);
            }

            private string? Touch(SuggestionField f, string? raw, DateTime when)
            {
                var key = ResolveKey(f, raw);
                if (key == null) return null;

                var dict = Terms[(int)f];
                if (!dict.TryGetValue(key, out var t))
                {
                    t = new Term { Key = key, Aliases = new[] { key }, Display = raw!.Trim() };
                    dict[key] = t;
                }
                t.Count++;
                if (when > t.LastUsed) t.LastUsed = when;
                return key;
            }

            private void Inc(PairKey k, double w)
            {
                Pairs.TryGetValue(k, out double c);
                Pairs[k] = c + w;
            }

            private void Bump(SuggestionField a, string? ka, SuggestionField b, string? kb, double w = 1.0)
            {
                if (ka == null || kb == null) return;
                Inc(new PairKey(a, ka, b, kb), w);
                Inc(new PairKey(b, kb, a, ka), w);
            }

            // Bump with the full key AND the parent seed key of A (so "Küche - unteres Regal"
            // also counts for "Küche"), and the same for B.
            private void BumpWithParents(SuggestionField a, string? ka, string? pa,
                                         SuggestionField b, string? kb, string? pb)
            {
                Bump(a, ka, b, kb);
                if (pa != null && pa != ka) Bump(a, pa, b, kb);
                if (pb != null && pb != kb) Bump(a, ka, b, pb);
                if (pa != null && pa != ka && pb != null && pb != kb) Bump(a, pa, b, pb);
            }

            public void Add(StorageEntry e, DateTime when)
            {
                var loc = Touch(SuggestionField.Location, e.Location, when);
                var cat = Touch(SuggestionField.Category, e.Category, when);
                var item = Touch(SuggestionField.ItemName, e.ItemName, when);

                string? locParent = loc == null ? null : ParentKey(SuggestionField.Location, loc);
                string? catParent = cat == null ? null : ParentKey(SuggestionField.Category, cat);
                string? itemParent = item == null ? null : ParentKey(SuggestionField.ItemName, item);

                BumpWithParents(SuggestionField.Location, loc, locParent,
                                SuggestionField.Category, cat, catParent);

                BumpWithParents(SuggestionField.Category, cat, catParent,
                                SuggestionField.ItemName, item, itemParent);
                BumpWithParents(SuggestionField.Location, loc, locParent,
                                SuggestionField.ItemName, item, itemParent);

                if (!string.IsNullOrWhiteSpace(e.Description))
                {
                    var segments = e.Description.Split(',',
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    foreach (var seg in segments)
                    {
                        if (seg.Length > MaxRemarkLength) continue;   // long free text is not reusable
                        var desc = Touch(SuggestionField.Description, seg, when);
                        BumpWithParents(SuggestionField.ItemName, item, itemParent,
                                        SuggestionField.Description, desc, null);
                        BumpWithParents(SuggestionField.Category, cat, catParent,
                                        SuggestionField.Description, desc, null);
                    }
                }
            }
        }
    }
}
