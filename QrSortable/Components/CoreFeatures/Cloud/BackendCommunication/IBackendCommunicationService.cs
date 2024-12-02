namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication.Models;
    public interface IBackendCommunicationService
    {
        Task InsertSampleModel(SampleModel sample);

    }
}
