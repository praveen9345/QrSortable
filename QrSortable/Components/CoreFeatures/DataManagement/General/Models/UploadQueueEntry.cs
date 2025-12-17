namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    /// <summary>
    /// Persistent queue entry for failed backend uploads.
    /// Payload stores the serialized DTO (JSON). DtoType is optionally stored to aid deserialization.
    /// </summary>
    public class UploadQueueEntry : DatabaseEntry
    {
        public string MultiuserId { get; set; }
        public string CollectionName { get; set; }

        /// <summary>
        /// Full CLR type name of DTO (optional). Used to deserialize payload back to concrete DTO.
        /// </summary>
        public string DtoTypeName { get; set; }

        /// <summary>
        /// JSON payload of the DTO (serialized by JsonSerializer).
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Number of failed attempts already performed.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Time when queued.
        /// </summary>
        public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
    }
}