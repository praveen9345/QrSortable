namespace QrSortable.Components.CoreFeatures.DataManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;

    /// <summary>
    ///     Provides support for retrieval and storage of all the data.
    /// </summary>
    public interface IBaseDatabaseManagerInterface
    {
        /// <summary>
        ///     Initializes the database manager by passing a function to create the required database context,
        ///     which is used for database interactions.
        /// </summary>
        /// <param name="contextCreationFunction">A function that creates a new database context.</param>
        void Initialize(Func<DbContext> contextCreationFunction);

        /// <summary>
        ///     Gets all database entries of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <returns>A queryable of entries of the specified type.</returns>
        Task<IQueryable<T>> GetAllAsync<T>() where T : DatabaseEntry;

        /// <summary>
        ///     Adds the given type of entity into the database asynchronously. Returns the added entity if success or null in case of failure by
        ///     logging the error.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be added into the database.</param>
        /// <returns>The inserted entity in case of success or null in case of failure.</returns>
        Task<T> AddAsync<T>(T t) where T : DatabaseEntry;

        /// <summary>
        ///     Adds a list of new entries of the specified type to the database asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The entries to add to the database.</param>
        /// <returns>True, if the entries have been added successfully; otherwise false.</returns>
        Task<bool> AddRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry;

        /// <summary>
        ///     Updates the given type of entity in the database asynchronously. Returns the updated entity if success or null in case of
        ///     failure by logging the error.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be updated into the database.</param>
        /// <returns>The updated entity in case of success or null in case of failure.</returns>
        Task<T> UpdateAsync<T>(T t) where T : DatabaseEntry;

        /// <summary>
        ///     Updates the given entries in the database asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The database entries to update.</param>
        /// <returns>True, if the update was successful; otherwise false.</returns>
        Task<bool> UpdateRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry;

        /// <summary>
        ///     Deletes the given entry from the database asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entry">The database entry to delete.</param>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        Task<bool> DeleteAsync<T>(T entry) where T : DatabaseEntry;

        /// <summary>
        ///     Deletes the given entries from the database asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entries">The database entries to delete.</param>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        Task<bool> DeleteRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry;

        /// <summary>
        ///     Begins a database transaction that can be reverted by calling the Rollback method.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        ///     Commits the current database transaction.
        ///     Loads a new context in case the commit failed.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        ///     Reverts the changes made during the current database transaction.
        /// </summary>
        void Rollback();
    }
}