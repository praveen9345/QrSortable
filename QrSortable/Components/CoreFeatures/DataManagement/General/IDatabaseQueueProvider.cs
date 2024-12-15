namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Interface of the Database queue provider.
    /// </summary>
    public interface IDatabaseQueueProvider
    {
        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The <see cref="DatabaseContext"/> is disposed afterwards.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/>.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The bool returned by the databaseOperationAsync.</returns>
        Task<bool> EnqueueDatabaseCallAsync(Func<CancellationToken, Task<bool>> databaseOperationAsync,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The function can return a result value 
        ///     that is returned to the caller after the <see cref="DatabaseContext"/> was disposed.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/> and returns a value.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The value returned by <paramref name="databaseOperationAsync"/>.</returns>
        Task<DatabaseEntry> EnqueueDatabaseUpdateAsync(
            Func<CancellationToken, Task<DatabaseEntry>> databaseOperationAsync,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The function can return a result value 
        ///     that is returned to the caller after the <see cref="DatabaseContext"/> was disposed.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/> and returns a value.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The value returned by <paramref name="databaseOperationAsync"/>.</returns>
        Task<IQueryable<DatabaseEntry>> EnqueueDatabaseRetrievalAsync(
            Func<CancellationToken, Task<IQueryable<DatabaseEntry>>> databaseOperationAsync,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a read-only database operation asynchronously. It creates a new <see cref="DatabaseContext"/> instance
        /// and passes it to the provided function for interacting with the database. The function can return a list of items 
        /// of type <typeparamref name="T"/> as a result. The method enqueues the operation, processes the queue, and returns 
        /// the result of the operation when completed.
        /// </summary>
        /// <typeparam name="T">The type of data that the database operation returns.</typeparam>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/> and
        ///  returns a <see cref="Task{T}"/> representing the result of the operation.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>
        ///     A <see cref="Task{T}"/> representing the asynchronous operation, containing 
        ///     the value returned by <paramref name="databaseOperationAsync"/>.
        /// </returns>
        Task<List<T>> EnqueueDatabaseReadonlyRetrievalAsync<T>(
            Func<CancellationToken, Task<List<T>>> databaseOperationAsync,
            CancellationToken cancellationToken = default);
    }
}
