namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models
{
    using Google.Cloud.Firestore;

    [FirestoreData]
    public class SampleModel
    {

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }
    }
}
