using QrSortable.Components.CoreFeatures.Scanner.Models;

namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{
    /// <summary>
    /// Compiled-in Tag Name suggestions for households, moving and small businesses.
    /// Every term is listed in this order: English, German, French, Spanish.
    /// Other app languages fall back to English (add a column + extend LangIndex to support more).
    ///
    /// Typing matches against ALL languages, but the chip is shown in the app language.
    /// The three groups are interleaved, so the first chips (nothing typed yet) show a mix.
    /// </summary>
    public static class TagNameSeed
    {
        private const int MaxResults = 6;   // SuggestionChipBar shows at most 6 chips

        // ------------------------------------------------------------------
        // Household
        // ------------------------------------------------------------------
        private static readonly string[][] Household =
        {
            new[] { "Kitchen", "Küche", "Cuisine", "Cocina" },
            new[] { "Bedroom", "Schlafzimmer", "Chambre", "Dormitorio" },
            new[] { "Living room", "Wohnzimmer", "Salon", "Salón" },
            new[] { "Bathroom", "Badezimmer", "Salle de bain", "Baño" },
            new[] { "Children's room", "Kinderzimmer", "Chambre d'enfant", "Habitación infantil" },
            new[] { "Garage", "Garage", "Garage", "Garaje" },
            new[] { "Basement", "Keller", "Cave", "Sótano" },
            new[] { "Attic", "Dachboden", "Grenier", "Ático" },
            new[] { "Office", "Büro", "Bureau", "Oficina" },
            new[] { "Pantry", "Vorratskammer", "Garde-manger", "Despensa" },
            new[] { "Garden", "Garten", "Jardin", "Jardín" },
            new[] { "Tools", "Werkzeug", "Outils", "Herramientas" },
            new[] { "Toys", "Spielzeug", "Jouets", "Juguetes" },
            new[] { "Books", "Bücher", "Livres", "Libros" },
            new[] { "Winter clothes", "Winterkleidung", "Vêtements d'hiver", "Ropa de invierno" },
            new[] { "Summer clothes", "Sommerkleidung", "Vêtements d'été", "Ropa de verano" },
            new[] { "Bedding", "Bettwäsche", "Linge de lit", "Ropa de cama" },
            new[] { "Christmas", "Weihnachten", "Noël", "Navidad" },
            new[] { "Documents", "Dokumente", "Documents", "Documentos" },
            new[] { "Electronics", "Elektronik", "Électronique", "Electrónica" },
            new[] { "Cleaning supplies", "Putzmittel", "Produits d'entretien", "Productos de limpieza" },
            new[] { "First aid", "Erste Hilfe", "Premiers secours", "Primeros auxilios" },
            new[] { "Photos and memories", "Fotos und Erinnerungen", "Photos et souvenirs", "Fotos y recuerdos" },
            new[] { "Camping", "Camping", "Camping", "Camping" },
            new[] { "Sports", "Sport", "Sport", "Deporte" },
        };

        // ------------------------------------------------------------------
        // Moving
        // ------------------------------------------------------------------
        private static readonly string[][] Moving =
        {
            new[] { "Fragile", "Zerbrechlich", "Fragile", "Frágil" },
            new[] { "Open first", "Zuerst öffnen", "À ouvrir en premier", "Abrir primero" },
            new[] { "Heavy", "Schwer", "Lourd", "Pesado" },
            new[] { "This side up", "Diese Seite oben", "Ce côté en haut", "Este lado arriba" },
            new[] { "Essentials", "Das Wichtigste", "L'essentiel", "Lo esencial" },
            new[] { "Unpack later", "Später auspacken", "À déballer plus tard", "Desembalar después" },
            new[] { "Valuables", "Wertsachen", "Objets de valeur", "Objetos de valor" },
            new[] { "Glassware", "Glaswaren", "Verrerie", "Cristalería" },
            new[] { "Dishes", "Geschirr", "Vaisselle", "Vajilla" },
            new[] { "Wardrobe box", "Kleiderbox", "Carton penderie", "Caja armario" },
            new[] { "Cables and chargers", "Kabel und Ladegeräte", "Câbles et chargeurs", "Cables y cargadores" },
            new[] { "Do not stack", "Nicht stapeln", "Ne pas empiler", "No apilar" },
            new[] { "Keep dry", "Trocken halten", "Garder au sec", "Mantener seco" },
            new[] { "Storage", "Lagerung", "Stockage", "Almacenamiento" },
            new[] { "Donate", "Spenden", "À donner", "Donar" },
            new[] { "Sell", "Verkaufen", "À vendre", "Vender" },
            new[] { "New home", "Neues Zuhause", "Nouvelle maison", "Casa nueva" },
            new[] { "Pet supplies", "Haustierbedarf", "Articles pour animaux", "Artículos para mascotas" },
            new[] { "Toiletries", "Hygieneartikel", "Produits d'hygiène", "Artículos de higiene" },
        };

        // ------------------------------------------------------------------
        // Small business
        // ------------------------------------------------------------------
        private static readonly string[][] Business =
        {
            new[] { "Inventory", "Inventar", "Inventaire", "Inventario" },
            new[] { "Products", "Produkte", "Produits", "Productos" },
            new[] { "Shipping", "Versand", "Expédition", "Envíos" },
            new[] { "Packaging", "Verpackung", "Emballage", "Embalaje" },
            new[] { "Orders", "Bestellungen", "Commandes", "Pedidos" },
            new[] { "Returns", "Rücksendungen", "Retours", "Devoluciones" },
            new[] { "Office supplies", "Büromaterial", "Fournitures de bureau", "Material de oficina" },
            new[] { "Invoices", "Rechnungen", "Factures", "Facturas" },
            new[] { "Samples", "Muster", "Échantillons", "Muestras" },
            new[] { "Raw materials", "Rohmaterial", "Matières premières", "Materias primas" },
            new[] { "Stock", "Lagerbestand", "Stock", "Existencias" },
            new[] { "Customer orders", "Kundenaufträge", "Commandes clients", "Pedidos de clientes" },
            new[] { "Equipment", "Ausrüstung", "Équipement", "Equipamiento" },
            new[] { "Marketing materials", "Werbematerial", "Supports marketing", "Material de marketing" },
            new[] { "Accounting", "Buchhaltung", "Comptabilité", "Contabilidad" },
            new[] { "Archive", "Archiv", "Archives", "Archivo" },
            new[] { "Ready to ship", "Versandbereit", "Prêt à expédier", "Listo para enviar" },
            new[] { "Low stock", "Niedriger Bestand", "Stock faible", "Pocas existencias" },
            new[] { "Spare parts", "Ersatzteile", "Pièces de rechange", "Repuestos" },
            new[] { "Market stall", "Marktstand", "Stand de marché", "Puesto de mercado" },
        };

        // ------------------------------------------------------------------
        // Index (declared AFTER the arrays above: static initializers run in order)
        // ------------------------------------------------------------------
        private sealed class Term
        {
            public string[] Display = Array.Empty<string>();   // en, de, fr, es
            public string[] Aliases = Array.Empty<string>();   // normalized, all languages
        }

        private static readonly Term[] Terms = Build();

        private static Term[] Build()
        {
            var groups = new[] { Household, Moving, Business };
            var list = new List<Term>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            int max = groups.Max(g => g.Length);

            // round-robin so the first chips mix household / moving / business
            for (int i = 0; i < max; i++)
            {
                foreach (var g in groups)
                {
                    if (i >= g.Length) continue;
                    var d = g[i];
                    if (!seen.Add(SuggestionText.Norm(d[0]))) continue;

                    list.Add(new Term
                    {
                        Display = d,
                        Aliases = d.Select(SuggestionText.Norm).Distinct().ToArray()
                    });
                }
            }
            return list.ToArray();
        }

        // ------------------------------------------------------------------
        // Query
        // ------------------------------------------------------------------
        public static IReadOnlyList<string> Suggest(string? typed, string? languageCode)
        {
            string q = SuggestionText.Norm(typed);
            int lang = LangIndex(languageCode);

            var hits = new List<(double Score, int Order, string Text)>();
            for (int i = 0; i < Terms.Length; i++)
            {
                var t = Terms[i];
                double score = 0;
                if (q.Length > 0)
                {
                    score = Match(q, t.Aliases);
                    if (score <= 0) continue;   // no match, or exact match (nothing to suggest)
                }
                hits.Add((score, i, t.Display[Math.Min(lang, t.Display.Length - 1)]));
            }

            return hits
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Order)
                .Take(MaxResults)
                .Select(x => x.Text)
                .ToArray();
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

        private static int LangIndex(string? languageCode)
        {
            string code = (languageCode ?? System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                .ToLowerInvariant();
            if (code.Length > 2) code = code.Substring(0, 2);
            return code switch { "de" => 1, "fr" => 2, "es" => 3, _ => 0 };
        }
    }
}