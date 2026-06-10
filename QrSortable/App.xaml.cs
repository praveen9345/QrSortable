namespace QrSortable
{
    using CommunityToolkit.Mvvm.Messaging;
    using QrSortable.Components.CoreFeatures.AppStart;
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

        private bool _startupCompleted;
        private readonly object _startupLock = new();

        public App(IAppService appService)
        {
            InitializeComponent();
            _appService = appService;
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
            lock (_startupLock)
            {
                if (_startupCompleted)
                {
                    return;
                }

                _startupCompleted = true;
            }

            try
            {

                // Important for iOS: dispatch startup to UI thread after Shell settles
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(1000); // small delay helps iOS Shell become ready
                    await _appService.OnStartAsync();
                });


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Startup failed: {ex}");

                throw;
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
