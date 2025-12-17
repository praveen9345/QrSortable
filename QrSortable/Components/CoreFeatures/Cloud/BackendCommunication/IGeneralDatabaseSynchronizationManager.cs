namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGeneralDatabaseSynchronizationManager
    {
        Task<bool> UploadAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UploadEntitiesAsync<T>(Func<T, DtoFirestoreData> mapper)
            where T : DataManagement.Models.DatabaseEntry;
    }
}
