namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using System.Threading.Tasks;

    /// <summary>
    ///     This manager handles the immediate backend synchronization using data objects composed of data collected within the general database.
    /// </summary>
    public interface IGeneralDatabaseSynchronizationManager
    {
        /// <summary>
        ///     Transfers all relevant data that was collected within the app to the backend.
        /// </summary>
        /// <returns>
        ///     True, if the last backup synchronization was updated successfully or if backend is not used. False, otherwise.
        /// </returns>
        Task<bool> SynchronizeAppDataAsync();
    }
}
