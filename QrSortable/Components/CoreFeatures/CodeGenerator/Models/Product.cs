namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{

    public class Product
    {
        /// <summary>
        /// Order id of the product.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Title of the product.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Price of the product.
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Indicates if the product is new.
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// URL of the product image.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

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

        /// <summary>
        /// Customer's full name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Street name.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// House number.
        /// </summary>
        public string HouseNo { get; set; }

        /// <summary>
        /// ZIP or postal code.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// City name.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Country name.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Customer's email address.
        /// </summary>
        public string Email { get; set; }

    }

}
