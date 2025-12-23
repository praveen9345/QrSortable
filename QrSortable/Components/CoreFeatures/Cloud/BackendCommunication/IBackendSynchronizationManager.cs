namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using System.Threading.Tasks;

    public interface IBackendSynchronizationManager
    {
        Task<bool> SynchronizeStoredObjectsAsync();
    }
}
