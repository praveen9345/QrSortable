namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides support for retrieval and storage of all the data.
    /// </summary>
    public interface IDatabaseManager : IBaseDatabaseManagerInterface
    {
        /// <summary>
        ///     An event handler which triggers as soon as a new database entry was added or deleted.
        /// </summary>
        event EventHandler<Type> EntryAddedOrDeleted;

        /// <summary>
        ///     Clears the database by deleting every DatabaseSet contained in it.
        /// </summary>
        Task ClearDatabaseAsync();

        /// <summary>
        ///     Gets all database entries of the given type and returns them as a list.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <returns>A list of entries of the specified type.</returns>
        Task<List<T>> GetListAsync<T>() where T : DatabaseEntry;
    }
}
