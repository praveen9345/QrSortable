namespace QrSortable.Components.CoreFeatures.DataManagement.Backend.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    public class DtoSubscriptionEntityModel : DatabaseEntry
    {
        public string IsUpdateData { get; set; } = "false";
        public bool IsSubscribed { get; set; }
        public string CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public string SubscriptionId { get; set; }
        public string Email { get; set; }
    }
}
