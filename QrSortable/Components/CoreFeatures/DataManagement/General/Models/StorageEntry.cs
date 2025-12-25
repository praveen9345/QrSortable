namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using System;

    public class StorageEntry : DatabaseEntry
    {
        public Guid StorageId { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BarcodeValue { get; set; }
        public string BarcodeType { get; set; }
        public string Location { get; set; }
        public string SearchInfo { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public IList<byte[]> ImageList { get; set; }

        public string BackgroundColorHex { get; set; }

        public StorageEntry() 
        {
            StorageId = Guid.NewGuid();

        }
    }
}
