namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using Navigation.ViewModels;
    using System;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using PlatformUtils;
    using QrSortable.Core.Components.UiFunctionality.Notification.Models;

    /// <summary>
    ///     The view model of the dialog depicting an activity indicator.
    ///     Handles the display of an activity indicator as long as a given function is awaited.
    ///     Either returns the boolean result value on completion or navigates using the given navigation function.
    /// </summary>
    public partial class DialogActivityIndicatorViewModel : BaseViewModel<DialogActivityIndicatorData, bool>
    {
        private readonly ITaskHelperService _taskHelperService;
        private Func<Task<bool>> _funcToAwait;
        private Func<Task> _navigationFunc;

        /// <summary>
        ///     Initializes an instance of the <see cref="DialogActivityIndicatorViewModel" /> class.
        /// </summary>
        /// <param name="taskHelperService">The service for running async tasks.</param>
        public DialogActivityIndicatorViewModel(IServiceProvider serviceProvider, ITaskHelperService taskHelperService)
        {
            _taskHelperService = taskHelperService;
            IsBackNavigationEnabled = false;
        }

        /// <summary>
        ///     Awaits the completion of the task from the given function after the application initializes. Once the task is completed, the
        ///     view model is either closed or the navigation function is executed.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await _taskHelperService.Run(async () =>
            {
                var functionCompletedWithSuccess = await _funcToAwait();

                NavigationService.CloseDialog(functionCompletedWithSuccess);

                if (_navigationFunc != null && functionCompletedWithSuccess)
                {
                    await _navigationFunc();
                }
            });
        }

        /// <summary>
        ///     Prepares the viewmodel and sets both the function containing the task to complete and the navigation function.
        /// </summary>
        /// <param name="parameter">The DialogActivityIndicatorData passed over to this viewmodel.</param>
        public override void Prepare(DialogActivityIndicatorData parameter)
        {
            Text = parameter.Text;
            _funcToAwait = parameter.AwaitableFunction;
            _navigationFunc = parameter.NavigationFunction;
        }

        /// <summary>
        ///     Gets or sets the text to display below the activity indicator.
        /// </summary>
        [ObservableProperty]
        private string _text;
    }
}