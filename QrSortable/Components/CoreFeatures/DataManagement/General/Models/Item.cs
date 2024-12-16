namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    public class Item
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public List<string> ImagesFilePath { get; set; } = new List<string>();
    }
}
