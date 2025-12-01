namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    public class YoursOrderData : DatabaseEntry
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
        /// Quantity of the product.
        /// </summary>
        public int ProductQuantity { get; set; }

        /// <summary>
        /// Date and  of the product added to basket.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Total price for the request.
        /// </summary>
        public string TotalPrice { get; set; }

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

        /// <summary>
        /// Referance code.
        /// </summary>
        public string ReferenceCode { get; set; }

        /// <summary>
        /// Shipment tracking number.
        /// </summary>
        public string ShipmentTracking { get; set; }

        /// <summary>
        /// Status of the order.
        /// </summary>
        public string StatusOfOrder { get; set; }

    }
}
