
using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;

namespace QrSortable.Components.CoreFeatures.DataManagement.Backend
{

    /// <summary>
    ///     Implementation of the interface IBackendDatabaseManager enabling interactions with the database used for backend
    ///     synchronization.
    /// </summary>
    public class BackendDatabaseManager : BaseDatabaseManager, IBackendDatabaseManager
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="BackendDatabaseManager" />.
        /// </summary>
        public BackendDatabaseManager()
        {
        }

        /// <summary>
        ///     Clears the database by deleting every DatabaseSet contained in it.
        /// </summary>
        public async Task ClearDatabaseAsync()
        {
            ResetContext();

            var storage = await GetAllAsync<DtoStorageEntryModel>();
            await DeleteRangeWithOptionalSaveChangesAsync(storage, false);

            var orders = await GetAllAsync<DtoOrdersModel>();
            await DeleteRangeWithOptionalSaveChangesAsync(orders, false);

            var subscriptionEntries = await GetAllAsync<DtoSubscriptionEntityModel>();
            await DeleteRangeWithOptionalSaveChangesAsync(subscriptionEntries, false);

        }
    }
}
