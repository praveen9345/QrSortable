namespace QrSortable.Components.CoreFeatures.Scanner.Models
{
    /// <summary>
    /// Compiled-in seed so suggestions are useful from the very first scan.
    /// Every term is listed in this order: English, German, French, Spanish.
    ///
    /// RULES (the service relies on them):
    ///  1. The FIRST entry (English) is the term's identity. Never rename it after release,
    ///     otherwise learned pair counts of existing users no longer match.
    ///  2. Every key used in a relation table (ItemsByCategory, RemarksByCategory,
    ///     CategoriesByLocation) must exist in Categories / Locations.
    ///  3. The same English term used in two places is merged into ONE term
    ///     (e.g. "Kids" is a remark for Clothing, Toys, Sports ...). Keep its translations identical.
    ///  4. Translations of two DIFFERENT terms in the same field must not be identical
    ///     (alias collision) - e.g. eyewear is "Eyeglasses", drinking glasses are "Glasses".
    ///  5. Order matters: earlier entries are shown first when nothing else ranks them.
    /// </summary>
    public static class SuggestionSeed
    {
        // ------------------------------------------------------------------
        // Shared remark terms (declared first: static initializers run in order)
        // ------------------------------------------------------------------
        private static readonly string[] Fragile = { "Fragile", "Zerbrechlich", "Fragile", "Frágil" };
        private static readonly string[] Gift = { "Gift", "Geschenk", "Cadeau", "Regalo" };
        private static readonly string[] Sharp = { "Sharp", "Scharf", "Tranchant", "Afilado" };
        private static readonly string[] Rusty = { "Rusty", "Rostig", "Rouillé", "Oxidado" };
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
        private static readonly string[] Opened = { "Opened", "Geöffnet", "Ouvert", "Abierto" };
        private static readonly string[] Sealed = { "Sealed", "Original verpackt", "Scellé", "Precintado" };
        private static readonly string[] Expired = { "Expired", "Abgelaufen", "Périmé", "Caducado" };
        private static readonly string[] Waterproof = { "Waterproof", "Wasserdicht", "Étanche", "Impermeable" };
        private static readonly string[] WithWarranty = { "With warranty", "Mit Garantie", "Sous garantie", "Con garantía" };
        private static readonly string[] WithManual = { "With manual", "Mit Anleitung", "Avec notice", "Con manual" };
        private static readonly string[] Heavy = { "Heavy", "Schwer", "Lourd", "Pesado" };
        private static readonly string[] TooSmall = { "Too small", "Zu klein", "Trop petit", "Pequeño" };

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
            new[] { "Dining room", "Esszimmer", "Salle à manger", "Comedor" },
            new[] { "Guest room", "Gästezimmer", "Chambre d'amis", "Habitación de invitados" },
            new[] { "Nursery", "Babyzimmer", "Chambre de bébé", "Habitación del bebé" },
            new[] { "Playroom", "Spielzimmer", "Salle de jeux", "Sala de juegos" },
            new[] { "Hobby room", "Hobbyraum", "Salle de loisirs", "Sala de aficiones" },
            new[] { "Entrance", "Eingang", "Entrée", "Entrada" },
            new[] { "Toilet", "WC", "Toilettes", "Aseo" },
            new[] { "Under the stairs", "Unter der Treppe", "Sous l'escalier", "Bajo la escalera" },
            new[] { "Boiler room", "Heizungsraum", "Chaufferie", "Cuarto de calderas" },
            new[] { "Carport", "Carport", "Abri voiture", "Cochera" },
            new[] { "Greenhouse", "Gewächshaus", "Serre", "Invernadero" },
            new[] { "Holiday home", "Ferienhaus", "Maison de vacances", "Casa de vacaciones" },
            new[] { "Car trunk", "Kofferraum", "Coffre", "Maletero" },
            new[] { "Cupboard", "Schrank", "Placard", "Alacena" },
            new[] { "Shelf", "Regal", "Étagère", "Estantería" },
            new[] { "Drawer", "Schublade", "Tiroir", "Cajón" },
            new[] { "Toolbox", "Werkzeugkasten", "Boîte à outils", "Caja de herramientas" },
            new[] { "Suitcase", "Koffer", "Valise", "Maleta" },
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
            new[] { "Furniture", "Möbel", "Meubles", "Muebles" },
            new[] { "Appliances", "Haushaltsgeräte", "Électroménager", "Electrodomésticos" },
            new[] { "Lighting", "Beleuchtung", "Éclairage", "Iluminación" },
            new[] { "Baby", "Baby", "Bébé", "Bebé" },
            new[] { "Pets", "Haustiere", "Animaux", "Mascotas" },
            new[] { "Food", "Lebensmittel", "Alimentation", "Alimentos" },
            new[] { "Toiletries", "Hygieneartikel", "Produits d'hygiène", "Artículos de higiene" },
            new[] { "Stationery", "Schreibwaren", "Papeterie", "Papelería" },
            new[] { "Travel", "Reisen", "Voyage", "Viajes" },
            new[] { "Automotive", "Autozubehör", "Accessoires auto", "Accesorios de coche" },
            new[] { "Party", "Party", "Fête", "Fiesta" },
            new[] { "Seasonal", "Saisonartikel", "Articles de saison", "Artículos de temporada" },
            new[] { "Accessories", "Accessoires", "Accessoires", "Accesorios" },
            new[] { "School", "Schule", "École", "Escuela" },
            new[] { "Hardware", "Eisenwaren", "Quincaillerie", "Ferretería" },
            new[] { "Media", "Medien", "Médias", "Multimedia" },
        };

        /// <summary>Generic remark chips (condition, status, plan) for the Remarks field.</summary>
        public static readonly string[][] Remarks =
        {
            new[] { "New", "Neu", "Neuf", "Nuevo" },
            new[] { "Used", "Gebraucht", "Utilisé", "Usado" },
            new[] { "Defective", "Defekt", "Défectueux", "Defectuoso" },
            Fragile,
            new[] { "Borrowed", "Geliehen", "Emprunté", "Prestado" },
            Gift,
            new[] { "Incomplete", "Unvollständig", "Incomplet", "Incompleto" },
            new[] { "Needs repair", "Reparaturbedürftig", "À réparer", "Necesita reparación" },
            new[] { "Like new", "Wie neu", "Comme neuf", "Como nuevo" },
            new[] { "Old", "Alt", "Ancien", "Antiguo" },
            new[] { "Broken", "Kaputt", "Cassé", "Roto" },
            new[] { "Dirty", "Schmutzig", "Sale", "Sucio" },
            new[] { "Clean", "Sauber", "Propre", "Limpio" },
            new[] { "Original packaging", "Originalverpackung", "Emballage d'origine", "Embalaje original" },
            new[] { "Vintage", "Vintage", "Vintage", "Vintage" },
            new[] { "Handmade", "Handgemacht", "Fait main", "Hecho a mano" },
            Heavy,
            new[] { "To sell", "Zum Verkaufen", "À vendre", "Para vender" },
            new[] { "To donate", "Zum Spenden", "À donner", "Para donar" },
            new[] { "To throw away", "Zum Wegwerfen", "À jeter", "Para tirar" },
            new[] { "Keep", "Behalten", "À garder", "Conservar" },
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
                new[] { "Spirit level", "Wasserwaage", "Niveau à bulle", "Nivel" },
                new[] { "Utility knife", "Cuttermesser", "Cutter", "Cúter" },
                new[] { "Chisels", "Stemmeisen", "Ciseaux à bois", "Formones" },
                new[] { "Sandpaper", "Schleifpapier", "Papier abrasif", "Papel de lija" },
                new[] { "Allen keys", "Inbusschlüssel", "Clés Allen", "Llaves Allen" },
                new[] { "Cordless screwdriver", "Akkuschrauber", "Visseuse sans fil", "Atornillador inalámbrico" },
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
                new[] { "Knives", "Messer", "Couteaux", "Cuchillos" },
                new[] { "Cutting boards", "Schneidebretter", "Planches à découper", "Tablas de cortar" },
                new[] { "Baking molds", "Backformen", "Moules à gâteau", "Moldes para hornear" },
                new[] { "Food containers", "Frischhaltedosen", "Boîtes de conservation", "Recipientes para alimentos" },
                new[] { "Bowls", "Schüsseln", "Bols", "Cuencos" },
                new[] { "Wine glasses", "Weingläser", "Verres à vin", "Copas de vino" },
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
                new[] { "Underwear", "Unterwäsche", "Sous-vêtements", "Ropa interior" },
                new[] { "Dresses", "Kleider", "Robes", "Vestidos" },
                new[] { "Gloves", "Handschuhe", "Gants", "Guantes" },
                new[] { "Hats and caps", "Mützen und Kappen", "Bonnets et casquettes", "Gorros y gorras" },
                new[] { "Swimwear", "Badebekleidung", "Maillots de bain", "Bañadores" },
                new[] { "Pajamas", "Schlafanzüge", "Pyjamas", "Pijamas" },
                new[] { "Boots", "Stiefel", "Bottes", "Botas" },
            },
                ["Bedding"] = new[]
            {
                new[] { "Duvet", "Bettdecke", "Couette", "Edredón" },
                new[] { "Pillows", "Kopfkissen", "Oreillers", "Almohadas" },
                new[] { "Bed sheets", "Bettlaken", "Draps", "Sábanas" },
                new[] { "Duvet covers", "Bettbezüge", "Housses de couette", "Fundas nórdicas" },
                new[] { "Blankets", "Decken", "Couvertures", "Mantas" },
                new[] { "Towels", "Handtücher", "Serviettes", "Toallas" },
                new[] { "Pillowcases", "Kissenbezüge", "Taies d'oreiller", "Fundas de almohada" },
                new[] { "Mattress topper", "Matratzenauflage", "Surmatelas", "Cubrecolchón" },
            },
                ["Books"] = new[]
            {
                new[] { "Novels", "Romane", "Romans", "Novelas" },
                new[] { "Cookbooks", "Kochbücher", "Livres de cuisine", "Libros de cocina" },
                new[] { "Children's books", "Kinderbücher", "Livres pour enfants", "Libros infantiles" },
                new[] { "Textbooks", "Schulbücher", "Manuels scolaires", "Libros de texto" },
                new[] { "Comics", "Comics", "Bandes dessinées", "Cómics" },
                new[] { "Magazines", "Zeitschriften", "Magazines", "Revistas" },
                new[] { "Travel guides", "Reiseführer", "Guides de voyage", "Guías de viaje" },
                new[] { "Dictionaries", "Wörterbücher", "Dictionnaires", "Diccionarios" },
                new[] { "Atlases", "Atlanten", "Atlas", "Atlas" },
                new[] { "Non-fiction books", "Sachbücher", "Livres documentaires", "Libros de no ficción" },
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
                new[] { "Action figures", "Actionfiguren", "Figurines d'action", "Figuras de acción" },
                new[] { "Play kitchen", "Spielküche", "Cuisine jouet", "Cocinita de juguete" },
                new[] { "Wooden toys", "Holzspielzeug", "Jouets en bois", "Juguetes de madera" },
                new[] { "Toy train set", "Spielzeugeisenbahn", "Train miniature", "Tren de juguete" },
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
                new[] { "Tablet", "Tablet", "Tablette", "Tableta" },
                new[] { "Speakers", "Lautsprecher", "Enceintes", "Altavoces" },
                new[] { "Remote controls", "Fernbedienungen", "Télécommandes", "Mandos a distancia" },
                new[] { "Hard drives", "Festplatten", "Disques durs", "Discos duros" },
                new[] { "Smartwatch", "Smartwatch", "Montre connectée", "Reloj inteligente" },
                new[] { "Power bank", "Powerbank", "Batterie externe", "Batería externa" },
                new[] { "Keyboard and mouse", "Tastatur und Maus", "Clavier et souris", "Teclado y ratón" },
                new[] { "Printer", "Drucker", "Imprimante", "Impresora" },
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
                new[] { "Charging cables", "Ladekabel", "Câbles de charge", "Cables de carga" },
                new[] { "Audio cables", "Audiokabel", "Câbles audio", "Cables de audio" },
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
                new[] { "Passports and IDs", "Reisepässe und Ausweise", "Passeports et pièces d'identité", "Pasaportes y documentos de identidad" },
                new[] { "Medical records", "Arztunterlagen", "Dossiers médicaux", "Informes médicos" },
                new[] { "Rental agreement", "Mietvertrag", "Contrat de bail", "Contrato de alquiler" },
                new[] { "Car papers", "Fahrzeugpapiere", "Papiers du véhicule", "Documentos del vehículo" },
                new[] { "Official letters", "Behördenpost", "Courrier administratif", "Correspondencia oficial" },
                new[] { "Payslips", "Gehaltsabrechnungen", "Fiches de paie", "Nóminas" },
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
                new[] { "Mirrors", "Spiegel", "Miroirs", "Espejos" },
                new[] { "Clocks", "Uhren", "Horloges", "Relojes de pared" },
                new[] { "Figurines", "Figuren", "Statuettes", "Figuras decorativas" },
                new[] { "Curtains", "Vorhänge", "Rideaux", "Cortinas" },
                new[] { "Rugs", "Teppiche", "Tapis", "Alfombras" },
                new[] { "Posters", "Poster", "Affiches", "Pósteres" },
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
                new[] { "Christmas decorations", "Weihnachtsdeko", "Décorations de Noël", "Decoración navideña" },
                new[] { "Christmas tree stand", "Christbaumständer", "Pied de sapin", "Soporte para árbol" },
                new[] { "Gift bags", "Geschenktüten", "Sacs cadeaux", "Bolsas de regalo" },
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
                new[] { "Basketball", "Basketball", "Ballon de basket", "Balón de baloncesto" },
                new[] { "Badminton set", "Badminton-Set", "Set de badminton", "Set de bádminton" },
                new[] { "Swimming gear", "Schwimmzeug", "Affaires de natation", "Equipo de natación" },
                new[] { "Inline skates", "Inlineskates", "Rollers en ligne", "Patines en línea" },
                new[] { "Skateboard", "Skateboard", "Skateboard", "Monopatín" },
                new[] { "Gym bag", "Sporttasche", "Sac de sport", "Bolsa de deporte" },
                new[] { "Snowboard", "Snowboard", "Snowboard", "Snowboard" },
            },
                ["Camping"] = new[]
            {
                new[] { "Tent", "Zelt", "Tente", "Tienda de campaña" },
                new[] { "Sleeping bags", "Schlafsäcke", "Sacs de couchage", "Sacos de dormir" },
                new[] { "Camping stove", "Campingkocher", "Réchaud de camping", "Hornillo de camping" },
                new[] { "Camping chairs", "Campingstühle", "Chaises de camping", "Sillas de camping" },
                new[] { "Lantern", "Laterne", "Lanterne", "Farol de camping" },
                new[] { "Backpack", "Rucksack", "Sac à dos", "Mochila" },
                new[] { "Sleeping mats", "Isomatten", "Matelas de sol", "Esterillas" },
                new[] { "Camping table", "Campingtisch", "Table de camping", "Mesa de camping" },
                new[] { "Cooler box", "Kühlbox", "Glacière", "Nevera portátil" },
                new[] { "Tent pegs", "Zeltheringe", "Piquets de tente", "Piquetas" },
                new[] { "Air mattress", "Luftmatratze", "Matelas gonflable", "Colchón hinchable" },
                new[] { "Headlamp", "Stirnlampe", "Lampe frontale", "Linterna frontal" },
                new[] { "Tarp", "Plane", "Bâche", "Lona" },
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
                new[] { "Bike tires", "Fahrradreifen", "Pneus de vélo", "Neumáticos de bicicleta" },
                new[] { "Bike trailer", "Fahrradanhänger", "Remorque de vélo", "Remolque de bicicleta" },
                new[] { "Bike basket", "Fahrradkorb", "Panier de vélo", "Cesta de bicicleta" },
                new[] { "Bike rack", "Fahrradträger", "Porte-vélos", "Portabicicletas" },
                new[] { "Bike parts", "Fahrradteile", "Pièces de vélo", "Piezas de bicicleta" },
                new[] { "Chain oil", "Kettenöl", "Huile pour chaîne", "Aceite para cadena" },
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
                new[] { "Garden gloves", "Gartenhandschuhe", "Gants de jardin", "Guantes de jardín" },
                new[] { "Lawn mower", "Rasenmäher", "Tondeuse", "Cortacésped" },
                new[] { "Hedge trimmer", "Heckenschere", "Taille-haie", "Cortasetos" },
                new[] { "Wheelbarrow", "Schubkarre", "Brouette", "Carretilla" },
                new[] { "Sprinkler", "Rasensprenger", "Arroseur", "Aspersor" },
                new[] { "Potting soil", "Blumenerde", "Terreau", "Tierra para plantas" },
                new[] { "Garden lights", "Gartenleuchten", "Lampes de jardin", "Luces de jardín" },
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
                new[] { "Cleaning spray", "Reinigungsspray", "Spray nettoyant", "Spray limpiador" },
                new[] { "Trash bags", "Müllbeutel", "Sacs poubelle", "Bolsas de basura" },
                new[] { "Window cleaner", "Glasreiniger", "Nettoyant vitres", "Limpiacristales" },
                new[] { "Dustpan", "Kehrschaufel", "Pelle à poussière", "Recogedor" },
                new[] { "Rubber gloves", "Gummihandschuhe", "Gants en caoutchouc", "Guantes de goma" },
                new[] { "Vacuum cleaner", "Staubsauger", "Aspirateur", "Aspiradora" },
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
                new[] { "Paint trays", "Farbwannen", "Bacs à peinture", "Bandejas de pintura" },
                new[] { "Varnish", "Lack", "Vernis", "Barniz" },
                new[] { "Putty knife", "Spachtel", "Couteau à enduire", "Espátula" },
                new[] { "Wall filler", "Spachtelmasse", "Enduit de rebouchage", "Masilla" },
                new[] { "Wallpaper", "Tapeten", "Papier peint", "Papel pintado" },
                new[] { "Wallpaper paste", "Tapetenkleister", "Colle à papier peint", "Cola para papel pintado" },
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
                new[] { "Fishing gear", "Angelausrüstung", "Matériel de pêche", "Equipo de pesca" },
                new[] { "Embroidery supplies", "Stickzeug", "Matériel de broderie", "Material de bordado" },
                new[] { "Beads", "Perlen", "Perles", "Cuentas" },
                new[] { "Telescope", "Teleskop", "Télescope", "Telescopio" },
                new[] { "Chess set", "Schachspiel", "Jeu d'échecs", "Ajedrez" },
                new[] { "Hiking gear", "Wanderausrüstung", "Équipement de randonnée", "Equipo de senderismo" },
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
                new[] { "Eye drops", "Augentropfen", "Gouttes pour les yeux", "Gotas para los ojos" },
                new[] { "Allergy medicine", "Allergiemittel", "Médicaments contre les allergies", "Medicamentos para la alergia" },
                new[] { "Disinfectant", "Desinfektionsmittel", "Désinfectant", "Desinfectante" },
                new[] { "Prescription medicine", "Verschreibungspflichtige Medikamente", "Médicaments sur ordonnance", "Medicamentos con receta" },
                new[] { "Face masks", "Masken", "Masques", "Mascarillas" },
                new[] { "Sun cream", "Sonnencreme", "Crème solaire", "Crema solar" },
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
                new[] { "Wedding memories", "Hochzeitsandenken", "Souvenirs de mariage", "Recuerdos de boda" },
                new[] { "Children's drawings", "Kinderzeichnungen", "Dessins d'enfants", "Dibujos infantiles" },
                new[] { "Postcards", "Postkarten", "Cartes postales", "Postales" },
                new[] { "Medals", "Medaillen", "Médailles", "Medallas" },
                new[] { "Old videos", "Alte Videos", "Anciennes vidéos", "Vídeos antiguos" },
            },
                ["Furniture"] = new[]
            {
                new[] { "Chairs", "Stühle", "Chaises", "Sillas" },
                new[] { "Tables", "Tische", "Tables", "Mesas" },
                new[] { "Shelves", "Regale", "Étagères", "Estanterías" },
                new[] { "Stools", "Hocker", "Tabourets", "Taburetes" },
                new[] { "Drawers", "Schubladen", "Tiroirs", "Cajones" },
                new[] { "Bed frame", "Bettgestell", "Cadre de lit", "Estructura de cama" },
                new[] { "Mattress", "Matratze", "Matelas", "Colchón" },
                new[] { "Cabinet doors", "Schranktüren", "Portes d'armoire", "Puertas de armario" },
                new[] { "Table legs", "Tischbeine", "Pieds de table", "Patas de mesa" },
                new[] { "Assembly instructions", "Aufbauanleitungen", "Notices de montage", "Instrucciones de montaje" },
            },
                ["Appliances"] = new[]
            {
                new[] { "Coffee maker", "Kaffeemaschine", "Cafetière", "Cafetera" },
                new[] { "Toaster", "Toaster", "Grille-pain", "Tostadora" },
                new[] { "Kettle", "Wasserkocher", "Bouilloire", "Hervidor" },
                new[] { "Blender", "Standmixer", "Blender", "Licuadora" },
                new[] { "Microwave", "Mikrowelle", "Micro-ondes", "Microondas" },
                new[] { "Iron", "Bügeleisen", "Fer à repasser", "Plancha" },
                new[] { "Hair dryer", "Föhn", "Sèche-cheveux", "Secador de pelo" },
                new[] { "Fan", "Ventilator", "Ventilateur", "Ventilador" },
                new[] { "Heater", "Heizlüfter", "Radiateur soufflant", "Calefactor" },
                new[] { "Air fryer", "Heißluftfritteuse", "Friteuse à air", "Freidora de aire" },
                new[] { "Food processor", "Küchenmaschine", "Robot de cuisine", "Robot de cocina" },
            },
                ["Lighting"] = new[]
            {
                new[] { "Light bulbs", "Glühbirnen", "Ampoules", "Bombillas" },
                new[] { "LED strips", "LED-Streifen", "Bandes LED", "Tiras LED" },
                new[] { "Table lamps", "Tischlampen", "Lampes de table", "Lámparas de mesa" },
                new[] { "Floor lamps", "Stehlampen", "Lampadaires", "Lámparas de pie" },
                new[] { "Lampshades", "Lampenschirme", "Abat-jour", "Pantallas de lámpara" },
                new[] { "Flashlights", "Taschenlampen", "Lampes de poche", "Linternas" },
                new[] { "Ceiling lights", "Deckenlampen", "Plafonniers", "Lámparas de techo" },
                new[] { "Night lights", "Nachtlichter", "Veilleuses", "Luces nocturnas" },
            },
                ["Baby"] = new[]
            {
                new[] { "Baby clothes", "Babykleidung", "Vêtements de bébé", "Ropa de bebé" },
                new[] { "Diapers", "Windeln", "Couches", "Pañales" },
                new[] { "Baby bottles", "Babyflaschen", "Biberons", "Biberones" },
                new[] { "Pacifiers", "Schnuller", "Tétines", "Chupetes" },
                new[] { "Stroller", "Kinderwagen", "Poussette", "Cochecito" },
                new[] { "Baby monitor", "Babyphone", "Babyphone", "Vigilabebés" },
                new[] { "Car seat", "Kindersitz", "Siège auto", "Silla de coche" },
                new[] { "Rattles", "Rasseln", "Hochets", "Sonajeros" },
                new[] { "Baby blankets", "Babydecken", "Couvertures de bébé", "Mantas de bebé" },
                new[] { "High chair", "Hochstuhl", "Chaise haute", "Trona" },
                new[] { "Bibs", "Lätzchen", "Bavoirs", "Baberos" },
            },
                ["Pets"] = new[]
            {
                new[] { "Dog leash", "Hundeleine", "Laisse", "Correa para perros" },
                new[] { "Pet food", "Tierfutter", "Nourriture pour animaux", "Comida para mascotas" },
                new[] { "Pet bowls", "Futternäpfe", "Gamelles", "Comederos" },
                new[] { "Cat litter", "Katzenstreu", "Litière", "Arena para gatos" },
                new[] { "Pet toys", "Tierspielzeug", "Jouets pour animaux", "Juguetes para mascotas" },
                new[] { "Pet bed", "Tierbett", "Panier pour animaux", "Cama para mascotas" },
                new[] { "Pet carrier", "Transportbox", "Caisse de transport", "Transportín" },
                new[] { "Aquarium supplies", "Aquarienzubehör", "Accessoires d'aquarium", "Accesorios de acuario" },
                new[] { "Pet grooming", "Tierpflege", "Toilettage", "Aseo de mascotas" },
            },
                ["Food"] = new[]
            {
                new[] { "Canned food", "Konserven", "Conserves", "Conservas" },
                new[] { "Pasta", "Nudeln", "Pâtes", "Pasta" },
                new[] { "Rice", "Reis", "Riz", "Arroz" },
                new[] { "Spices", "Gewürze", "Épices", "Especias" },
                new[] { "Jam jars", "Marmeladengläser", "Pots de confiture", "Tarros de mermelada" },
                new[] { "Beverages", "Getränke", "Boissons", "Bebidas" },
                new[] { "Baking ingredients", "Backzutaten", "Ingrédients de pâtisserie", "Ingredientes de repostería" },
                new[] { "Snacks", "Snacks", "Snacks", "Aperitivos" },
                new[] { "Wine bottles", "Weinflaschen", "Bouteilles de vin", "Botellas de vino" },
                new[] { "Coffee and tea", "Kaffee und Tee", "Café et thé", "Café y té" },
            },
                ["Toiletries"] = new[]
            {
                new[] { "Shampoo", "Shampoo", "Shampooing", "Champú" },
                new[] { "Toothpaste", "Zahnpasta", "Dentifrice", "Pasta de dientes" },
                new[] { "Toothbrushes", "Zahnbürsten", "Brosses à dents", "Cepillos de dientes" },
                new[] { "Soap", "Seife", "Savon", "Jabón" },
                new[] { "Toilet paper", "Toilettenpapier", "Papier toilette", "Papel higiénico" },
                new[] { "Razors", "Rasierer", "Rasoirs", "Maquinillas de afeitar" },
                new[] { "Cosmetics", "Kosmetik", "Cosmétiques", "Cosméticos" },
                new[] { "Perfume", "Parfüm", "Parfum", "Perfume" },
                new[] { "Hair care", "Haarpflegeprodukte", "Soins capillaires", "Productos para el cabello" },
            },
                ["Stationery"] = new[]
            {
                new[] { "Notebooks", "Notizbücher", "Carnets", "Cuadernos" },
                new[] { "Pens", "Stifte", "Stylos", "Bolígrafos" },
                new[] { "Folders", "Ordner", "Classeurs", "Carpetas" },
                new[] { "Paper", "Papier", "Papier", "Papel" },
                new[] { "Envelopes", "Briefumschläge", "Enveloppes", "Sobres" },
                new[] { "Stamps", "Briefmarken", "Timbres", "Sellos" },
                new[] { "Stapler and hole punch", "Hefter und Locher", "Agrafeuse et perforatrice", "Grapadora y perforadora" },
                new[] { "Printer ink", "Druckerpatronen", "Cartouches d'encre", "Cartuchos de tinta" },
                new[] { "Scissors and glue", "Schere und Kleber", "Ciseaux et colle", "Tijeras y pegamento" },
                new[] { "Sticky notes", "Haftnotizen", "Notes autocollantes", "Notas adhesivas" },
            },
                ["Travel"] = new[]
            {
                new[] { "Suitcases", "Koffer", "Valises", "Maletas" },
                new[] { "Travel bags", "Reisetaschen", "Sacs de voyage", "Bolsas de viaje" },
                new[] { "Travel adapters", "Reiseadapter", "Adaptateurs de voyage", "Adaptadores de viaje" },
                new[] { "Travel pillow", "Nackenkissen", "Oreiller de voyage", "Almohada de viaje" },
                new[] { "Toiletry bag", "Kulturbeutel", "Trousse de toilette", "Neceser" },
                new[] { "Luggage tags", "Kofferanhänger", "Étiquettes de bagage", "Etiquetas de equipaje" },
                new[] { "Beach items", "Strandsachen", "Affaires de plage", "Artículos de playa" },
                new[] { "Packing cubes", "Packwürfel", "Organisateurs de valise", "Organizadores de maleta" },
            },
                ["Automotive"] = new[]
            {
                new[] { "Winter tires", "Winterreifen", "Pneus d'hiver", "Neumáticos de invierno" },
                new[] { "Summer tires", "Sommerreifen", "Pneus d'été", "Neumáticos de verano" },
                new[] { "Roof box", "Dachbox", "Coffre de toit", "Cofre de techo" },
                new[] { "Jumper cables", "Starthilfekabel", "Câbles de démarrage", "Cables de arranque" },
                new[] { "Warning triangle", "Warndreieck", "Triangle de signalisation", "Triángulo de emergencia" },
                new[] { "Car care products", "Autopflege", "Produits d'entretien auto", "Productos de limpieza para coche" },
                new[] { "Ice scraper", "Eiskratzer", "Grattoir à glace", "Rascador de hielo" },
                new[] { "Car mats", "Fußmatten", "Tapis de voiture", "Alfombrillas de coche" },
                new[] { "Motor oil", "Motoröl", "Huile moteur", "Aceite de motor" },
            },
                ["Party"] = new[]
            {
                new[] { "Party decorations", "Partydeko", "Décorations de fête", "Decoración de fiesta" },
                new[] { "Balloons", "Luftballons", "Ballons", "Globos" },
                new[] { "Costumes", "Kostüme", "Déguisements", "Disfraces" },
                new[] { "Paper plates and cups", "Pappteller und Becher", "Assiettes et gobelets en carton", "Platos y vasos de cartón" },
                new[] { "Birthday candles", "Geburtstagskerzen", "Bougies d'anniversaire", "Velas de cumpleaños" },
                new[] { "Party banners", "Girlanden", "Guirlandes de fête", "Guirnaldas" },
                new[] { "Party games", "Partyspiele", "Jeux de fête", "Juegos de fiesta" },
                new[] { "Fireworks", "Feuerwerk", "Feux d'artifice", "Fuegos artificiales" },
            },
                ["Seasonal"] = new[]
            {
                new[] { "Easter decorations", "Osterdeko", "Décorations de Pâques", "Decoración de Pascua" },
                new[] { "Halloween decorations", "Halloween-Deko", "Décorations d'Halloween", "Decoración de Halloween" },
                new[] { "Carnival costumes", "Faschingskostüme", "Déguisements de carnaval", "Disfraces de carnaval" },
                new[] { "Beach umbrella", "Sonnenschirm", "Parasol", "Sombrilla" },
                new[] { "Paddling pool", "Planschbecken", "Pataugeoire", "Piscina infantil" },
                new[] { "Sleds", "Schlitten", "Luges", "Trineos" },
                new[] { "Snow shovel", "Schneeschaufel", "Pelle à neige", "Pala para nieve" },
                new[] { "Hot water bottle", "Wärmflasche", "Bouillotte", "Bolsa de agua caliente" },
                new[] { "Easter baskets", "Osterkörbchen", "Paniers de Pâques", "Cestas de Pascua" },
            },
                ["Accessories"] = new[]
            {
                new[] { "Belts", "Gürtel", "Ceintures", "Cinturones" },
                new[] { "Scarves", "Schals", "Écharpes", "Bufandas" },
                new[] { "Sunglasses", "Sonnenbrillen", "Lunettes de soleil", "Gafas de sol" },
                new[] { "Eyeglasses", "Brillen", "Lunettes de vue", "Gafas" },
                new[] { "Bags and purses", "Taschen", "Sacs à main", "Bolsos" },
                new[] { "Wallets", "Geldbörsen", "Portefeuilles", "Carteras" },
                new[] { "Jewelry", "Schmuck", "Bijoux", "Joyas" },
                new[] { "Watches", "Armbanduhren", "Montres", "Relojes de pulsera" },
                new[] { "Umbrellas", "Regenschirme", "Parapluies", "Paraguas" },
                new[] { "Ties", "Krawatten", "Cravates", "Corbatas" },
                new[] { "Hair accessories", "Haarschmuck", "Accessoires pour cheveux", "Accesorios para el pelo" },
            },
                ["School"] = new[]
            {
                new[] { "School bags", "Schulranzen", "Cartables", "Mochilas escolares" },
                new[] { "Pencil case", "Federmappe", "Trousse", "Estuche" },
                new[] { "School notebooks", "Schulhefte", "Cahiers", "Cuadernos escolares" },
                new[] { "Art supplies", "Malstifte und Farben", "Matériel de dessin", "Material de dibujo" },
                new[] { "Lunch boxes", "Brotdosen", "Boîtes à goûter", "Fiambreras" },
                new[] { "Drinking bottles", "Trinkflaschen", "Gourdes", "Botellas de agua" },
                new[] { "Old homework", "Alte Hausaufgaben", "Anciens devoirs", "Tareas antiguas" },
                new[] { "School sports clothes", "Sportkleidung für die Schule", "Tenue de sport scolaire", "Ropa de deporte escolar" },
            },
                ["Hardware"] = new[]
            {
                new[] { "Nails", "Nägel", "Clous", "Clavos" },
                new[] { "Wall plugs", "Dübel", "Chevilles", "Tacos" },
                new[] { "Hooks", "Haken", "Crochets", "Ganchos" },
                new[] { "Hinges", "Scharniere", "Charnières", "Bisagras" },
                new[] { "Cable ties", "Kabelbinder", "Colliers de serrage", "Bridas" },
                new[] { "Padlocks", "Vorhängeschlösser", "Cadenas", "Candados" },
                new[] { "Nuts and bolts", "Schrauben und Muttern", "Écrous et boulons", "Tuercas y pernos" },
                new[] { "Glue", "Kleber", "Colle", "Pegamento" },
                new[] { "Adhesive tape", "Klebeband", "Ruban adhésif", "Cinta adhesiva" },
                new[] { "Keys", "Schlüssel", "Clés", "Llaves" },
            },
                ["Media"] = new[]
            {
                new[] { "DVDs", "DVDs", "DVD", "DVD" },
                new[] { "CDs", "CDs", "CD", "CD" },
                new[] { "Vinyl records", "Schallplatten", "Disques vinyle", "Discos de vinilo" },
                new[] { "Video games", "Videospiele", "Jeux vidéo", "Videojuegos" },
                new[] { "Blu-rays", "Blu-rays", "Blu-ray", "Blu-ray" },
                new[] { "Audio books", "Hörbücher", "Livres audio", "Audiolibros" },
                new[] { "Cassettes", "Kassetten", "Cassettes", "Casetes" },
                new[] { "USB sticks", "USB-Sticks", "Clés USB", "Memorias USB" },
                new[] { "Memory cards", "Speicherkarten", "Cartes mémoire", "Tarjetas de memoria" },
            },
            };

        // ------------------------------------------------------------------
        // Relation 2: Category (English name) -> typical REMARKS
        // ------------------------------------------------------------------
        public static readonly Dictionary<string, string[][]> RemarksByCategory =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Tools"] = new[]
            {
                new[] { "Cordless", "Akku", "Sans fil", "Inalámbrico" },
                new[] { "With case", "Mit Koffer", "Avec mallette", "Con maletín" },
                Complete, Rusty, Sharp,
            },
                ["Kitchenware"] = new[]
            {
                new[] { "Dishwasher safe", "Spülmaschinenfest", "Compatible lave-vaisselle", "Apto para lavavajillas" },
                new[] { "Set", "Set", "Lot", "Juego" },
                Complete, Sharp, Fragile,
            },
                ["Clothing"] = new[] { Winter, Summer, Washed, Kids, TooSmall },
                ["Bedding"] = new[] { Washed, Winter, Summer, Spare },
                ["Books"] = new[]
            {
                Kids,
                new[] { "Hardcover", "Gebunden", "Relié", "Tapa dura" },
                Valuable,
                new[] { "Signed", "Signiert", "Dédicacé", "Firmado" },
            },
                ["Toys"] = new[]
            {
                Kids, BatteriesIncl, Complete, Working,
                new[] { "Wooden", "Aus Holz", "En bois", "De madera" },
            },
                ["Electronics"] = new[] { BatteriesIncl, Working, WithWarranty, WithManual, Spare },
                ["Cables"] = new[]
            {
                Working, Spare,
                new[] { "Long", "Lang", "Long", "Largo" },
                new[] { "Short", "Kurz", "Court", "Corto" },
            },
                ["Documents"] = new[]
            {
                Important,
                new[] { "Original", "Original", "Original", "Original" },
                new[] { "Copy", "Kopie", "Copie", "Copia" },
                new[] { "Archive", "Archiv", "Archive", "Archivo" },
            },
                ["Decorations"] = new[]
            {
                Valuable, Fragile,
                new[] { "Seasonal", "Saisonal", "De saison", "De temporada" },
                Gift,
            },
                ["Christmas"] = new[]
            {
                Complete, Fragile, Working,
                new[] { "Glass", "Glas", "Verre", "Vidrio" },
            },
                ["Sports"] = new[] { Kids, Winter, Summer, Complete },
                ["Camping"] = new[] { Complete, Summer, Waterproof, Working },
                ["Bicycle"] = new[] { Kids, Spare, Working, Rusty },
                ["Gardening"] = new[] { Summer, Winter, Rusty, Working },
                ["Cleaning"] = new[] { Opened, Sealed, Spare, Expired },
                ["Paint"] = new[]
            {
                Opened, Sealed,
                new[] { "Dried out", "Eingetrocknet", "Sec", "Seco" },
                new[] { "Leftover", "Rest", "Reste", "Sobrante" },
            },
                ["Hobby"] = new[] { Complete, Valuable, Kids, Fragile },
                ["Medicine"] = new[] { Expired, Opened, Sealed, Kids },
                ["Memories"] = new[] { Valuable, Important, Fragile, Gift },
                ["Furniture"] = new[]
            {
                new[] { "Disassembled", "Zerlegt", "Démonté", "Desmontado" },
                Heavy, WithManual, Complete,
            },
                ["Appliances"] = new[] { Working, WithWarranty, WithManual, Spare },
                ["Lighting"] = new[]
            {
                Working, Spare, Fragile,
                new[] { "LED", "LED", "LED", "LED" },
            },
                ["Baby"] = new[] { Washed, Complete, TooSmall, Gift },
                ["Pets"] = new[] { Opened, Sealed, Expired, Washed },
                ["Food"] = new[]
            {
                Expired, Opened, Sealed,
                new[] { "Homemade", "Selbstgemacht", "Fait maison", "Casero" },
            },
                ["Toiletries"] = new[] { Opened, Sealed, Expired, Spare },
                ["Stationery"] = new[] { Complete, Spare, Sealed, Important },
                ["Travel"] = new[] { Complete, Waterproof, Spare, Washed },
                ["Automotive"] = new[] { Winter, Summer, Complete, Spare, Heavy },
                ["Party"] = new[]
            {
                new[] { "Reusable", "Wiederverwendbar", "Réutilisable", "Reutilizable" },
                Sealed, Kids, Complete,
            },
                ["Seasonal"] = new[] { Winter, Summer, Complete, Fragile },
                ["Accessories"] = new[] { Valuable, Gift, Kids, Fragile },
                ["School"] = new[] { Kids, Complete, Spare, Washed },
                ["Hardware"] = new[]
            {
                Sealed, Rusty, Spare,
                new[] { "Mixed", "Gemischt", "Assortis", "Surtido" },
            },
                ["Media"] = new[]
            {
                Working, Complete, Valuable,
                new[] { "Scratched", "Zerkratzt", "Rayé", "Rayado" },
            },
            };

        // ------------------------------------------------------------------
        // Relation 3: Location (English name) -> typical CATEGORIES (English names)
        // Drives Category chips in the box view AND item names for custom categories.
        // ------------------------------------------------------------------
        public static readonly Dictionary<string, string[]> CategoriesByLocation =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Garage"] = new[] { "Tools", "Bicycle", "Paint", "Sports", "Camping", "Gardening", "Automotive", "Hardware" },
                ["Basement"] = new[] { "Tools", "Cleaning", "Christmas", "Paint", "Documents", "Memories", "Food", "Hardware" },
                ["Attic"] = new[] { "Christmas", "Memories", "Decorations", "Clothing", "Bedding", "Books", "Seasonal", "Baby" },
                ["Kitchen"] = new[] { "Kitchenware", "Appliances", "Food", "Cleaning" },
                ["Living room"] = new[] { "Books", "Decorations", "Electronics", "Media", "Toys", "Cables", "Lighting", "Furniture" },
                ["Bedroom"] = new[] { "Clothing", "Bedding", "Books", "Accessories", "Lighting", "Furniture" },
                ["Children's room"] = new[] { "Toys", "Books", "Clothing", "School", "Baby" },
                ["Bathroom"] = new[] { "Toiletries", "Medicine", "Bedding", "Cleaning", "Appliances" },
                ["Office"] = new[] { "Documents", "Stationery", "Electronics", "Cables", "Books", "Media", "School" },
                ["Hallway"] = new[] { "Clothing", "Accessories", "Sports", "Bicycle", "Pets" },
                ["Garden"] = new[] { "Gardening", "Tools", "Sports", "Camping", "Seasonal", "Pets" },
                ["Shed"] = new[] { "Gardening", "Tools", "Bicycle", "Paint", "Hardware", "Seasonal" },
                ["Workshop"] = new[] { "Tools", "Paint", "Hardware", "Hobby", "Electronics", "Cables" },
                ["Storage room"] = new[] { "Christmas", "Camping", "Sports", "Decorations", "Tools", "Seasonal", "Travel", "Party" },
                ["Laundry room"] = new[] { "Cleaning", "Bedding", "Clothing", "Appliances" },
                ["Pantry"] = new[] { "Food", "Kitchenware", "Cleaning", "Appliances" },
                ["Balcony"] = new[] { "Gardening", "Bicycle", "Camping", "Seasonal" },
                ["Terrace"] = new[] { "Gardening", "Camping", "Decorations", "Seasonal", "Furniture" },
                ["Wardrobe"] = new[] { "Clothing", "Bedding", "Accessories" },
                ["Car"] = new[] { "Automotive", "Tools", "Medicine", "Documents", "Cleaning", "Baby", "Travel" },
                ["Dining room"] = new[] { "Kitchenware", "Decorations", "Furniture", "Lighting" },
                ["Guest room"] = new[] { "Bedding", "Clothing", "Furniture", "Books" },
                ["Nursery"] = new[] { "Baby", "Toys", "Clothing", "Bedding", "Lighting" },
                ["Playroom"] = new[] { "Toys", "Books", "Media", "School" },
                ["Hobby room"] = new[] { "Hobby", "Books", "Media", "Stationery", "Electronics" },
                ["Entrance"] = new[] { "Clothing", "Accessories", "Sports", "Pets", "Hardware" },
                ["Toilet"] = new[] { "Toiletries", "Cleaning" },
                ["Under the stairs"] = new[] { "Cleaning", "Appliances", "Tools", "Sports" },
                ["Boiler room"] = new[] { "Cleaning", "Tools", "Hardware", "Documents" },
                ["Carport"] = new[] { "Bicycle", "Automotive", "Gardening", "Seasonal" },
                ["Greenhouse"] = new[] { "Gardening", "Tools" },
                ["Holiday home"] = new[] { "Kitchenware", "Bedding", "Camping", "Sports", "Books", "Travel" },
                ["Car trunk"] = new[] { "Automotive", "Tools", "Medicine", "Camping", "Sports" },
                ["Cupboard"] = new[] { "Kitchenware", "Food", "Cleaning", "Documents", "Medicine" },
                ["Shelf"] = new[] { "Books", "Media", "Decorations", "Documents", "Toys" },
                ["Drawer"] = new[] { "Stationery", "Cables", "Documents", "Hardware", "Accessories", "Kitchenware" },
                ["Toolbox"] = new[] { "Tools", "Hardware" },
                ["Suitcase"] = new[] { "Travel", "Clothing", "Toiletries", "Documents" },
            };
    }
}
