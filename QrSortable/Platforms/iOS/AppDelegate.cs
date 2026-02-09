using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

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
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Console.WriteLine("🔥 iOS UnhandledException:");
            Console.WriteLine(e.ExceptionObject?.ToString());
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Console.WriteLine("🔥 iOS UnobservedTaskException:");
            Console.WriteLine(e.Exception?.ToString());
            e.SetObserved();
        };

        return base.FinishedLaunching(app, options);
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
