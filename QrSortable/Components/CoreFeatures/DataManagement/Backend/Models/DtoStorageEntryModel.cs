namespace QrSortable.Components.CoreFeatures.DataManagement.Backend.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    public class DtoStorageEntryModel : DatabaseEntry
    {
        /// <summary>
        /// Order data is for instert or update of the product.
        /// </summary>
        public bool IsUpdateData { get; set; } = false;

        public string StorageId { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string BarcodeValue { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string BarcodeType { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string SearchInfo { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Firestore supports byte[]; list is stored as array of blobs.
        /// </summary>
        public IList<byte[]> ImageList { get; set; } = new List<byte[]>();
        /// <summary>
        /// .......................
        /// </summary>
        public string BackgroundColorHex { get; set; }
    }
}
