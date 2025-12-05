namespace QrSortable.Components.CoreFeatures.OrdersPayments.Models
{
    public class OrderedData
    {
        /// <summary>
        /// Order id of the product.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Date and time of the ordered item.
        /// </summary>
        private DateTime OrderDateTime { get; set; }

        /// <summary>
        /// Title of the product.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL of the product image.
        /// </summary>
        public string ImageUrl { get; set; } = "image_icon";

        /// <summary>
        /// Status of the order.
        /// </summary>
        public string StatusOfOrder { get; set; }

        /// <summary>
        /// Enable the of StatusOfOrder of the order.
        /// </summary>
        public bool IsEnabelStatusOfOrder { get; set; } = false;

        /// <summary>
        /// Shipment tracking number.
        /// </summary>
        public string ShipmentTracking { get; set; }

        /// <summary>
        /// Total price for the order.
        /// </summary>
        public string TotalPrice { get; set; }
    }
}
