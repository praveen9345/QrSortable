namespace QrSortable
{
	using QrSortable.Components.CoreFeatures.AppStart;
	 using Microsoft.AppCenter.Crashes;

    /// <summary>
    ///     Class representing the cross-platform application.
    /// </summary>
    public partial class App : Application
	{
		private readonly IAppService _appService;

		/// <summary>
		///     Initializes the application.
		/// </summary>
		/// <param name="appService">The service used by the application.</param>
		public App(IAppService appService)
		{
			
			InitializeComponent();
			_appService = appService;
        }

		/// <summary>
		/// Creates and returns a new window with an instance of AppShell as the content.
		/// </summary>
		/// <param name="activationState">The activation state for the window (optional).</param>
		/// <returns>A new Window object with the AppShell as its content.</returns>
		protected override Window CreateWindow(IActivationState? activationState)
		{
            
            return new Window(new AppShell());
		}

        /// <summary>
        /// Invoked when the application starts.
        /// </summary>
        /// <remarks>
        /// This method is called when the application is launched. It overrides the base class's <see cref="OnStart"/> method
        /// to perform necessary initialization tasks for the application. It calls the <c>OnStartAsync</c> method of the
        /// associated <see cref="AppService"/> instance asynchronously.
        /// </remarks>
        protected override async void OnStart()
        {
            base.OnStart();

#if IOS
    Console.WriteLine("=== AppCenter Crash Test ===");
    
    // Check if crashes are enabled
    bool isCrashesEnabled = await Crashes.IsEnabledAsync();
    Console.WriteLine($"Crashes Enabled: {isCrashesEnabled}");
    
    // Check for previous crash
    bool didCrash = await Crashes.HasCrashedInLastSessionAsync();
    Console.WriteLine($"Had previous crash: {didCrash}");
    
    if (didCrash)
    {
        var report = await Crashes.GetLastSessionCrashReportAsync();
        Console.WriteLine($"Previous crash ID: {report?.Id}");
    }
    
    // Wait for crash upload
    await Task.Delay(5000);
    
    // Initialize app
    await _appService.OnStartAsync();
    
    // Simple crash test - comment out after first successful test
    bool shouldTestCrash = Preferences.Get("TestCrashDone", false);
    if (!shouldTestCrash)
    {
        Preferences.Set("TestCrashDone", true);
        Console.WriteLine("CRASHING NOW!");
        await Task.Delay(1000);
        throw new Exception("Manual test crash - this should appear in AppCenter");
    }
    else
    {
        Console.WriteLine("Crash test already done. Check AppCenter portal.");
    }
#else
            await _appService.OnStartAsync();
#endif
        }

    }
}
