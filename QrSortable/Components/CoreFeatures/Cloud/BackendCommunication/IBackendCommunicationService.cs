namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    public interface IBackendCommunicationService
    {

        Task InsertAsync<T>(T data) where T : DtoFirestoreData;

        Task<T?> GetAsync<T>(string id) where T : DtoFirestoreData, new();

        Task UpdateAsync<T>(T data) where T : DtoFirestoreData;

        Task DeleteAsync<T>(string id, string collectionName);
    }
}














