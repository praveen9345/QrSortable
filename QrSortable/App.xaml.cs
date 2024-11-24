namespace QrSortable
{
	using QrSortable.Components.CoreFeatures.AppStart;

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
			await _appService.OnStartAsync();
        }

    }
}