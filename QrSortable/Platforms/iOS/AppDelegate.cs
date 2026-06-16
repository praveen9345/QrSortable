using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels;
using QrSortable.Components.CoreFeatures.OrdersPayments;
using QrSortable.Components.Logging;

namespace QrSortable;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }

    public override bool WillFinishLaunching(UIApplication uiApplication, NSDictionary launchOptions)
    {
        CreateDirectoriesAndSetSkipBackupAttribute();
        return base.WillFinishLaunching(uiApplication, launchOptions);
    }

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        var logger =IPlatformApplication.Current?.Services.GetRequiredService<ILogger>();

        logger?.Log("FinishedLaunching");

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            logger?.Log("UnhandledException");

            if (e.ExceptionObject is Exception ex)
            {
                logger?.LogException(ex);
            }
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            logger?.Log("🔥 iOS UnobservedTaskException:");
            logger?.LogException(e.Exception);
            e.SetObserved();
        };

        return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
        if (url != null)
        {
            var uri = new Uri(url.AbsoluteString);

            var deepLinkService =
            IPlatformApplication.Current.Services.GetRequiredService<IDeepLinkService>();


            _ = deepLinkService.HandleAsync(uri);

        }

        return true;
    }

    private void CreateDirectoriesAndSetSkipBackupAttribute()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var libraryPath = Path.Combine(documents, "..", "Library");
        var dbPath = Path.Combine(libraryPath, "Database");

        Directory.CreateDirectory(dbPath);

        NSFileManager.SetSkipBackupAttribute(libraryPath, true);
        NSFileManager.SetSkipBackupAttribute(dbPath, true);
    }
}
