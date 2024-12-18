namespace QrSortable.Components.UiFunctionality.Notification.ViewModels
{
    using Models;
    using Navigation.ViewModels;
    using System;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using PlatformUtils;

    /// <summary>
    ///     The view model of the dialog depicting an activity indicator.
    ///     Handles the display of an activity indicator as long as a given function is awaited.
    ///     On completion, this viewmodel closes itself and returns the result of the awaited function.
    /// </summary>
    public partial class DialogGenericActivityIndicatorViewModel : BaseViewModel<DialogGenericActivityIndicatorData, object>
    {
        private readonly ITaskHelperService _taskHelperService;
        private Func<Task<object>> _funcToAwait;

        /// <summary>
        ///     Initializes an instance of the <see cref="DialogGenericActivityIndicatorViewModel" /> class.
        /// </summary>
        /// <param name="taskHelperService">The service for running async tasks.</param>
        public DialogGenericActivityIndicatorViewModel(IServiceProvider serviceProvider, ITaskHelperService taskHelperService)
        {
            _taskHelperService = taskHelperService;
            IsBackNavigationEnabled = false;
        }

        /// <summary>
        ///     Awaits the completion of the task from the given function after the application initializes. Once the task is completed, the
        ///     view model is closed and the result of the awaited function is returned.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await _taskHelperService.Run(async () =>
            {
                var result = await _funcToAwait();

                NavigationService.CloseDialog(result);
            });
        }

        /// <summary>
        ///     Prepares the viewmodel and sets the function containing the task to complete and text to display.
        /// </summary>
        /// <param name="parameter">The DialogGenericActivityIndicatorData passed over to this viewmodel.</param>
        public override void Prepare(DialogGenericActivityIndicatorData parameter)
        {
            Text = parameter.Text;
            _funcToAwait = parameter.AwaitableFunction;
        }

        /// <summary>
        ///     Gets or sets the text to display below the activity indicator.
        /// </summary>
        [ObservableProperty]
        private string _text;
    }
}