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
        /// <returns> An awaitable task. </returns>
        Task Navigate<T>(object parameter) where T : Page;

        /// <summary>
        ///     This method navigates to a specific page of type T asynchronously.
        /// </summary>
        /// <param name="parameter"> The parameter to pass.</param>
        /// <returns> A task representing the asynchronous navigation operation and returning a result of type TReturn. </returns>
        Task<TReturn> OpenDialogAndAwaitResultAsync<T, TParameter, TReturn>(TParameter parameter) where T : ContentPage;

        /// <summary>
        ///     This method navigates to a specific page of type T asynchronously.
        /// </summary>
        /// <returns> A task representing the asynchronous navigation operation and returning a result of type TReturn. </returns>
        Task<TReturn> OpenDialogAndAwaitResultAsync<T, TReturn>() where T : ContentPage;

        /// <summary>
        ///     Closes the current view and navigates back to the previous view.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task Close();

        /// <summary>
        ///     Closes the current dialog and navigates back in the application.
        /// </summary>
        /// <param name="result">The dialog result to pass back to the dialog caller.</param>
        void CloseDialog<TReturn>(TReturn result);

        /// <summary>
        ///     Changes the presentation to a new page.
        /// </summary>
        /// <param name="typeOfPriorViewModel"> The type of the prior view model.</param>
        /// <returns> A task representing the asynchronous operation. </returns>
        Task ChangePresentation(Type typeOfPriorViewModel);

        /// <summary>
        ///     Returns the current location as string.
        /// </summary>
        public string CurrentLocation { get; }
    }
}