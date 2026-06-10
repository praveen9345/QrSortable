namespace QrSortable
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.Reflection;
    /// <summary>
	///     AppShell class for describing the visual hierarchy of the application.
	/// </summary>
	public partial class AppShell : Shell
	{
        private static bool _routesRegistered;
        private static readonly object RouteLock = new();

        /// <summary>
        ///     AppShell constructor for initializing the component on the target OS.
        /// </summary>
        public AppShell()
		{
			InitializeComponent();
        }


        /// <summary>
        /// Registers routes exactly once.
        /// Call this after the main window has been created.
        /// </summary>
        public static void RegisterRoutes()
        {
            if (_routesRegistered)
            {
                return;
            }

            lock (RouteLock)
            {
                if (_routesRegistered)
                {
                    return;
                }


                var views = Assembly.GetExecutingAssembly().GetExportedTypes()
                                  .Where(t =>
                                      t.IsClass &&
                                      !t.IsAbstract &&
                                      t.Name.EndsWith("View", StringComparison.Ordinal))
                                  .ToList();


                foreach (var view in views)
                {
                    try
                    {
                        if (view.Name == "RootView")
                        {
                            Routing.RegisterRoute("//" + nameof(MainPage) + "/" + nameof(RootView), view);
                            continue;
                        }
                        Routing.RegisterRoute(view.Name, view);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[AppShell] Failed to register route for {view.Name}: {ex}");
                    }
                }

                _routesRegistered = true;
            }
        }
    }
}
