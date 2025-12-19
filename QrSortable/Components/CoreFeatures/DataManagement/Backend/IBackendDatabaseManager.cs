namespace QrSortable.Components.CoreFeatures.DataManagement.Backend
{
    /// <summary>
    ///     Provides support for retrieval and storage of all the data contained in the backend database.
    /// </summary>
    public interface IBackendDatabaseManager : IBaseDatabaseManagerInterface
    {
        /// <summary>
        ///     Clears the database by deleting every DatabaseSet contained in it.
        /// </summary>
        Task ClearDatabaseAsync();
    }
}
