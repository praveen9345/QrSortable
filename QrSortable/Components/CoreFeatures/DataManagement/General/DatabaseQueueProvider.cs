namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Implementation of the Database queue provider.
    /// </summary>
    public class DatabaseQueueProvider : IDatabaseQueueProvider
    {
        private readonly ConcurrentQueue<IDatabaseAction> _concurrentQueue;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        ///     Constructor for the Databasecontextprovider. Sets up the priority queue.
        /// </summary>
        public DatabaseQueueProvider()
        {
            _concurrentQueue = new ConcurrentQueue<IDatabaseAction>();
        }

        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The <see cref="DatabaseContext"/> is disposed afterwards.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/>.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The bool returned by the databaseOperationAsync.</returns>
        public async Task<bool> EnqueueDatabaseCallAsync(Func<CancellationToken, Task<bool>> databaseOperationAsync, 
            CancellationToken cancellationToken = default)
        {
            var dbAction = new DatabaseAction<bool>();

            var tcs = new TaskCompletionSource<bool>();
            dbAction.AddAction(databaseOperationAsync, tcs);

            _concurrentQueue.Enqueue(dbAction);

            await RunQueue();
            return await tcs.Task;
        }

        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The function can return a result value 
        ///     that is returned to the caller after the <see cref="DatabaseContext"/> was disposed.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/> and returns a value.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The value returned by <paramref name="databaseOperationAsync"/>.</returns>
        public async Task<DatabaseEntry> EnqueueDatabaseUpdateAsync(Func<CancellationToken, Task<DatabaseEntry>> databaseOperationAsync, 
            CancellationToken cancellationToken = default)
        {
            var dbAction = new DatabaseAction<DatabaseEntry>();

            var tcs = new TaskCompletionSource<DatabaseEntry>();
            dbAction.AddAction(databaseOperationAsync, tcs);

            _concurrentQueue.Enqueue(dbAction);

            await RunQueue();

            return await tcs.Task;
        }

        /// <summary>
        ///     Creates a <see cref="DatabaseContext"/> instance and 
        ///     provides it to a given function to allow the caller to interact with the database context.
        ///     The function can return a result value 
        ///     that is returned to the caller after the <see cref="DatabaseContext"/> was disposed.
        /// </summary>
        /// <param name="databaseOperationAsync">A function that can interact with the created <see cref="DatabaseContext"/> and returns a value.</param>
        /// <param name="cancellationToken">Token to cancel execution early.</param>
        /// <returns>The value returned by <paramref name="databaseOperationAsync"/>.</returns>
        public async Task<IQueryable<DatabaseEntry>> EnqueueDatabaseRetrievalAsync(
            Func<CancellationToken, Task<IQueryable<DatabaseEntry>>> databaseOperationAsync,
            CancellationToken cancellationToken = default)
        {
            var dbAction = new DatabaseAction<IQueryable<DatabaseEntry>>();

            var tcs = new TaskCompletionSource<IQueryable<DatabaseEntry>>();
            dbAction.AddAction(databaseOperationAsync, tcs);

            _concurrentQueue.Enqueue(dbAction);

            await RunQueue();

            return await tcs.Task;
        }

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
        public async Task<List<T>> EnqueueDatabaseReadonlyRetrievalAsync<T>(
            Func<CancellationToken, Task<List<T>>> databaseOperationAsync,
            CancellationToken cancellationToken = default)
        {
            var dbAction = new DatabaseAction<List<T>>();

            var tcs = new TaskCompletionSource<List<T>>();
            dbAction.AddAction(databaseOperationAsync, tcs);

            _concurrentQueue.Enqueue(dbAction);

            await RunQueue();

            return await tcs.Task;
        }

        private async Task RunQueue()
        {
            await _semaphore.WaitAsync();

            try
            {

                while (_concurrentQueue.Any())
                {
                    _concurrentQueue.TryDequeue(out var action);
                    if (action == null) throw new Exception("Dequeue Error");


                    await action.ExecuteAsync(new CancellationToken());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("QrSortable.DatabaseQueueProvider:" + e);
            }
            finally
            {
                _semaphore.Release();
            }

        }
    }
}