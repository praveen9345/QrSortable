namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models
{
    using Google.Cloud.Firestore;

    /// <summary>
    /// Firestore persistence model for Orders.
    /// </summary>
    [FirestoreData]
    public class DtoOrdersModel : DtoFirestoreData
    {
        /// <summary>
        /// Firestore document id (assigned). Must be get for Firestore SDK.
        /// </summary>
        [FirestoreDocumentId]
        public override string MultiuserId { get; } //multiple user id
        /// <summary>
        /// Collection name for these documents.
        /// </summary>
        public override string CollectionName => "Orders";

        /// <summary>
        /// Order id of the product.
        /// </summary>
        [FirestoreProperty]
        public string OrderId { get; set; }
        /// <summary>
        /// Title of the product.
        /// </summary>
        [FirestoreProperty]
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the product.
        /// </summary>
        [FirestoreProperty]
        public string Description { get; set; }

        /// <summary>
        /// Type of code to generate (e.g., "QR code" or "Bar code").
        /// </summary>
        [FirestoreProperty]
        public string CodeType { get; set; }

        /// <summary>
        /// Page type (e.g., "A4(12 code)" or "A5(6 code)").
        /// </summary>
        [FirestoreProperty]
        public string PageType { get; set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        [FirestoreProperty]
        public string ProductQuantity { get; set; }

        /// <summary>
        /// Date and  of the product added to basket.
        /// </summary>
        [FirestoreProperty]
        public string DateTime { get; set; }

        /// <summary>
        /// Total price for the order.
        /// </summary>
        [FirestoreProperty]
        public string TotalPrice { get; set; }

        /// <summary>
        /// Customer's full name.
        /// </summary>
        [FirestoreProperty]
        public string Name { get; set; }

        /// <summary>
        /// Street name.
        /// </summary>
        [FirestoreProperty]
        public string Street { get; set; }

        /// <summary>
        /// House number.
        /// </summary>
        [FirestoreProperty]
        public string HouseNo { get; set; }

        /// <summary>
        /// ZIP or postal code.
        /// </summary>
        [FirestoreProperty]
        public string ZipCode { get; set; }

        /// <summary>
        /// City name.
        /// </summary>
        [FirestoreProperty]
        public string City { get; set; }

        /// <summary>
        /// Country name.
        /// </summary>
        [FirestoreProperty]
        public string Country { get; set; }

        /// <summary>
        /// Customer's email address.
        /// </summary>
        [FirestoreProperty]
        public string Email { get; set; }

        /// <summary>
        /// Referance code.
        /// </summary>
        [FirestoreProperty]
        public string ReferenceCode { get; set; }

        /// <summary>
        /// Shipment tracking number.
        /// </summary>
        [FirestoreProperty]
        public string ShipmentTracking { get; set; }

        /// <summary>
        /// Status of the order.
        /// </summary>
        [FirestoreProperty]
        public string StatusOfOrder { get; set; }

        /// <summary>
        /// List of generated PDFs stored as byte arrays.
        /// </summary>
        [FirestoreProperty]
        public List<byte[]> PdfFiles { get; set; } = new();
    }
}
