namespace QrSortable.Components.PlatformUtils
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The implementation of the interface <see cref="ITaskHelperService" />.
    /// </summary>
    public class TaskHelperService : ITaskHelperService
    {

        /// <summary>
        ///     Runs the given task until the given time has passed.
        /// </summary>
        /// <param name="function"> A function which encapsulates the task to be run </param>
        /// <param name="timeout"> The timespan until which the task should be run.</param>
        /// <returns>
        ///     An awaitable task which returns true if the task threw no exception and finished before the specified
        ///     timeout. False, otherwise.
        /// </returns>
        public async Task<bool> RunTaskUntilTimeoutAsync(Func<Task> function, TimeSpan timeout)
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            try
            {
                var task = function();
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    await completedTask;
                    timeoutCancellationTokenSource.Cancel();
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                timeoutCancellationTokenSource.Cancel();
                Console.WriteLine("Error: TaskHelperService: RunTaskUntilTimeoutAsync: " + exception);
                return false;
            }
        }

        /// <summary>
        ///     Delays until the timeout is reached.
        /// </summary>
        /// <param name="timeout"> The timespan signaling how long shall be delayed for. </param>
        /// <returns> An awaitable task which finishes once the timeout is reached. </returns>
        public async Task Delay(TimeSpan timeout)
        {
            await Task.Delay(timeout);
        }

        /// <summary>
        ///     Runs the passed function on a new thread.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <returns>A task proxy for the task returned by the function.</returns>
        public Task Run(Func<Task> function)
        {
            return Task.Run(function);
        }
    }
}