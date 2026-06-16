namespace QrSortable
{
    using QrSortable.Components.Logging;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.Reflection;
    /// <summary>
	///     AppShell class for describing the visual hierarchy of the application.
	/// </summary>
	public partial class AppShell : Shell
    {

        /// <summary>
        ///     AppShell constructor for initializing the component on the target OS.
        /// </summary>
        public AppShell()
        {
            var logger = ServiceHelper.GetService<ILogger>();

            try
            {
                logger?.Log("AppShell Constructor START");

                InitializeComponent();

                logger?.Log("InitializeComponent Completed");

                var views = Assembly.GetExecutingAssembly()
                    .GetExportedTypes()
                    .Where(t => t.Name.EndsWith("View"))
                    .ToList();

                logger?.Log($"Found {views.Count} Views");

                foreach (var view in views)
                {
                    logger?.Log($"Registering {view.Name}");

                    if (view.Name == "RootView")
                    {
                        Routing.RegisterRoute(
                            "//" + nameof(MainPage) + "/" + nameof(RootView),
                            view);
                    }
                    else
                    {
                        Routing.RegisterRoute(view.Name, view);
                    }
                }

                logger?.Log("AppShell Constructor END");
            }
            catch (Exception ex)
            {
                logger?.LogException(ex);
            }
        }
    }
}