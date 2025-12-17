namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using System.Threading;
    using System.Threading.Tasks;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;

    public interface IBackendSynchronizationManager
    {
        Task InitializeAsync();
        Task EnqueueAsync(DtoFirestoreData dto);
        Task ForceProcessQueueAsync();
        Task StopAsync();
    }
}
