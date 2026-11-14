namespace QrSortable.Components.CoreFeatures.Scanner.Models
{
    /// <summary>
    /// Compiled-in seed so suggestions work on day one.
    /// Every term is listed in this order: English, German, French, Spanish.
    ///
    /// Besides the flat lists (Locations, Categories, Remarks) there are three RELATION tables.
    /// They tell the service which item names / remarks / categories belong together,
    /// so "Tools" in the box already gives "Hammer, Screwdriver, Drill..." for Item Name.
    /// The user's own saved data is learned on top and outranks these defaults.
    /// </summary>
    public static class SuggestionSeed
    {
        // ------------------------------------------------------------------
        // Flat lists
        // ------------------------------------------------------------------
        public static readonly string[][] Locations =
        {
            new[] { "Garage", "Garage", "Garage", "Garaje" },
            new[] { "Basement", "Keller", "Cave", "Sótano" },
            new[] { "Attic", "Dachboden", "Grenier", "Ático" },
            new[] { "Kitchen", "Küche", "Cuisine", "Cocina" },
            new[] { "Living room", "Wohnzimmer", "Salon", "Salón" },
            new[] { "Bedroom", "Schlafzimmer", "Chambre", "Dormitorio" },
            new[] { "Children's room", "Kinderzimmer", "Chambre d'enfant", "Habitación infantil" },
            new[] { "Bathroom", "Badezimmer", "Salle de bain", "Baño" },
            new[] { "Office", "Büro", "Bureau", "Oficina" },
            new[] { "Hallway", "Flur", "Couloir", "Pasillo" },
            new[] { "Garden", "Garten", "Jardin", "Jardín" },
            new[] { "Shed", "Gartenhaus", "Abri de jardin", "Cobertizo" },
            new[] { "Workshop", "Werkstatt", "Atelier", "Taller" },
            new[] { "Storage room", "Abstellraum", "Débarras", "Trastero" },
            new[] { "Laundry room", "Waschküche", "Buanderie", "Lavadero" },
            new[] { "Pantry", "Vorratskammer", "Garde-manger", "Despensa" },
            new[] { "Balcony", "Balkon", "Balcon", "Balcón" },
            new[] { "Terrace", "Terrasse", "Terrasse", "Terraza" },
            new[] { "Wardrobe", "Kleiderschrank", "Penderie", "Armario" },
            new[] { "Car", "Auto", "Voiture", "Coche" },
        };

        public static readonly string[][] Categories =
        {
            new[] { "Tools", "Werkzeug", "Outils", "Herramientas" },
            new[] { "Kitchenware", "Küchenartikel", "Ustensiles de cuisine", "Utensilios de cocina" },
            new[] { "Clothing", "Kleidung", "Vêtements", "Ropa" },
            new[] { "Bedding", "Bettwäsche", "Linge de lit", "Ropa de cama" },
            new[] { "Books", "Bücher", "Livres", "Libros" },
            new[] { "Toys", "Spielzeug", "Jouets", "Juguetes" },
            new[] { "Electronics", "Elektronik", "Électronique", "Electrónica" },
            new[] { "Cables", "Kabel", "Câbles", "Cables" },
            new[] { "Documents", "Dokumente", "Documents", "Documentos" },
            new[] { "Decorations", "Dekoration", "Décoration", "Decoración" },
            new[] { "Christmas", "Weihnachten", "Noël", "Navidad" },
            new[] { "Sports", "Sport", "Sport", "Deporte" },
            new[] { "Camping", "Camping", "Camping", "Camping" },
            new[] { "Bicycle", "Fahrrad", "Vélo", "Bicicleta" },
            new[] { "Gardening", "Gartenartikel", "Jardinage", "Jardinería" },
            new[] { "Cleaning", "Putzmittel", "Produits d'entretien", "Limpieza" },
            new[] { "Paint", "Farben", "Peinture", "Pintura" },
            new[] { "Hobby", "Hobby", "Loisirs", "Aficiones" },
            new[] { "Medicine", "Medikamente", "Médicaments", "Medicamentos" },
            new[] { "Memories", "Erinnerungen", "Souvenirs", "Recuerdos" },
        };

        /// <summary>Generic remark chips (condition etc.) for the Remarks field.</summary>
        public static readonly string[][] Remarks =
        {
            new[] { "New", "Neu", "Neuf", "Nuevo" },
            new[] { "Used", "Gebraucht", "Utilisé", "Usado" },
            new[] { "Defective", "Defekt", "Défectueux", "Defectuoso" },
            new[] { "Fragile", "Zerbrechlich", "Fragile", "Frágil" },
            new[] { "Borrowed", "Geliehen", "Emprunté", "Prestado" },
            new[] { "Gift", "Geschenk", "Cadeau", "Regalo" },
            new[] { "Incomplete", "Unvollständig", "Incomplet", "Incompleto" },
            new[] { "Needs repair", "Reparaturbedürftig", "À réparer", "Necesita reparación" },
        };

        // ------------------------------------------------------------------
        // Relation 1: Category (English name) -> typical ITEM NAMES
        // ------------------------------------------------------------------
        public static readonly Dictionary<string, string[][]> ItemsByCategory =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Tools"] = new[]
            {
                new[] { "Hammer", "Hammer", "Marteau", "Martillo" },
                new[] { "Screwdriver", "Schraubendreher", "Tournevis", "Destornillador" },
                new[] { "Drill", "Bohrmaschine", "Perceuse", "Taladro" },
                new[] { "Pliers", "Zange", "Pince", "Alicates" },
                new[] { "Screws", "Schrauben", "Vis", "Tornillos" },
                new[] { "Wrench", "Schraubenschlüssel", "Clé", "Llave inglesa" },
                new[] { "Tape measure", "Maßband", "Mètre ruban", "Cinta métrica" },
                new[] { "Saw", "Säge", "Scie", "Sierra" },
            },
                ["Kitchenware"] = new[]
            {
                new[] { "Pots", "Töpfe", "Casseroles", "Ollas" },
                new[] { "Pans", "Pfannen", "Poêles", "Sartenes" },
                new[] { "Plates", "Teller", "Assiettes", "Platos" },
                new[] { "Glasses", "Gläser", "Verres", "Vasos" },
                new[] { "Cups", "Tassen", "Tasses", "Tazas" },
                new[] { "Cutlery", "Besteck", "Couverts", "Cubiertos" },
                new[] { "Baking tray", "Backblech", "Plaque de cuisson", "Bandeja de horno" },
                new[] { "Mixer", "Mixer", "Mixeur", "Batidora" },
            },
                ["Clothing"] = new[]
            {
                new[] { "Jackets", "Jacken", "Vestes", "Chaquetas" },
                new[] { "Winter clothes", "Winterkleidung", "Vêtements d'hiver", "Ropa de invierno" },
                new[] { "Summer clothes", "Sommerkleidung", "Vêtements d'été", "Ropa de verano" },
                new[] { "Shoes", "Schuhe", "Chaussures", "Zapatos" },
                new[] { "T-shirts", "T-Shirts", "T-shirts", "Camisetas" },
                new[] { "Pants", "Hosen", "Pantalons", "Pantalones" },
                new[] { "Sweaters", "Pullover", "Pulls", "Jerséis" },
                new[] { "Socks", "Socken", "Chaussettes", "Calcetines" },
            },
                ["Bedding"] = new[]
            {
                new[] { "Duvet", "Bettdecke", "Couette", "Edredón" },
                new[] { "Pillows", "Kopfkissen", "Oreillers", "Almohadas" },
                new[] { "Bed sheets", "Bettlaken", "Draps", "Sábanas" },
                new[] { "Duvet covers", "Bettbezüge", "Housses de couette", "Fundas nórdicas" },
                new[] { "Blankets", "Decken", "Couvertures", "Mantas" },
                new[] { "Towels", "Handtücher", "Serviettes", "Toallas" },
            },
                ["Books"] = new[]
            {
                new[] { "Novels", "Romane", "Romans", "Novelas" },
                new[] { "Cookbooks", "Kochbücher", "Livres de cuisine", "Libros de cocina" },
                new[] { "Children's books", "Kinderbücher", "Livres pour enfants", "Libros infantiles" },
                new[] { "Textbooks", "Schulbücher", "Manuels scolaires", "Libros de texto" },
                new[] { "Comics", "Comics", "Bandes dessinées", "Cómics" },
                new[] { "Magazines", "Zeitschriften", "Magazines", "Revistas" },
            },
                ["Toys"] = new[]
            {
                new[] { "Lego", "Lego", "Lego", "Lego" },
                new[] { "Board games", "Brettspiele", "Jeux de société", "Juegos de mesa" },
                new[] { "Puzzles", "Puzzles", "Puzzles", "Puzles" },
                new[] { "Stuffed animals", "Kuscheltiere", "Peluches", "Peluches" },
                new[] { "Dolls", "Puppen", "Poupées", "Muñecas" },
                new[] { "Toy cars", "Spielzeugautos", "Petites voitures", "Coches de juguete" },
                new[] { "Building blocks", "Bausteine", "Blocs de construction", "Bloques de construcción" },
            },
                ["Electronics"] = new[]
            {
                new[] { "Laptop", "Laptop", "Ordinateur portable", "Portátil" },
                new[] { "Chargers", "Ladegeräte", "Chargeurs", "Cargadores" },
                new[] { "Headphones", "Kopfhörer", "Écouteurs", "Auriculares" },
                new[] { "Old phones", "Alte Handys", "Anciens téléphones", "Teléfonos antiguos" },
                new[] { "Camera", "Kamera", "Appareil photo", "Cámara" },
                new[] { "Batteries", "Batterien", "Piles", "Pilas" },
                new[] { "Game console", "Spielkonsole", "Console de jeux", "Consola" },
                new[] { "Router", "Router", "Routeur", "Router" },
            },
                ["Cables"] = new[]
            {
                new[] { "USB cables", "USB-Kabel", "Câbles USB", "Cables USB" },
                new[] { "HDMI cables", "HDMI-Kabel", "Câbles HDMI", "Cables HDMI" },
                new[] { "Extension cords", "Verlängerungskabel", "Rallonges", "Alargadores" },
                new[] { "Power strips", "Steckdosenleisten", "Multiprises", "Regletas" },
                new[] { "Network cables", "Netzwerkkabel", "Câbles réseau", "Cables de red" },
                new[] { "Adapters", "Adapter", "Adaptateurs", "Adaptadores" },
                new[] { "Power cables", "Netzkabel", "Câbles d'alimentation", "Cables de alimentación" },
            },
                ["Documents"] = new[]
            {
                new[] { "Contracts", "Verträge", "Contrats", "Contratos" },
                new[] { "Tax documents", "Steuerunterlagen", "Documents fiscaux", "Documentos fiscales" },
                new[] { "Insurance papers", "Versicherungsunterlagen", "Documents d'assurance", "Documentos de seguros" },
                new[] { "Invoices", "Rechnungen", "Factures", "Facturas" },
                new[] { "Manuals", "Bedienungsanleitungen", "Notices d'utilisation", "Manuales" },
                new[] { "Certificates", "Zeugnisse", "Certificats", "Certificados" },
                new[] { "Bank statements", "Kontoauszüge", "Relevés bancaires", "Extractos bancarios" },
                new[] { "Warranty cards", "Garantiekarten", "Cartes de garantie", "Tarjetas de garantía" },
            },
                ["Decorations"] = new[]
            {
                new[] { "Vases", "Vasen", "Vases", "Jarrones" },
                new[] { "Picture frames", "Bilderrahmen", "Cadres photo", "Marcos de fotos" },
                new[] { "Candles", "Kerzen", "Bougies", "Velas" },
                new[] { "Candle holders", "Kerzenhalter", "Bougeoirs", "Candelabros" },
                new[] { "Wall pictures", "Wandbilder", "Tableaux", "Cuadros" },
                new[] { "Artificial flowers", "Kunstblumen", "Fleurs artificielles", "Flores artificiales" },
                new[] { "Cushions", "Kissen", "Coussins", "Cojines" },
            },
                ["Christmas"] = new[]
            {
                new[] { "Christmas tree", "Weihnachtsbaum", "Sapin de Noël", "Árbol de Navidad" },
                new[] { "Baubles", "Christbaumkugeln", "Boules de Noël", "Bolas de Navidad" },
                new[] { "Fairy lights", "Lichterkette", "Guirlande lumineuse", "Luces de Navidad" },
                new[] { "Advent wreath", "Adventskranz", "Couronne de l'Avent", "Corona de Adviento" },
                new[] { "Nativity scene", "Krippe", "Crèche", "Belén" },
                new[] { "Wrapping paper", "Geschenkpapier", "Papier cadeau", "Papel de regalo" },
                new[] { "Tinsel", "Lametta", "Guirlandes", "Espumillón" },
            },
                ["Sports"] = new[]
            {
                new[] { "Football", "Fußball", "Ballon de football", "Balón de fútbol" },
                new[] { "Tennis rackets", "Tennisschläger", "Raquettes de tennis", "Raquetas de tenis" },
                new[] { "Yoga mat", "Yogamatte", "Tapis de yoga", "Esterilla de yoga" },
                new[] { "Dumbbells", "Hanteln", "Haltères", "Mancuernas" },
                new[] { "Skis", "Ski", "Skis", "Esquís" },
                new[] { "Helmets", "Helme", "Casques", "Cascos" },
                new[] { "Ice skates", "Schlittschuhe", "Patins à glace", "Patines de hielo" },
            },
                ["Camping"] = new[]
            {
                new[] { "Tent", "Zelt", "Tente", "Tienda de campaña" },
                new[] { "Sleeping bags", "Schlafsäcke", "Sacs de couchage", "Sacos de dormir" },
                new[] { "Camping stove", "Campingkocher", "Réchaud de camping", "Hornillo de camping" },
                new[] { "Camping chairs", "Campingstühle", "Chaises de camping", "Sillas de camping" },
                new[] { "Lantern", "Laterne", "Lanterne", "Linterna" },
                new[] { "Backpack", "Rucksack", "Sac à dos", "Mochila" },
                new[] { "Sleeping mats", "Isomatten", "Matelas de sol", "Esterillas" },
            },
                ["Bicycle"] = new[]
            {
                new[] { "Bike pump", "Fahrradpumpe", "Pompe à vélo", "Bomba de bicicleta" },
                new[] { "Bike helmet", "Fahrradhelm", "Casque de vélo", "Casco de bicicleta" },
                new[] { "Bike lock", "Fahrradschloss", "Antivol", "Candado de bicicleta" },
                new[] { "Bike lights", "Fahrradlichter", "Éclairage de vélo", "Luces de bicicleta" },
                new[] { "Spare tubes", "Ersatzschläuche", "Chambres à air", "Cámaras de repuesto" },
                new[] { "Bike tools", "Fahrradwerkzeug", "Outils de vélo", "Herramientas de bicicleta" },
                new[] { "Bike bags", "Fahrradtaschen", "Sacoches de vélo", "Alforjas" },
            },
                ["Gardening"] = new[]
            {
                new[] { "Garden hose", "Gartenschlauch", "Tuyau d'arrosage", "Manguera de jardín" },
                new[] { "Shovel", "Schaufel", "Pelle", "Pala" },
                new[] { "Rake", "Rechen", "Râteau", "Rastrillo" },
                new[] { "Flower pots", "Blumentöpfe", "Pots de fleurs", "Macetas" },
                new[] { "Garden shears", "Gartenschere", "Sécateur", "Tijeras de podar" },
                new[] { "Watering can", "Gießkanne", "Arrosoir", "Regadera" },
                new[] { "Seeds", "Samen", "Graines", "Semillas" },
                new[] { "Fertilizer", "Dünger", "Engrais", "Fertilizante" },
            },
                ["Cleaning"] = new[]
            {
                new[] { "Detergent", "Waschmittel", "Lessive", "Detergente" },
                new[] { "Sponges", "Schwämme", "Éponges", "Esponjas" },
                new[] { "Cleaning cloths", "Putzlappen", "Chiffons", "Trapos de limpieza" },
                new[] { "Mop", "Wischmopp", "Balai serpillière", "Fregona" },
                new[] { "Bucket", "Eimer", "Seau", "Cubo" },
                new[] { "Vacuum bags", "Staubsaugerbeutel", "Sacs d'aspirateur", "Bolsas de aspiradora" },
                new[] { "Broom", "Besen", "Balai", "Escoba" },
                new[] { "Dish soap", "Spülmittel", "Liquide vaisselle", "Lavavajillas" },
            },
                ["Paint"] = new[]
            {
                new[] { "Wall paint", "Wandfarbe", "Peinture murale", "Pintura de pared" },
                new[] { "Paint brushes", "Pinsel", "Pinceaux", "Pinceles" },
                new[] { "Paint rollers", "Farbrollen", "Rouleaux", "Rodillos" },
                new[] { "Masking tape", "Malerkrepp", "Ruban de masquage", "Cinta de carrocero" },
                new[] { "Wood stain", "Holzlasur", "Lasure", "Lasur para madera" },
                new[] { "Spray paint", "Sprühlack", "Bombe de peinture", "Pintura en aerosol" },
                new[] { "Primer", "Grundierung", "Sous-couche", "Imprimación" },
                new[] { "Drop cloth", "Abdeckfolie", "Bâche de protection", "Lona protectora" },
            },
                ["Hobby"] = new[]
            {
                new[] { "Sewing kit", "Nähzeug", "Kit de couture", "Kit de costura" },
                new[] { "Knitting wool", "Strickwolle", "Laine à tricoter", "Lana para tejer" },
                new[] { "Craft supplies", "Bastelmaterial", "Matériel de bricolage créatif", "Material de manualidades" },
                new[] { "Musical instruments", "Musikinstrumente", "Instruments de musique", "Instrumentos musicales" },
                new[] { "Sheet music", "Noten", "Partitions", "Partituras" },
                new[] { "Photography gear", "Fotoausrüstung", "Matériel photo", "Equipo fotográfico" },
                new[] { "Model kits", "Modellbausätze", "Maquettes", "Maquetas" },
                new[] { "Painting supplies", "Malzubehör", "Matériel de peinture", "Material de pintura" },
            },
                ["Medicine"] = new[]
            {
                new[] { "First aid kit", "Erste-Hilfe-Set", "Trousse de premiers secours", "Botiquín" },
                new[] { "Painkillers", "Schmerzmittel", "Antidouleurs", "Analgésicos" },
                new[] { "Plasters", "Pflaster", "Pansements", "Tiritas" },
                new[] { "Thermometer", "Fieberthermometer", "Thermomètre", "Termómetro" },
                new[] { "Cold medicine", "Erkältungsmittel", "Médicaments contre le rhume", "Medicamentos para el resfriado" },
                new[] { "Bandages", "Verbände", "Bandages", "Vendas" },
                new[] { "Vitamins", "Vitamine", "Vitamines", "Vitaminas" },
            },
                ["Memories"] = new[]
            {
                new[] { "Photo albums", "Fotoalben", "Albums photo", "Álbumes de fotos" },
                new[] { "Old photos", "Alte Fotos", "Anciennes photos", "Fotos antiguas" },
                new[] { "Letters", "Briefe", "Lettres", "Cartas" },
                new[] { "Souvenirs", "Souvenirs", "Souvenirs", "Souvenirs" },
                new[] { "Diaries", "Tagebücher", "Journaux intimes", "Diarios" },
                new[] { "Baby memories", "Babysachen", "Souvenirs de bébé", "Recuerdos de bebé" },
                new[] { "Trophies", "Pokale", "Trophées", "Trofeos" },
                new[] { "Greeting cards", "Grußkarten", "Cartes de vœux", "Tarjetas de felicitación" },
            },
            };

        // ------------------------------------------------------------------
        // Relation 2: Category (English name) -> typical REMARKS
        // ------------------------------------------------------------------
        private static readonly string[] Kids = { "Kids", "Kinder", "Enfants", "Niños" };
        private static readonly string[] Winter = { "Winter", "Winter", "Hiver", "Invierno" };
        private static readonly string[] Summer = { "Summer", "Sommer", "Été", "Verano" };
        private static readonly string[] Washed = { "Washed", "Gewaschen", "Lavé", "Lavado" };
        private static readonly string[] Complete = { "Complete", "Vollständig", "Complet", "Completo" };
        private static readonly string[] Working = { "Working", "Funktioniert", "Fonctionne", "Funciona" };
        private static readonly string[] BatteriesIncl = { "Batteries included", "Mit Batterien", "Piles incluses", "Con pilas" };
        private static readonly string[] Spare = { "Spare", "Ersatz", "De rechange", "De repuesto" };
        private static readonly string[] Valuable = { "Valuable", "Wertvoll", "Précieux", "Valioso" };
        private static readonly string[] Important = { "Important", "Wichtig", "Important", "Importante" };
        private static readonly string[] Open = { "Opened", "Geöffnet", "Ouvert", "Abierto" };
        private static readonly string[] Sealed = { "Sealed", "Original verpackt", "Scellé", "Precintado" };
        private static readonly string[] Expired = { "Expired", "Abgelaufen", "Périmé", "Caducado" };
        private static readonly string[] Waterproof = { "Waterproof", "Wasserdicht", "Étanche", "Impermeable" };

        public static readonly Dictionary<string, string[][]> RemarksByCategory =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Tools"] = new[]
            {
                new[] { "Cordless", "Akku", "Sans fil", "Inalámbrico" },
                new[] { "With case", "Mit Koffer", "Avec mallette", "Con maletín" },
                Complete,
            },
                ["Kitchenware"] = new[]
            {
                new[] { "Dishwasher safe", "Spülmaschinenfest", "Compatible lave-vaisselle", "Apto para lavavajillas" },
                new[] { "Set", "Set", "Lot", "Juego" },
                Complete,
            },
                ["Clothing"] = new[] { Winter, Summer, Washed, Kids },
                ["Bedding"] = new[] { Washed, Winter, Summer, Spare },
                ["Books"] = new[]
            {
                Kids,
                new[] { "Hardcover", "Gebunden", "Relié", "Tapa dura" },
                Valuable,
            },
                ["Toys"] = new[] { Kids, BatteriesIncl, Complete, Working },
                ["Electronics"] = new[]
            {
                BatteriesIncl, Working,
                new[] { "With warranty", "Mit Garantie", "Sous garantie", "Con garantía" },
            },
                ["Cables"] = new[]
            {
                Working, Spare,
                new[] { "Long", "Lang", "Long", "Largo" },
            },
                ["Documents"] = new[]
            {
                Important,
                new[] { "Original", "Original", "Original", "Original" },
                new[] { "Copy", "Kopie", "Copie", "Copia" },
            },
                ["Decorations"] = new[]
            {
                Valuable,
                new[] { "Seasonal", "Saisonal", "De saison", "De temporada" },
            },
                ["Christmas"] = new[]
            {
                Complete,
                new[] { "Glass", "Glas", "Verre", "Vidrio" },
                Working,
            },
                ["Sports"] = new[] { Kids, Winter, Summer },
                ["Camping"] = new[] { Complete, Summer, Waterproof },
                ["Bicycle"] = new[] { Kids, Spare, Working },
                ["Gardening"] = new[]
            {
                Summer, Winter,
                new[] { "Rusty", "Rostig", "Rouillé", "Oxidado" },
            },
                ["Cleaning"] = new[] { Open, Sealed, Spare },
                ["Paint"] = new[]
            {
                Open, Sealed,
                new[] { "Dried out", "Eingetrocknet", "Sec", "Seco" },
            },
                ["Hobby"] = new[] { Complete, Valuable, Kids },
                ["Medicine"] = new[] { Expired, Open, Sealed },
                ["Memories"] = new[] { Valuable, Important, new[] { "Fragile", "Zerbrechlich", "Fragile", "Frágil" } },
            };

        // ------------------------------------------------------------------
        // Relation 3: Location (English name) -> typical CATEGORIES (English names)
        // Helps Category chips in the box view AND item names when the category is custom.
        // ------------------------------------------------------------------
        public static readonly Dictionary<string, string[]> CategoriesByLocation =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Garage"] = new[] { "Tools", "Bicycle", "Paint", "Sports", "Camping", "Gardening" },
                ["Basement"] = new[] { "Tools", "Cleaning", "Christmas", "Paint", "Documents", "Memories" },
                ["Attic"] = new[] { "Christmas", "Memories", "Decorations", "Clothing", "Bedding", "Books" },
                ["Kitchen"] = new[] { "Kitchenware", "Cleaning", "Electronics" },
                ["Living room"] = new[] { "Books", "Decorations", "Electronics", "Toys", "Cables" },
                ["Bedroom"] = new[] { "Clothing", "Bedding", "Books" },
                ["Children's room"] = new[] { "Toys", "Books", "Clothing" },
                ["Bathroom"] = new[] { "Cleaning", "Medicine", "Bedding" },
                ["Office"] = new[] { "Documents", "Electronics", "Cables", "Books", "Hobby" },
                ["Hallway"] = new[] { "Clothing", "Sports", "Bicycle" },
                ["Garden"] = new[] { "Gardening", "Tools", "Sports", "Camping" },
                ["Shed"] = new[] { "Gardening", "Tools", "Bicycle", "Paint" },
                ["Workshop"] = new[] { "Tools", "Paint", "Hobby", "Electronics", "Cables" },
                ["Storage room"] = new[] { "Christmas", "Camping", "Sports", "Decorations", "Tools" },
                ["Laundry room"] = new[] { "Cleaning", "Bedding", "Clothing" },
                ["Pantry"] = new[] { "Kitchenware", "Cleaning" },
                ["Balcony"] = new[] { "Gardening", "Bicycle", "Camping" },
                ["Terrace"] = new[] { "Gardening", "Camping", "Decorations" },
                ["Wardrobe"] = new[] { "Clothing", "Bedding" },
                ["Car"] = new[] { "Tools", "Medicine", "Documents", "Cleaning" },
            };
    }
}
