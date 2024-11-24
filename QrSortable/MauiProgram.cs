namespace QrSortable
{
    using System.Reflection;
    using CommunityToolkit.Maui;
    using Microsoft.Maui.Controls.Compatibility.Hosting;
    using QrSortable.Components.PlatformUtils;

    #if ANDROID
      using Android.Graphics.Drawables;
      using Android.Graphics;
      using Microsoft.Maui.Controls.Handlers;
      using Android.Widget;
    #elif IOS
      using UIKit;
    #endif

    public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.UseMauiCompatibility()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("Itim-Regular.ttf", "ItimRegular");
                    fonts.AddFont("FluentSystemIcons-Filled.ttf", "FluentIcons");
				})
				.RegisterServices()
                .RegisterViewsAndViewModels()
                .ConfigureMauiHandlers(handlers =>
                {
                #if ANDROID
                    Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Entry", (handler, _) =>
                    {  
                        handler.PlatformView.Background = null;
                    });
                #elif IOS
                    Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Entry", (handler, _) =>
                    {  
                        handler.PlatformView.BorderStyle = UITextBorderStyle.None;
                    });
                #endif

                });

			var app = builder.Build();
            // Needs to be initialized after building the app to link the services to the created singletons.
            ServiceHelper.Initialize(app.Services);
			return app;
		}

		/// <summary>
        ///     Registers all classes of which the name ends with "Service" 
        ///     and that have an interface whose name ends with the name of the service.
        /// </summary>
        /// <param name="builder">The app in which the services are registered.</param>
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {

            string[] singletonTypeEndings =
            {
                "Service", "Manager", "Wrapper", "Logger", "Provider", "Scheduler", "Handler", "Client", "Builder",
                "Validator", "Helper"
            };

            var exportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();

            foreach (var singletonType in singletonTypeEndings)
            {
                foreach (var service in exportedTypes)
                {
                    if (!service.IsInterface && service.Name.EndsWith(singletonType)
                                             && !service.Name.EndsWith("HttpClientWrapper") && !service.IsAbstract)
                    {
                        var interfaceType = service.GetInterfaces().FirstOrDefault(type =>
                            type.Name.EndsWith(service.Name));

                        if (interfaceType != null)
                        {
                            builder.Services.AddSingleton(interfaceType, service);
                        }
                    }
                }
            }

            return builder;
        }

        /// <summary>
        ///     Registers all classes of which the name ends with "ViewModel" 
        ///     and tries to register a matching view for each view model.
        /// </summary>
        /// <param name="builder">The app builder.</param>
        public static MauiAppBuilder RegisterViewsAndViewModels(this MauiAppBuilder builder)
        {
            var types = Assembly.GetExecutingAssembly().GetExportedTypes();

            foreach (var type in types)
            {
                if (type.Name.EndsWith("ViewModel") && !type.IsAbstract)
                {
                    var viewType = types.FirstOrDefault(t => t.Name == type.Name.Replace("ViewModel", "View"));
                    if (viewType != null)
                    {
                        builder.Services.AddTransient(type);
                        builder.Services.AddTransient(viewType);
                    }
                }
            }
            return builder;
        }
	}
}

