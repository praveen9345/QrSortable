namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Google.Cloud.Firestore;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BackendCommunicationService :IBackendCommunicationService
    {
        private FirestoreDb Db { get; set; }
        private readonly IAesHelper _aesHelper;

        public BackendCommunicationService(IAesHelper aesHelper)
        {
            Db = FirestoreDb.Create(Configuration.Constants.PROJECT_ID);
            _aesHelper = aesHelper;
        }

        public async Task InsertAsync<T>(T data) where T : FirestoreData
        {
            Validate(data);

            // Serialize to JSON and encrypt
            var encryptedJson = EncryptData(data);

            var doc = new Dictionary<string, object>
            {
                { "EncryptedData", encryptedJson }
            };

            await Db.Collection(data.CollectionName)
                    .Document(data.MultiuserId)
                    .CreateAsync(doc);
        }

        public async Task<T?> GetAsync<T>(string id) where T : FirestoreData, new()
        {
            var temp = new T();
            var docRef = Db.Collection(temp.CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return null;

            var encryptedJson = snapshot.GetValue<string>("EncryptedData");
            return DecryptData<T>(encryptedJson);
        }

        public async Task UpdateAsync<T>(T data) where T : FirestoreData
        {
            Validate(data);

            var encryptedJson = EncryptData(data);

            var doc = new Dictionary<string, object>
            {
                { "EncryptedData", encryptedJson }
            };

            await Db.Collection(data.CollectionName)
                    .Document(data.MultiuserId)
                    .SetAsync(doc, SetOptions.Overwrite);
        }

        public async Task DeleteAsync<T>(string id, string collectionName)
        {
            await Db.Collection(collectionName)
                    .Document(id)
                    .DeleteAsync();
        }

        #region Helpers

        private string EncryptData<T>(T data)
        {
            // Convert object to JSON (handles byte[], Unicode, complex types)
            var json = JsonSerializer.Serialize(data);
            return _aesHelper.Encrypt(json);
        }

        private T DecryptData<T>(string encryptedJson)
        {
            var json = _aesHelper.Decrypt(encryptedJson);
            return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Failed to deserialize decrypted data.");
        }

        private static void Validate(FirestoreData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(data.MultiuserId))
                throw new ArgumentException("Document Id is required.");

            if (string.IsNullOrWhiteSpace(data.CollectionName))
                throw new ArgumentException("CollectionName is required.");
        }

        #endregion
    }
}
