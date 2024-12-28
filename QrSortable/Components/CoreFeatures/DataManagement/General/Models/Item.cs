namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    public class Item
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public List<byte[]> Images { get; set; } = new List<byte[]>();
    }
}
