namespace QrSortable.Components.PlatformUtils
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     The service interface providing helper methods concerning tasks.
    /// </summary>
    public interface ITaskHelperService
    {
        /// <summary>
        ///     Runs the given task until the given time has passed.
        /// </summary>
        /// <param name="function"> A function which encapsulates the task to be run </param>
        /// <param name="timeout"> The timespan until which the task should be run.</param>
        /// <returns> An awaitable task which returns true if the task threw no exception and finished before the specified timeout. False, otherwise. </returns>
        Task<bool> RunTaskUntilTimeoutAsync(Func<Task> function, TimeSpan timeout);

        /// <summary>
        ///     Delays until the timeout is reached.
        /// </summary>
        /// <param name="timeout"> The timespan signaling how long shall be delayed for. </param>
        /// <returns> An awaitable task which finishes once the timeout is reached. </returns>
        Task Delay(TimeSpan timeout);

        /// <summary>
        ///     Runs the passed function on a new thread.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <returns>A task proxy for the task returned by the function.</returns>
        Task Run(Func<Task> function);
    }
}
