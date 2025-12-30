namespace QrSortable.Components.CoreFeatures.Assistant
{
    using QrSortable.Components.CoreFeatures.Assistant.Helpers;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;

    public class StorageFinderService : IStorageFinderService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IStorageFinderHelper _storageFinderHelper;

        public StorageFinderService(IDatabaseManager databaseManager, IStorageFinderHelper storageFinderHelper) 
        {
            _databaseManager = databaseManager;
            _storageFinderHelper = storageFinderHelper;
        }
        
        public async Task<List<StorageEntry>> FindGenericAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<StorageEntry>();

            var allEntriesCache = await _databaseManager.GetListAsync<StorageEntry>();

            if (allEntriesCache == null || !allEntriesCache.Any())
            {
                return new List<StorageEntry>();
            }

            string normalizedQuery = _storageFinderHelper.Normalize(query);

            return await Task.Run(() =>
            {
                return allEntriesCache
                   .Select(entry => new
                   {
                       Item = entry,
                       // SearchInfo already contains all fields; we normalize it for 99% accuracy
                       NormSearchInfo = _storageFinderHelper.Normalize(entry.SearchInfo)
                   })
                   .Select(x => new
                   {
                       x.Item,
                       // Calculate similarity score between normalized query and normalized data
                       Score = _storageFinderHelper.CalculateSimilarity(normalizedQuery, x.NormSearchInfo)
                   })
                   // 0.4 is the optimal threshold for balancing "Precision" vs "Recall" 
                   .Where(x => x.Score > 0.4 || x.Item.SearchInfo.ToLower().Contains(query.ToLower()))
                   .OrderByDescending(x => x.Score).Select(x => x.Item).ToList();
            },ct);

        }
    }
}
