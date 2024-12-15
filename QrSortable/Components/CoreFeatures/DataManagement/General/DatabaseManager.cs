namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     Implementation of the database manager interface enabling interactions with the database.
    /// </summary>
    public class DatabaseManager : BaseDatabaseManager, IDatabaseManager
    {
        private readonly IDatabaseQueueProvider _databaseQueueProvider;
        private readonly List<Type> _typesForEventToBeTriggered;

        /// <summary>
        ///     An event handler which triggers as soon as a new database entry was added or deleted.
        /// </summary>
        public event EventHandler<Type> EntryAddedOrDeleted;

        /// <summary>
        ///     Initializes an instance of the <see cref="DatabaseManager" />.
        /// </summary>
        /// <param name="databaseQueueProvider">The provider for the database queue functionality.</param>
        public DatabaseManager(IDatabaseQueueProvider databaseQueueProvider)
        {
            _databaseQueueProvider = databaseQueueProvider;
            _typesForEventToBeTriggered = new List<Type>();
        }

        /// <summary>
        ///     Gets all database entries of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <returns>A queryable of entries of the specified type.</returns>
        public new async Task<IQueryable<T>> GetAllAsync<T>() where T : DatabaseEntry
        {
            var entities = await _databaseQueueProvider.EnqueueDatabaseRetrievalAsync(
                async token => await base.GetAllAsync<T>());

            return entities as IQueryable<T>;
        }

        /// <summary>
        ///     Gets all database entries of the given type and returns them as a list.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <returns>A list of entries of the specified type.</returns>
        public async Task<List<T>> GetListAsync<T>() where T : DatabaseEntry
        {
            var entities = await _databaseQueueProvider.EnqueueDatabaseReadonlyRetrievalAsync(
                async token => (await base.GetAllAsync<T>()).ToList());

            return entities;
        }

        /// <summary>
        ///     Calls the base AddAsync method and invokes the event <see cref="EntryAddedOrDeleted" /> in case of success.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be added into the database.</param>
        /// <returns>The inserted entity in case of success or null in case of failure.</returns>
        public new async Task<T> AddAsync<T>(T t) where T : DatabaseEntry
        {
            var entity = await _databaseQueueProvider
                .EnqueueDatabaseUpdateAsync(async token => await base.AddAsync(t));

            if (entity == null) return null;

            HandleEventForType<T>();

            return entity as T;
        }

        /// <summary>
        ///     Calls the base AddRangeAsync method and invokes the event <see cref="EntryAddedOrDeleted" /> in case of success.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The entries to add to the database.</param>
        /// <returns>True, if the entries have been added successfully; otherwise false.</returns>
        public new async Task<bool> AddRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            var success = await _databaseQueueProvider
                .EnqueueDatabaseCallAsync(async token => await base.AddRangeAsync<T>(entries));

            if (!success) return false;

            HandleEventForType<T>();

            return true;
        }

        /// <summary>
        ///     Updates the given type of entity into the database. Returns the updated entity if success or null in case of
        ///     failure by logging the error.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be updated into the database.</param>
        /// <returns>The updated entity in case of success or null in case of failure.</returns>
        public new async Task<T> UpdateAsync<T>(T t) where T : DatabaseEntry
        {
            var entity = await _databaseQueueProvider
                .EnqueueDatabaseUpdateAsync(async token => await base.UpdateAsync(t));

            if (entity == null) return null;

            HandleEventForType<T>();

            return entity as T;
        }

        /// <summary>
        ///     Updates the given entries in the database.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The database entries to update.</param>
        /// <returns>True, if the update was successful; otherwise false.</returns>
        public new async Task<bool> UpdateRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            var success = await _databaseQueueProvider
                .EnqueueDatabaseCallAsync(
                    async token => await base.UpdateRangeAsync(entries));

            if (!success) return false;

            HandleEventForType<T>();

            return true;
        }

        /// <summary>
        ///     Calls the base DeleteAsync method and invokes the event <see cref="EntryAddedOrDeleted" /> in case of success.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entry">The database entry to delete.</param>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        public new async Task<bool> DeleteAsync<T>(T entry) where T : DatabaseEntry
        {
            var success = await _databaseQueueProvider
                .EnqueueDatabaseCallAsync(async token => await base.DeleteAsync(entry));

            if (!success) return false;

            HandleEventForType<T>();

            return true;
        }

        /// <summary>
        ///     Calls the base DeleteRangeAsync method and invokes the event <see cref="EntryAddedOrDeleted" /> in case of success.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entries">The database entries to delete.</param>
        /// <returns>True, if the update was successful; otherwise false.</returns>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        public new async Task<bool> DeleteRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            var success = await _databaseQueueProvider
                .EnqueueDatabaseCallAsync(async token => await base.DeleteRangeAsync(entries));

            if (!success) return false;

            HandleEventForType<T>();

            return true;
        }

        /// <summary>
        ///     Commits the current database transaction. Furthermore, if an add or a delete operation was done during the
        ///     transaction the EntryAddedOrDeletedEvent is fired.
        /// </summary>
        public new void CommitTransaction()
        {
            base.CommitTransaction();
            foreach (var type in _typesForEventToBeTriggered)
            {
                EntryAddedOrDeleted?.Invoke(this, type);
            }
            _typesForEventToBeTriggered.Clear();
        }

        /// <summary>
        ///     Reverts the changes made during the current database transaction.
        /// </summary>
        public new void Rollback()
        {
            base.Rollback();
            _typesForEventToBeTriggered.Clear();
        }

        /// <summary>
        ///     Clears the database by deleting every DatabaseSet contained in it.
        /// </summary>
        public async Task ClearDatabaseAsync()
        {
            ResetContext();
            var generalInformation = await GetAllAsync<GeneralInformation>();
            await DeleteRangeWithOptionalSaveChangesAsync(generalInformation, false);
            
            var UserInfos = await GetAllAsync<UserInfos>();
            await DeleteRangeWithOptionalSaveChangesAsync(UserInfos, false);

            var StorySaved = await GetAllAsync<StorySaved>();
            await DeleteRangeWithOptionalSaveChangesAsync(StorySaved, false);
        }

        private void HandleEventForType<T>()
        {
            if (!IsTransactionInProcess)
            {
                EntryAddedOrDeleted?.Invoke(this, typeof(T));
            }
            else
            {
                _typesForEventToBeTriggered.Add(typeof(T));
            }
        }
    }
}
