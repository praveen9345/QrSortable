namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    public interface IBackendCommunicationService
    {

        Task InsertAsync<T>(T data) where T : FirestoreData;

        Task<T?> GetAsync<T>(string id) where T : FirestoreData, new();

        Task UpdateAsync<T>(T data) where T : FirestoreData;

        Task DeleteAsync<T>(string id, string collectionName);
    }
}














