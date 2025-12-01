namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    public class AddToBasketData : DatabaseEntry
    {
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
        public decimal TotalPrice { get; set; } = 0m;

    }
}
