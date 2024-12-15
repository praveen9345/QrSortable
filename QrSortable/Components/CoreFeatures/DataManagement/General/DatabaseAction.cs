namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The class for executing Database actions in a queue.
    /// </summary>
    public class DatabaseAction<T> : IDatabaseAction
    {
        private Func<CancellationToken, Task<T>> _databaseOperationAsync;
        private TaskCompletionSource<T> _taskCompletionSource;

        /// <summary>
        ///     Adds an action to be executed in a queue.
        /// </summary>
        /// <param name="databaseOperationAsync">The operation to execute later.</param>
        /// <param name="tcs">A TaskCompletionSource indicating whether the action is finished.</param>
        public void AddAction(Func<CancellationToken, Task<T>> databaseOperationAsync, TaskCompletionSource<T> tcs)
        {
            _databaseOperationAsync = databaseOperationAsync;
            _taskCompletionSource= tcs;
        }

        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to stop the execution.</param>
        /// <returns>An awaitable task.</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _taskCompletionSource.SetResult(await _databaseOperationAsync(cancellationToken));
        }
    }
}
