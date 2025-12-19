namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models
{
    public abstract class FirestoreData
    {
        public abstract string MultiuserId { get; set; }
        public abstract string CollectionName { get; }
    }
}