namespace QrSortable.Components.UiFunctionality.Navigation
{
    /// <summary>
    ///     Interface of the service providing navigation functionality.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        ///     Navigates to a page.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <returns> An awaitable task. </returns>
        Task Navigate<T>() where T : Page;

        /// <summary>
        ///     Navigates to a page while passing a parameter.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <param name="parameter"> The parameter to pass. </param>
        /// <param name="fromRoot"> Optionally reset the view stack. </param>
        /// <returns> An awaitable task. </returns>
        Task Navigate<T>(object parameter, bool fromRoot) where T : Page;

        /// <summary>
        ///     Closes the current view and navigates back to the previous view.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task Close();

    }
}