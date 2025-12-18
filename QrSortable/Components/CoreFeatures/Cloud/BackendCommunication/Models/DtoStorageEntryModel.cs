namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models
{
    using Google.Cloud.Firestore;

    /// <summary>
    /// Firestore persistence model for StorageEntry.
    /// </summary>
    [FirestoreData]
    public class DtoStorageEntryModel : DtoFirestoreData
    {
        /// <summary>
        /// Firestore document id (assigned). Must be get for Firestore SDK.
        /// </summary>
        [FirestoreDocumentId]
        public override string MultiuserId { get; set; } //multiple user id

        /// <summary>
        /// Collection name for these documents.
        /// </summary>
        public override string CollectionName => "StorageEntries";

        /// <summary>
        /// Stored as string for Firestore (Guid is not a native type).
        /// </summary>
        [FirestoreProperty]
        public string StorageId { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string Category { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string CreatedDate { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string BarcodeValue { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string BarcodeType { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string Location { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string SearchInfo { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string ItemName { get; set; }

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string Description { get; set; }

        /// <summary>
        /// Firestore supports byte[]; list is stored as array of blobs.
        /// </summary>
        [FirestoreProperty]
        public IList<String> ImageUrls { get; set; } = new List<string>();

        /// <summary>
        /// .......................
        /// </summary>
        [FirestoreProperty]
        public string BackgroundColorHex { get; set; }

    }
}
