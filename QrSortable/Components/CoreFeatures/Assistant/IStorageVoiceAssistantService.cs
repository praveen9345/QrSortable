namespace QrSortable.Components.CoreFeatures.Assistant
{
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;

    public interface IStorageVoiceAssistantService
    {
        Task<List<StorageEntry>> SpeakMatchesAsync(double minConfidence = 0.2);
    }
}
