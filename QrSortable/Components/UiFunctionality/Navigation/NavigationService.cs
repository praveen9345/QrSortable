namespace QrSortable.Components.UiFunctionality.Navigation
{
    using QrSortable.Components.PlatformUtils.Wrappers;
    using CommunityToolkit.Maui.Core;

    /// <summary>
    ///     Implementation of the service providing navigation functionality.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly INavigationShellWrapper _shellWrapper;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NavigationService"/> class
        ///     with the specified <see cref="INavigationShellWrapper"/>.
        /// </summary>
        /// <param name="navigationShellWrapper">The navigation shell wrapper.</param>
        public NavigationService(INavigationShellWrapper navigationShellWrapper, IPopupService popupService)
        {
            _shellWrapper = navigationShellWrapper;
        }

        /// <summary>
        ///     Navigates to a page.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <returns> An awaitable task. </returns>
        public Task Navigate<T>() where T : Page
        {
            return _shellWrapper.GoToAsync( GetShellPath<T>(), false);
        }

        /// <summary>
        ///     Navigates to a page while passing a parameter.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <param name="parameter"> The parameter to pass. </param>
        /// <returns> An awaitable task. </returns>
        public async Task Navigate<T>(object parameter, bool fromRoot = false) where T : Page
        {
            var dictionary = new Dictionary<string, object>()
            {
                {"parameter", parameter}
            };

            if(fromRoot)
            {
                await _shellWrapper.PopToRootAsync(true);
            }
            await _shellWrapper.GoToAsync( GetShellPath<T>(), false, dictionary);
        }

        private string GetShellPath<T>()
        {
            var name = typeof(T).Name;
            var location = _shellWrapper.GetCurrentState().Location.ToString();

            return name;
        }

        /// <summary>
        ///     Closes the current view and navigates back to the previous view.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async Task Close()
        {
            await _shellWrapper.GoToAsync("..", false);
        }
    }

}