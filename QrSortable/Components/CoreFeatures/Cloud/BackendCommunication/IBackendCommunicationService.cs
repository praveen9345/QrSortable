namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;

    public interface IBackendCommunicationService
    {

        Task<bool> InsertAsync<T>(T data, bool isFrombackendSync = false);

        Task<bool> UpdateAsync<T>(T data, bool isFrombackendSync = false);

        Task<T?> GetAsync<T>(string id) where T : FirestoreData, new();

        Task<bool> DeleteAsync<T>(T data, bool isFrombackendSync = false);

        Task<List<T>> GetAllAsync<T>() where T : FirestoreData, new();

        Task<List<T>> GetByMultiuserIdAsync<T>(string multiuserId) where T : FirestoreData, new();

    }
}














