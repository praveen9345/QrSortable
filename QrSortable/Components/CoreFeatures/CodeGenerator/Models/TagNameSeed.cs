using QrSortable.Components.CoreFeatures.Scanner.Models;

namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{
    /// <summary>
    /// Compiled-in Tag Name suggestions for households, moving and small businesses
    /// (rooms, storage places, item categories, moving-day labels, stock and paperwork).
    /// Every term is listed in this order: English, German, French, Spanish.
    /// Other app languages fall back to English (add a column + extend LangIndex to support more).
    ///
    /// Typing matches against ALL languages, but the chip is shown in the app language.
    /// The three groups are interleaved, so the first chips (nothing typed yet) show a mix.
    /// Rule: the English text is the term's identity and must be unique across all groups.
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
            new[] { "Dining room", "Esszimmer", "Salle à manger", "Comedor" },
            new[] { "Hallway", "Flur", "Couloir", "Pasillo" },
            new[] { "Guest room", "Gästezimmer", "Chambre d'amis", "Habitación de invitados" },
            new[] { "Nursery", "Babyzimmer", "Chambre de bébé", "Habitación del bebé" },
            new[] { "Playroom", "Spielzimmer", "Salle de jeux", "Sala de juegos" },
            new[] { "Laundry room", "Waschküche", "Buanderie", "Lavadero" },
            new[] { "Storage room", "Abstellraum", "Débarras", "Trastero" },
            new[] { "Utility room", "Hauswirtschaftsraum", "Arrière-cuisine", "Cuarto de servicio" },
            new[] { "Balcony", "Balkon", "Balcon", "Balcón" },
            new[] { "Terrace", "Terrasse", "Terrasse", "Terraza" },
            new[] { "Shed", "Gartenhaus", "Abri de jardin", "Cobertizo" },
            new[] { "Workshop", "Werkstatt", "Atelier", "Taller" },
            new[] { "Wardrobe", "Kleiderschrank", "Penderie", "Armario" },
            new[] { "Toilet", "WC", "Toilettes", "Aseo" },
            new[] { "Entrance", "Eingang", "Entrée", "Entrada" },
            new[] { "Under the stairs", "Unter der Treppe", "Sous l'escalier", "Bajo la escalera" },
            new[] { "Boiler room", "Heizungsraum", "Chaufferie", "Cuarto de calderas" },
            new[] { "Carport", "Carport", "Abri voiture", "Cochera" },
            new[] { "Greenhouse", "Gewächshaus", "Serre", "Invernadero" },
            new[] { "Holiday home", "Ferienhaus", "Maison de vacances", "Casa de vacaciones" },
            new[] { "Hobby room", "Hobbyraum", "Salle de loisirs", "Sala de aficiones" },
            new[] { "Home office", "Homeoffice", "Bureau à domicile", "Oficina en casa" },
            new[] { "Master bedroom", "Elternschlafzimmer", "Chambre parentale", "Dormitorio principal" },
            new[] { "Suitcase", "Koffer", "Valise", "Maleta" },
            new[] { "Toolbox", "Werkzeugkasten", "Boîte à outils", "Caja de herramientas" },
            new[] { "Tools", "Werkzeug", "Outils", "Herramientas" },
            new[] { "Power tools", "Elektrowerkzeug", "Outils électriques", "Herramientas eléctricas" },
            new[] { "Hand tools", "Handwerkzeug", "Outils à main", "Herramientas manuales" },
            new[] { "Screws and nails", "Schrauben und Nägel", "Vis et clous", "Tornillos y clavos" },
            new[] { "Hardware", "Eisenwaren", "Quincaillerie", "Ferretería" },
            new[] { "Paint", "Farben", "Peinture", "Pintura" },
            new[] { "Wallpaper", "Tapeten", "Papier peint", "Papel pintado" },
            new[] { "Light bulbs", "Glühbirnen", "Ampoules", "Bombillas" },
            new[] { "Lamps", "Lampen", "Lampes", "Lámparas" },
            new[] { "Batteries", "Batterien", "Piles", "Pilas" },
            new[] { "Cables", "Kabel", "Câbles", "Cables" },
            new[] { "Extension cords", "Verlängerungskabel", "Rallonges", "Alargadores" },
            new[] { "Electronics", "Elektronik", "Électronique", "Electrónica" },
            new[] { "Computer accessories", "Computerzubehör", "Accessoires informatiques", "Accesorios de informática" },
            new[] { "Printer supplies", "Druckerzubehör", "Fournitures d'imprimante", "Suministros de impresora" },
            new[] { "Old phones", "Alte Handys", "Anciens téléphones", "Teléfonos antiguos" },
            new[] { "Video games", "Videospiele", "Jeux vidéo", "Videojuegos" },
            new[] { "DVDs and CDs", "DVDs und CDs", "DVD et CD", "DVD y CD" },
            new[] { "Vinyl records", "Schallplatten", "Disques vinyles", "Discos de vinilo" },
            new[] { "Books", "Bücher", "Livres", "Libros" },
            new[] { "Magazines", "Zeitschriften", "Magazines", "Revistas" },
            new[] { "Toys", "Spielzeug", "Jouets", "Juguetes" },
            new[] { "Board games", "Brettspiele", "Jeux de société", "Juegos de mesa" },
            new[] { "Puzzles", "Puzzles", "Puzzles", "Puzles" },
            new[] { "Baby items", "Babysachen", "Affaires de bébé", "Cosas de bebé" },
            new[] { "Baby clothes", "Babykleidung", "Vêtements de bébé", "Ropa de bebé" },
            new[] { "Kids clothes", "Kinderkleidung", "Vêtements d'enfant", "Ropa infantil" },
            new[] { "Clothes", "Kleidung", "Vêtements", "Ropa" },
            new[] { "Winter clothes", "Winterkleidung", "Vêtements d'hiver", "Ropa de invierno" },
            new[] { "Summer clothes", "Sommerkleidung", "Vêtements d'été", "Ropa de verano" },
            new[] { "Shoes", "Schuhe", "Chaussures", "Zapatos" },
            new[] { "Jackets", "Jacken", "Vestes", "Chaquetas" },
            new[] { "Hats and scarves", "Mützen und Schals", "Bonnets et écharpes", "Gorros y bufandas" },
            new[] { "Handbags", "Handtaschen", "Sacs à main", "Bolsos" },
            new[] { "Jewelry", "Schmuck", "Bijoux", "Joyas" },
            new[] { "Cosmetics", "Kosmetik", "Cosmétiques", "Cosméticos" },
            new[] { "Bedding", "Bettwäsche", "Linge de lit", "Ropa de cama" },
            new[] { "Towels", "Handtücher", "Serviettes", "Toallas" },
            new[] { "Blankets", "Decken", "Couvertures", "Mantas" },
            new[] { "Pillows", "Kopfkissen", "Oreillers", "Almohadas" },
            new[] { "Curtains", "Vorhänge", "Rideaux", "Cortinas" },
            new[] { "Rugs", "Teppiche", "Tapis", "Alfombras" },
            new[] { "Table linen", "Tischwäsche", "Linge de table", "Mantelería" },
            new[] { "Cutlery", "Besteck", "Couverts", "Cubiertos" },
            new[] { "Plates and bowls", "Teller und Schüsseln", "Assiettes et bols", "Platos y cuencos" },
            new[] { "Glasses and cups", "Gläser und Tassen", "Verres et tasses", "Vasos y tazas" },
            new[] { "Pots and pans", "Töpfe und Pfannen", "Casseroles et poêles", "Ollas y sartenes" },
            new[] { "Baking supplies", "Backzubehör", "Matériel de pâtisserie", "Utensilios de repostería" },
            new[] { "Small appliances", "Kleingeräte", "Petits appareils", "Pequeños electrodomésticos" },
            new[] { "Spices", "Gewürze", "Épices", "Especias" },
            new[] { "Canned food", "Konserven", "Conserves", "Conservas" },
            new[] { "Dry food", "Trockenvorräte", "Produits secs", "Alimentos secos" },
            new[] { "Drinks", "Getränke", "Boissons", "Bebidas" },
            new[] { "Decorations", "Dekoration", "Décoration", "Decoración" },
            new[] { "Candles", "Kerzen", "Bougies", "Velas" },
            new[] { "Vases", "Vasen", "Vases", "Jarrones" },
            new[] { "Picture frames", "Bilderrahmen", "Cadres photo", "Marcos de fotos" },
            new[] { "Photos and memories", "Fotos und Erinnerungen", "Photos et souvenirs", "Fotos y recuerdos" },
            new[] { "Souvenirs", "Souvenirs", "Souvenirs", "Souvenirs" },
            new[] { "Documents", "Dokumente", "Documents", "Documentos" },
            new[] { "Important documents", "Wichtige Dokumente", "Documents importants", "Documentos importantes" },
            new[] { "Receipts", "Belege", "Reçus", "Recibos" },
            new[] { "Warranties", "Garantien", "Garanties", "Garantías" },
            new[] { "Manuals", "Bedienungsanleitungen", "Notices d'utilisation", "Manuales" },
            new[] { "Spare keys", "Ersatzschlüssel", "Clés de rechange", "Llaves de repuesto" },
            new[] { "Emergency kit", "Notfallset", "Kit d'urgence", "Kit de emergencia" },
            new[] { "First aid", "Erste Hilfe", "Premiers secours", "Primeros auxilios" },
            new[] { "Medicine", "Medikamente", "Médicaments", "Medicamentos" },
            new[] { "Cleaning supplies", "Putzmittel", "Produits d'entretien", "Productos de limpieza" },
            new[] { "Stationery", "Schreibwaren", "Papeterie", "Papelería" },
            new[] { "School supplies", "Schulsachen", "Fournitures scolaires", "Material escolar" },
            new[] { "Craft supplies", "Bastelmaterial", "Matériel de bricolage créatif", "Material de manualidades" },
            new[] { "Sewing supplies", "Nähzubehör", "Fournitures de couture", "Material de costura" },
            new[] { "Art supplies", "Künstlerbedarf", "Matériel d'art", "Material de arte" },
            new[] { "Musical instruments", "Musikinstrumente", "Instruments de musique", "Instrumentos musicales" },
            new[] { "Christmas", "Weihnachten", "Noël", "Navidad" },
            new[] { "Christmas lights", "Weihnachtsbeleuchtung", "Illuminations de Noël", "Luces de Navidad" },
            new[] { "Easter", "Ostern", "Pâques", "Pascua" },
            new[] { "Halloween", "Halloween", "Halloween", "Halloween" },
            new[] { "Party supplies", "Partybedarf", "Articles de fête", "Artículos de fiesta" },
            new[] { "Gift wrap", "Geschenkpapier", "Papier cadeau", "Papel de regalo" },
            new[] { "Gifts", "Geschenke", "Cadeaux", "Regalos" },
            new[] { "Garden tools", "Gartengeräte", "Outils de jardin", "Herramientas de jardín" },
            new[] { "Garden furniture", "Gartenmöbel", "Meubles de jardin", "Muebles de jardín" },
            new[] { "Flower pots", "Blumentöpfe", "Pots de fleurs", "Macetas" },
            new[] { "Seeds and fertilizer", "Samen und Dünger", "Graines et engrais", "Semillas y fertilizante" },
            new[] { "Barbecue", "Grill", "Barbecue", "Barbacoa" },
            new[] { "Pool toys", "Poolspielzeug", "Jouets de piscine", "Juguetes de piscina" },
            new[] { "Camping", "Camping", "Camping", "Camping" },
            new[] { "Sports", "Sport", "Sport", "Deporte" },
            new[] { "Ski gear", "Skiausrüstung", "Équipement de ski", "Equipo de esquí" },
            new[] { "Hiking gear", "Wanderausrüstung", "Équipement de randonnée", "Equipo de senderismo" },
            new[] { "Fishing gear", "Angelausrüstung", "Matériel de pêche", "Equipo de pesca" },
            new[] { "Bike gear", "Fahrradzubehör", "Accessoires de vélo", "Accesorios de bicicleta" },
            new[] { "Car accessories", "Autozubehör", "Accessoires auto", "Accesorios de coche" },
            new[] { "Pets", "Haustiere", "Animaux", "Mascotas" },
            new[] { "Recycling", "Recycling", "Recyclage", "Reciclaje" },
            new[] { "Seasonal items", "Saisonartikel", "Articles de saison", "Artículos de temporada" },
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
            new[] { "Handle with care", "Vorsicht", "Manipuler avec précaution", "Manejar con cuidado" },
            new[] { "First night box", "Erste-Nacht-Box", "Carton de première nuit", "Caja de la primera noche" },
            new[] { "Last box", "Letzte Kiste", "Dernier carton", "Última caja" },
            new[] { "Do not open", "Nicht öffnen", "Ne pas ouvrir", "No abrir" },
            new[] { "Load first", "Zuerst einladen", "Charger en premier", "Cargar primero" },
            new[] { "Load last", "Zuletzt einladen", "Charger en dernier", "Cargar al final" },
            new[] { "Unload first", "Zuerst ausladen", "Décharger en premier", "Descargar primero" },
            new[] { "Keep upright", "Aufrecht lagern", "Garder à la verticale", "Mantener vertical" },
            new[] { "To kitchen", "In die Küche", "Pour la cuisine", "A la cocina" },
            new[] { "To bedroom", "Ins Schlafzimmer", "Pour la chambre", "Al dormitorio" },
            new[] { "To living room", "Ins Wohnzimmer", "Pour le salon", "Al salón" },
            new[] { "To bathroom", "Ins Badezimmer", "Pour la salle de bain", "Al baño" },
            new[] { "To children's room", "Ins Kinderzimmer", "Pour la chambre d'enfant", "A la habitación infantil" },
            new[] { "To office", "Ins Büro", "Pour le bureau", "A la oficina" },
            new[] { "To garage", "In die Garage", "Pour le garage", "Al garaje" },
            new[] { "To basement", "In den Keller", "Pour la cave", "Al sótano" },
            new[] { "To attic", "Auf den Dachboden", "Pour le grenier", "Al ático" },
            new[] { "Kitchen essentials", "Küchenbedarf", "Essentiels de cuisine", "Básicos de cocina" },
            new[] { "Bathroom essentials", "Badezimmerbedarf", "Essentiels de salle de bain", "Básicos de baño" },
            new[] { "Pictures and mirrors", "Bilder und Spiegel", "Tableaux et miroirs", "Cuadros y espejos" },
            new[] { "Coats", "Mäntel", "Manteaux", "Abrigos" },
            new[] { "Plants", "Pflanzen", "Plantes", "Plantas" },
            new[] { "TV and accessories", "Fernseher und Zubehör", "Télévision et accessoires", "Televisor y accesorios" },
            new[] { "Paperwork", "Papierkram", "Paperasse", "Papeleo" },
            new[] { "Keys", "Schlüssel", "Clés", "Llaves" },
            new[] { "Furniture parts", "Möbelteile", "Pièces de meubles", "Piezas de muebles" },
            new[] { "Screws and bolts", "Schrauben und Bolzen", "Vis et boulons", "Tornillos y pernos" },
            new[] { "Bed parts", "Bettteile", "Pièces de lit", "Piezas de cama" },
            new[] { "Temporary storage", "Zwischenlager", "Stockage temporaire", "Almacenamiento temporal" },
            new[] { "Long-term storage", "Langzeitlagerung", "Stockage longue durée", "Almacenamiento prolongado" },
            new[] { "Garage sale", "Flohmarkt", "Vide-grenier", "Venta de garaje" },
            new[] { "Recycle", "Zum Recycling", "À recycler", "Para reciclar" },
            new[] { "Throw away", "Wegwerfen", "À jeter", "Tirar" },
            new[] { "Needs repair", "Reparaturbedürftig", "À réparer", "Necesita reparación" },
            new[] { "Moving documents", "Umzugsunterlagen", "Documents de déménagement", "Documentos de la mudanza" },
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
            new[] { "Warehouse", "Lagerhalle", "Entrepôt", "Almacén" },
            new[] { "Stockroom", "Lagerraum", "Réserve", "Depósito" },
            new[] { "Shop floor", "Verkaufsfläche", "Surface de vente", "Zona de venta" },
            new[] { "Showroom", "Showroom", "Salle d'exposition", "Sala de exposición" },
            new[] { "Display items", "Ausstellungsstücke", "Articles d'exposition", "Artículos de exposición" },
            new[] { "Counter", "Theke", "Comptoir", "Mostrador" },
            new[] { "Cash register", "Kasse", "Caisse", "Caja registradora" },
            new[] { "Packing station", "Packstation", "Poste d'emballage", "Estación de embalaje" },
            new[] { "Back office", "Verwaltungsbüro", "Bureau administratif", "Oficina administrativa" },
            new[] { "Packing materials", "Verpackungsmaterial", "Matériel d'emballage", "Material de embalaje" },
            new[] { "Boxes and bags", "Kartons und Beutel", "Cartons et sachets", "Cajas y bolsas" },
            new[] { "Bubble wrap", "Luftpolsterfolie", "Film à bulles", "Plástico de burbujas" },
            new[] { "Packaging tape", "Paketband", "Ruban d'emballage", "Cinta de embalaje" },
            new[] { "Shipping labels", "Versandetiketten", "Étiquettes d'expédition", "Etiquetas de envío" },
            new[] { "Labels", "Etiketten", "Étiquettes", "Etiquetas" },
            new[] { "Delivery notes", "Lieferscheine", "Bons de livraison", "Albaranes" },
            new[] { "Purchase orders", "Einkaufsbestellungen", "Bons de commande", "Órdenes de compra" },
            new[] { "Pending orders", "Offene Bestellungen", "Commandes en attente", "Pedidos pendientes" },
            new[] { "Completed orders", "Erledigte Bestellungen", "Commandes terminées", "Pedidos completados" },
            new[] { "Pre-orders", "Vorbestellungen", "Précommandes", "Preventas" },
            new[] { "Customer returns", "Kundenretouren", "Retours clients", "Devoluciones de clientes" },
            new[] { "Damaged goods", "Beschädigte Ware", "Marchandises endommagées", "Mercancía dañada" },
            new[] { "Sale items", "Sonderangebote", "Articles en promotion", "Artículos en oferta" },
            new[] { "New arrivals", "Neuheiten", "Nouveautés", "Novedades" },
            new[] { "Best sellers", "Bestseller", "Meilleures ventes", "Más vendidos" },
            new[] { "Seasonal stock", "Saisonware", "Stock saisonnier", "Existencias de temporada" },
            new[] { "Clothing stock", "Kleiderbestand", "Stock de vêtements", "Existencias de ropa" },
            new[] { "Handmade goods", "Handgemachte Produkte", "Produits faits main", "Productos hechos a mano" },
            new[] { "Parts and components", "Teile und Komponenten", "Pièces et composants", "Piezas y componentes" },
            new[] { "Prototypes", "Prototypen", "Prototypes", "Prototipos" },
            new[] { "Ingredients", "Zutaten", "Ingrédients", "Ingredientes" },
            new[] { "Cafe supplies", "Cafébedarf", "Fournitures de café", "Suministros de cafetería" },
            new[] { "Kitchen equipment", "Küchenausrüstung", "Équipement de cuisine", "Equipo de cocina" },
            new[] { "Salon supplies", "Salonbedarf", "Fournitures de salon", "Suministros de salón" },
            new[] { "Beauty products", "Schönheitsprodukte", "Produits de beauté", "Productos de belleza" },
            new[] { "Medical supplies", "Medizinischer Bedarf", "Fournitures médicales", "Suministros médicos" },
            new[] { "Customer files", "Kundenakten", "Dossiers clients", "Expedientes de clientes" },
            new[] { "Contracts", "Verträge", "Contrats", "Contratos" },
            new[] { "Quotes", "Angebote", "Devis", "Presupuestos" },
            new[] { "Supplier documents", "Lieferantenunterlagen", "Documents fournisseurs", "Documentos de proveedores" },
            new[] { "Tax documents", "Steuerunterlagen", "Documents fiscaux", "Documentos fiscales" },
            new[] { "Payroll", "Lohnabrechnung", "Paie", "Nóminas" },
            new[] { "HR files", "Personalakten", "Dossiers du personnel", "Expedientes de personal" },
            new[] { "Licenses and permits", "Lizenzen und Genehmigungen", "Licences et permis", "Licencias y permisos" },
            new[] { "Insurance", "Versicherung", "Assurance", "Seguros" },
            new[] { "Promotional items", "Werbeartikel", "Articles promotionnels", "Artículos promocionales" },
            new[] { "Flyers", "Flyer", "Flyers", "Folletos" },
            new[] { "Business cards", "Visitenkarten", "Cartes de visite", "Tarjetas de visita" },
            new[] { "Banners", "Banner", "Banderoles", "Pancartas" },
            new[] { "Event supplies", "Eventbedarf", "Matériel d'événement", "Material para eventos" },
            new[] { "Trade show materials", "Messematerial", "Matériel de salon", "Material de feria" },
            new[] { "IT equipment", "IT-Geräte", "Matériel informatique", "Equipos informáticos" },
            new[] { "Printers", "Drucker", "Imprimantes", "Impresoras" },
            new[] { "Cables and adapters", "Kabel und Adapter", "Câbles et adaptateurs", "Cables y adaptadores" },
            new[] { "Office furniture", "Büromöbel", "Mobilier de bureau", "Muebles de oficina" },
            new[] { "Uniforms", "Uniformen", "Uniformes", "Uniformes" },
            new[] { "Workwear", "Arbeitskleidung", "Vêtements de travail", "Ropa de trabajo" },
            new[] { "Safety equipment", "Sicherheitsausrüstung", "Équipement de sécurité", "Equipo de seguridad" },
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
