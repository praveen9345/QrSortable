namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{

    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;

    public class BackendCommunicationService :IBackendCommunicationService
    {
        private FirestoreDb Db { get; set; }

        public BackendCommunicationService()
        {
            Db = FirestoreDb.Create(Configuration.Constants.PROJECT_ID);
        }

        public async Task InsertSampleModel(SampleModel sample)
        { 
             await Db.Collection("SampleModels").AddAsync(sample);
        }

       
    }
}
