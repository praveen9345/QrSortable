namespace QrSortable.Components.UiFunctionality.Navigation
{
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Navigation.Views;

    /// <summary>
    ///     Implementation of the service providing navigation functionality.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly INavigationShellWrapper _shellWrapper;

        private const string RootNavigationPath = "//" + nameof(MainPage) + "/" + nameof(RootView);
        private const string BackNavigationPath = "..";

        /// <summary>
        ///     Initializes a new instance of the <see cref="NavigationService"/> class
        ///     with the specified <see cref="INavigationShellWrapper"/>.
        /// </summary>
        /// <param name="navigationShellWrapper">The navigation shell wrapper.</param>
        public NavigationService(INavigationShellWrapper navigationShellWrapper)
        {
            _shellWrapper = navigationShellWrapper;
        }

        /// <summary>
        ///     Maintain dialog completion sources to keep track of dialog results.
        ///     A list is used to enable correct handling of dialog results, even if one dialog is shown on top of another dialog.
        /// </summary>
        private static IList<TaskCompletionSource<object>> DialogCloseCompletionSource { get; set; } =
            new List<TaskCompletionSource<object>>();

        /// <summary>
        ///     Navigates to a page.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <returns> An awaitable task. </returns>
        public async Task Navigate<T>() where T : Page
        {
            try
            {
                await _semaphore.WaitAsync();

                await _shellWrapper.GoToAsync(GetShellPath<T>(), false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NavigationService : Navigate<T>(): " +ex.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        ///     Navigates to a page while passing a parameter.
        /// </summary>
        /// <typeparam name="T"> The class of the page. </typeparam>
        /// <param name="parameter"> The parameter to pass. </param>
        /// <returns> An awaitable task. </returns>
        public async Task Navigate<T>(object parameter) where T : Page
        {
            try
            {
                await _semaphore.WaitAsync();

                var dictionary = new Dictionary<string, object>()
                {
                    {"parameter", parameter}
                };

                await _shellWrapper.GoToAsync(GetShellPath<T>(), false, dictionary);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NavigationService : Navigate<T>(object parameter): " + ex.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private string GetShellPath<T>()
        {
            var name = typeof(T).Name;
            if (name == nameof(RootView))
            {
                return RootNavigationPath;
            }

            var location = _shellWrapper.GetCurrentState().Location.ToString();
            if (location.Contains(name))
            {
                return location.Substring(0, location.IndexOf(name, StringComparison.Ordinal) + name.Length);
            }

            return name;
        }

        /// <summary>
        ///     This method navigates to a specific dialog of type T and awaits its result.
        /// </summary>
        /// <remarks>
        ///     Please make sure to call the corresponding <see cref="INavigationService.CloseDialog{TReturn}"/>
        ///     within the opened dialog, to ensure a correct closure of dialogs.
        /// </remarks>
        /// <param name="parameter"> The parameter to pass.</param>
        /// <returns> A task representing the asynchronous navigation operation and returning a result. </returns>
        public async Task<TReturn> OpenDialogAndAwaitResultAsync<T, TParameter, TReturn>(TParameter parameter) where T : ContentPage
            where TReturn : notnull
        {
            try
            {
                await _semaphore.WaitAsync();

                var dictionary = new Dictionary<string, object>()
                {
                    {"parameter", parameter},
                };

                var taskCompletionSource = new TaskCompletionSource<object>();
                DialogCloseCompletionSource.Add(taskCompletionSource);
                await _shellWrapper.GoToAsync(typeof(T).Name, false, dictionary);

                var value = (TReturn)await taskCompletionSource.Task;

                await CloseDialog();

                return value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        ///     This method navigates to a specific page of type T asynchronously.
        /// </summary>
        /// <returns> A task representing the asynchronous navigation operation and returning a result of type TReturn. </returns>
        public async Task<TReturn> OpenDialogAndAwaitResultAsync<T, TReturn>() where T : ContentPage
            where TReturn : notnull
        {
            try
            {
                await _semaphore.WaitAsync();

                var taskCompletionSource = new TaskCompletionSource<object>();
                DialogCloseCompletionSource.Add(taskCompletionSource);
                await _shellWrapper.GoToAsync(typeof(T).Name, false);

                var value = (TReturn)await taskCompletionSource.Task;

                await CloseDialog();

                return value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task CloseDialog()
        {
            try
            {
                var path = _shellWrapper.GetCurrentState().Location.ToString();
                if (path != RootNavigationPath)
                    await _shellWrapper.GoToAsync(BackNavigationPath, false);
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine("NavigationService:CloseDialog: " + ioex.ToString());
            }
        }

        /// <summary>
        ///     Closes the current view and navigates back to the previous view.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async Task Close()
        {
            try
            {
                await _semaphore.WaitAsync();

                var path = _shellWrapper.GetCurrentState().Location.ToString();
                if (path != RootNavigationPath)
                {
                    await _shellWrapper.GoToAsync(BackNavigationPath, false);
                }      
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine("NavigationService : Close(): " + ioex.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        ///     Closes the current dialog and navigates back in the application.
        /// </summary>
        /// <param name="result">The dialog result to pass back to the dialog caller.</param>
        public void CloseDialog<TReturn>(TReturn result)
        {
            var taskCompletionSource = DialogCloseCompletionSource.LastOrDefault();
            if (taskCompletionSource != null && taskCompletionSource.TrySetResult(result))
            {
                DialogCloseCompletionSource.Remove(taskCompletionSource);
            }
            else
            {
                Console.WriteLine("NavigationService : CloseDialog(): Failed to set dialog result.");
            }
        }

        /// <summary>
        ///     Changes the presentation to a new page.
        /// </summary>
        /// <param name="typeOfPriorViewModel"> The type of the prior view model.</param>
        /// <returns> A task representing the asynchronous operation. </returns>
        public async Task ChangePresentation(Type typeOfPriorViewModel)
        {
            await _semaphore.WaitAsync();

            var previousViewName = typeOfPriorViewModel.Name.Replace("ViewModel", "View");

            if (CurrentLocation.Contains(previousViewName))
            {
                while (CurrentLocation.Split('/').Last() != previousViewName)
                    await Shell.Current.Navigation.PopAsync(false);
            }
            else
            {
                await _shellWrapper.GoToAsync(previousViewName, false);
            }
            _semaphore.Release();
        }

        /// <summary>
        ///     Returns the current location as string.
        /// </summary>
        public string CurrentLocation => _shellWrapper.GetCurrentState().Location.OriginalString;
    }
}