namespace QrSortable.Components.CoreFeatures.Assistant
{
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    public interface IStorageFinderService
    {
        Task<List<StorageEntry>> FindGenericAsync(string query, CancellationToken ct = default);
    }
}
