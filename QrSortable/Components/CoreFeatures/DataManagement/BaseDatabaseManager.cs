namespace QrSortable.Components.CoreFeatures.DataManagement
{
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     A base data manager class enabling interactions with the databases.
    /// </summary>
    public abstract class BaseDatabaseManager
    {
        private Func<DbContext> _createNewDatabaseContextFunc;
        private DbContext _databaseContext;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        ///     A value indicating whether a transaction is currently processed.
        /// </summary>
        protected bool IsTransactionInProcess;

        private async Task<bool> SaveChangesAndHandleLoggingAsync(DbContext context)
        {
            try
            {
                await _semaphore.WaitAsync();
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
                Console.WriteLine("Error: BaseDatabaseManager: "+  dbUpdateException);
                return false;
            }
            finally
            {
                _semaphore.Release();
            }

            return true;
        }

        /// <summary>
        ///     Initializes the database manager by passing a function to create the required database context,
        ///     which is used for database interactions.
        /// </summary>
        /// <param name="contextCreationFunction">A function that creates a new database context.</param>
        public void Initialize(Func<DbContext> contextCreationFunction)
        {
            _createNewDatabaseContextFunc = contextCreationFunction;
            _databaseContext = contextCreationFunction.Invoke();
            _databaseContext.Database.Migrate();
        }

        /// <summary>
        ///     Gets all database entries of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <returns>A list of entries of the specified type.</returns>
        public async Task<IQueryable<T>> GetAllAsync<T>() where T : DatabaseEntry
        {
            try
            {
                await _semaphore.WaitAsync();
                return _databaseContext.Set<T>().AsQueryable();

            }
            catch (DbUpdateException dbUpdateException)
            {
                Console.WriteLine("Error: BaseDatabaseManager: "+  dbUpdateException);
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        ///     Adds the given type of entity into the database asynchronously. Returns the added entity if success or null in case of failure by
        ///     logging the error.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be added into the database.</param>
        /// <returns>The inserted entity in case of success or null in case of failure.</returns>
        public async Task<T> AddAsync<T>(T t) where T : DatabaseEntry
        {
            T entity = _databaseContext.Add(t).Entity;
            return await SaveChangesAndHandleLoggingAsync(_databaseContext) ? entity : null;
        }

        /// <summary>
        ///     Adds a list of new entries of the specified type to the database.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The entries to add to the database.</param>
        /// <returns>True, if the entries have been added successfully; otherwise false.</returns>
        public async Task<bool> AddRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            _databaseContext.AddRange(entries);
            return await SaveChangesAndHandleLoggingAsync(_databaseContext);
        }

        /// <summary>
        ///     Updates the given type of entity into the database. Returns the updated entity if success or null in case of
        ///     failure by logging the error.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="t">The object of the entity to be updated into the database.</param>
        /// <returns>The updated entity in case of success or null in case of failure.</returns>
        public async Task<T> UpdateAsync<T>(T t) where T : DatabaseEntry
        {
            T entity = _databaseContext.Update(t).Entity;
            return await SaveChangesAndHandleLoggingAsync(_databaseContext) ? entity : null;
        }

        /// <summary>
        ///     Updates the given entries in the database.
        /// </summary>
        /// <typeparam name="T">The type of the database entries.</typeparam>
        /// <param name="entries">The database entries to update.</param>
        /// <returns>True, if the update was successful; otherwise false.</returns>
        public async Task<bool> UpdateRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            _databaseContext.UpdateRange(entries);
            return await SaveChangesAndHandleLoggingAsync(_databaseContext);
        }

        /// <summary>
        ///     Deletes the given entry from the database.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entry">The database entry to delete.</param>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        public async Task<bool> DeleteAsync<T>(T entry) where T : DatabaseEntry
        {
            _databaseContext.Remove(entry);
            return await SaveChangesAndHandleLoggingAsync(_databaseContext);
        }

        /// <summary>
        ///     Deletes the given entries from the database.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entries">The database entries to delete.</param>
        /// <returns>True, if the update was successful; otherwise false.</returns>
        /// <returns>True, if the deletion was successful; otherwise false.</returns>
        public async Task<bool> DeleteRangeAsync<T>(IEnumerable<T> entries) where T : DatabaseEntry
        {
            _databaseContext.RemoveRange(entries);
            return await SaveChangesAndHandleLoggingAsync(_databaseContext);
        }

        /// <summary>
        ///     Begins a database transaction that can be reverted by calling the Rollback method.
        ///     In case of database transactions, all operations are done without saving the context.
        ///     The context is then finally saved on commit.
        /// </summary>
        public void BeginTransaction()
        {
            IsTransactionInProcess = true;
            _databaseContext.Database.BeginTransaction();
        }

        /// <summary>
        ///     Commits the current database transaction.
        /// </summary>
        public void CommitTransaction()
        {
            _databaseContext.Database.CommitTransaction();
            IsTransactionInProcess = false;
        }

        /// <summary>
        ///     Reverts the changes made during the current database transaction.
        /// </summary>
        public void Rollback()
        {
            _databaseContext.Database.RollbackTransaction();

            // re-create context after rollback to ensure that the context reflects the latest database state
            _databaseContext = _createNewDatabaseContextFunc.Invoke();
            IsTransactionInProcess = false;
        }

        /// <summary>
        ///     Deletes the given entries from the database but doesn't save the changes if not needed.
        /// </summary>
        /// <param name="entries"> The entries to be deleted. </param>
        /// <param name="shallChangesBeSaved"> Whether the changes shall be saved. </param>
        /// <returns>An awaitable Task.</returns>
        protected async Task DeleteRangeWithOptionalSaveChangesAsync<T>(IEnumerable<T> entries, bool shallChangesBeSaved) where T : DatabaseEntry
        {
            _databaseContext.RemoveRange(entries);
            if (shallChangesBeSaved)
            {
                await SaveChangesAndHandleLoggingAsync(_databaseContext);
            }
        }

        /// <summary>
        ///     Resets the database context used.
        /// </summary>
        protected void ResetContext()
        {
            _databaseContext = _createNewDatabaseContextFunc.Invoke();
        }
    }
}