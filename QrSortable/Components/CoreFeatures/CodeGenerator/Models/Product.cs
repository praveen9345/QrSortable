namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{

    public class Product
    {
        /// <summary>
        /// .........................
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// .................
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// .................
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// ...............................
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// ...........................
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Type of code to generate (e.g., "QR code" or "Bar code").
        /// </summary>
        public string CodeType { get; set; } = "QR code";

        /// <summary>
        /// Page type (e.g., "A4(12 code)" or "A5(6 code)").
        /// </summary>
        public string PageType { get; set; }

        /// <summary>
        /// Tag name associated with the code (e.g., "kitchen").
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Hex color for the code (default is black).
        /// </summary>
        public string ColorHex { get; set; } = "#000000";

        /// <summary>
        /// Number of pages to generate.
        /// </summary>
        public int NumberOfPages { get; set; } = 1;

        /// <summary>
        /// Total price for the request.
        /// </summary>
        public decimal TotalPrice { get; set; } = 0m;
    }

}
