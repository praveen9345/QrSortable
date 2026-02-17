namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
   using QrSortable.Components.CoreFeatures.DataManagement.Models;

   public class SubscriptionEntity : DatabaseEntry
    {
        public bool IsSubscribed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public string SubscriptionId { get; set; }

    }

}
