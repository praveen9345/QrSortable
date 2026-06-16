namespace QrSortable
{
    using CommunityToolkit.Mvvm.Messaging;
    using QrSortable.Components.CoreFeatures.AppStart;
    using QrSortable.Components.Logging;
    using QrSortable.Components.UiFunctionality.Navigation.Helper;

    /// <summary>
    /// Cross-platform application entry.
    /// 
    /// Key startup fix:
    /// - Do NOT run startup work too early in OnStart()
    /// - Wait until the main Window is created
    /// - Then register routes and run startup initialization
    /// 
    /// This reduces startup pressure during iOS UIKit window/safe-area setup.
    /// </summary>
    public partial class App : Application
    {
        private readonly IAppService _appService;
        private readonly ILogger _logger;

        private bool _startupCompleted;
        private readonly object _startupLock = new();

        public App(IAppService appService, ILogger logger)
        {
           
            _logger = logger;

            _logger.Log("App Constructor START");

            InitializeComponent();

            _appService = appService;

            _logger.Log("App Constructor END");

        }

        /// <summary>
        /// Creates the main application window with AppShell.
        /// </summary>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            // Window lifecycle hooks
            window.Created += OnWindowCreated;
            window.Resumed += OnWindowResumed;
            window.Destroying += OnWindowDestroying;

            return window;
        }

        /// <summary>
        /// Runs once after the main window is created.
        /// This is a safer place than OnStart() for startup work on iOS.
        /// </summary>
        private async void OnWindowCreated(object? sender, EventArgs e)
        {
            _logger.Log("Window Created");

            lock (_startupLock)
            {
                if (_startupCompleted)
                {
                    _logger.Log("Startup Already Completed");
                    return;
                }

                _startupCompleted = true;
            }

            try
            {
                _logger.Log("Before Delay");

                // Important for iOS: dispatch startup to UI thread after Shell settles
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(50); // small delay helps iOS Shell become ready

                    _logger.Log("After Delay");

                    _logger.Log("Before OnStartAsync");

                    await _appService.OnStartAsync();

                    _logger.Log("After OnStartAsync");
                });


            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        /// <summary>
        /// Invoked when the app resumes from background.
        /// Broadcasts a message so any subscriber (e.g. RootViewModel) can react.
        /// </summary>
        private void OnWindowResumed(object? sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new AppResumedMessage());
        }

        /// <summary>
        /// Cleanup window event subscriptions.
        /// </summary>
        private void OnWindowDestroying(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                window.Created -= OnWindowCreated;
                window.Resumed -= OnWindowResumed;
                window.Destroying -= OnWindowDestroying;
            }
        }
    }
}
