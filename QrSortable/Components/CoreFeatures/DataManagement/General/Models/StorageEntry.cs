namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    public class StorageEntry : DatabaseEntry
    {
        public Guid StorageId { get; private set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BarcodeValue { get; set; }
        public string BarcodeType { get; set; }
        public string Location { get; set; }
        public string SearchInfo { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    
        public StorageEntry() 
        {
            StorageId = Guid.NewGuid();
        }
    
    }
}
