namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The interface for executing Database actions in a queue.
    /// </summary>
    public interface IDatabaseAction
    {
        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to stop the execution.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}