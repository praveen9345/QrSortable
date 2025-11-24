namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{

    public class Product
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public bool IsNew { get; set; } = false;
        public string ImageUrl { get; set; }
    }

}
