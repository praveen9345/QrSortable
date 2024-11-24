namespace QrSortable.Components.CoreFeatures.AppStart
{
    /// <summary>
    /// Represents an application service interface with a method for starting the service asynchronously.
    /// </summary>
    public interface IAppService
    {
        /// <summary>
        /// Performs necessary actions to start the application service asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task OnStartAsync();
    }

}