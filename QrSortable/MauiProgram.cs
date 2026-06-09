namespace QrSortable
{
    using System.Reflection;
    using CommunityToolkit.Maui;
    using Microsoft.Maui.Controls.Compatibility.Hosting;
    using QrSortable.Components.PlatformUtils;
    using BarcodeScanning;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.AppStart;
    using QrSortable.Components.UiFunctionality.Navigation;
    using QrSortable.Components.CoreFeatures.Assistant.Helpers;
    using QrSortable.Components.CoreFeatures.Assistant;
    using QrSortable.Components.CoreFeatures.Cloud;
    using QrSortable.Components.CoreFeatures.Cloud.BackendCommunication;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using QrSortable.Components.CoreFeatures.CodeGenerator;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.OrdersPayments;
    using QrSortable.Components.CoreFeatures.Scanner;
    using QrSortable.Components.CoreFeatures.Settings.Wrappers;
    using QrSortable.Components.CoreFeatures.Settings;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.TimeHandling;
    using QrSortable.Components.UiFunctionality.Localization;
    using QrSortable.Components.UiFunctionality.Notification;
    using Microsoft.Extensions.DependencyInjection;
    using PdfSharpCore.Fonts;

#if ANDROID
    using Android.Graphics.Drawables;
    using Android.Graphics;
    using Microsoft.Maui.Controls.Handlers;
    using Android.Widget;
    using Platforms.Android.Components.PlatformUtils;

#elif IOS
    using UIKit;
    using QrSortable.Platforms.iOS.Components.PlatformUtils;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Crashes;
    using Microsoft.AppCenter.Analytics;
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
                .UseBarcodeScanning()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("Itim-Regular.ttf", "ItimRegular");
                    fonts.AddFont("FluentSystemIcons-Filled.ttf", "FluentIcons");
				})
                .RegisterCoreServices()
                .RegisterFeatureServices()
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

            // Initialize App Center (outside of handlers configuration)
#if IOS
		            AppCenter.Start("ios=744f43a3-1c43-4dc2-bb92-d55acdb20fe6",typeof(Analytics),typeof(Crashes));
                    
                    // Enable verbose logging for debugging
                    AppCenter.LogLevel = LogLevel.Verbose;
#endif

            PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new PdfFontResolver();
            
            var app = builder.Build();
            // Needs to be initialized after building the app to link the services to the created singletons.
            ServiceHelper.Initialize(app.Services);
			return app;
		}


        /// <summary>
        /// Register app-wide / platform / startup-critical services here.
        /// Use Singleton for true app-wide services.
        /// </summary>
        public static MauiAppBuilder RegisterCoreServices(this MauiAppBuilder builder)
        {
            // =========================================================
            // PLATFORM SERVICES
            // =========================================================
#if ANDROID
            builder.Services.AddSingleton<IImageService, AndroidImageService>();
            builder.Services.AddSingleton<IPermissionService, AndroidPermissionService>();
            builder.Services.AddSingleton<IVersionCheckService, AndroidVersionCheckService>();
#elif IOS
            builder.Services.AddSingleton<IImageService, IosImageService>();
            builder.Services.AddSingleton<IPermissionService, IosPermissionService>();
            builder.Services.AddSingleton<IVersionCheckService, IosVersionCheckService>();
#endif

            builder.Services.AddSingleton<IAppService, AppService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            builder.Services.AddSingleton<IBackendCommunicationService, BackendCommunicationService>();
            builder.Services.AddSingleton<IBackendSynchronizationManager, BackendSynchronizationManager>();
            builder.Services.AddSingleton<IGeneralDatabaseSynchronizationManager, GeneralDatabaseSynchronizationManager>();
            builder.Services.AddSingleton<IBackendDatabaseManager, BackendDatabaseManager>();
            builder.Services.AddSingleton<IGeneralInformationManager, GeneralInformationManager>();
            builder.Services.AddSingleton<IDatabaseManager, DatabaseManager>();
            builder.Services.AddSingleton<IDatabaseQueueProvider, DatabaseQueueProvider>(); 
            builder.Services.AddSingleton<IMauiEssentialsWrapper, MauiEssentialsWrapper>();
            builder.Services.AddSingleton<INavigationShellWrapper, NavigationShellWrapper>(); 
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<ILanguageProvider, LanguageProvider>();

            builder.Services.AddSingleton<IDeepLinkService, DeepLinkService>();
            builder.Services.AddSingleton<ITimerService, TimerService>();
            builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

            return builder;
        }

        /// <summary>
        /// Register helpers / generators / validators / builders / wrappers here.
        /// These should usually be Transient unless they truly need shared global state.
        /// </summary>
        public static MauiAppBuilder RegisterFeatureServices(this MauiAppBuilder builder)
        {
            
            //helper
            builder.Services.AddTransient<IStorageFinderHelper, StorageFinderHelper>();
            builder.Services.AddTransient<IAesHelper, AesHelper>();

            //Services
            builder.Services.AddTransient<IStorageVoiceAssistantService, StorageVoiceAssistantService>();
            builder.Services.AddTransient<ICodeGeneratorService, CodeGeneratorService>();
            builder.Services.AddTransient<IPdfGeneratorService, PdfGeneratorService>();
            builder.Services.AddTransient<IMollieService, MollieService>();
            builder.Services.AddTransient<IFilePickerService, FilePickerService>();
            builder.Services.AddTransient<ICultureInfoWrapper, CultureInfoWrapper>();
            builder.Services.AddTransient<IFileWrapper, FileWrapper>();
            builder.Services.AddTransient<IFileManager, FileManager>();
            builder.Services.AddTransient<ISharedMethodService, SharedMethodService>();
            builder.Services.AddTransient<ITaskHelperService, TaskHelperService>();
            builder.Services.AddTransient<IDialogService, DialogService>();
            builder.Services.AddTransient<IToastService, ToastService>();
            builder.Services.AddTransient<IBackendDatabaseHelper, BackendDatabaseHelper>();
            builder.Services.AddTransient<IStorageFinderService, StorageFinderService>();
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

