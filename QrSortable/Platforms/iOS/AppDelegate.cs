using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels;

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

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
        if (url != null)
        {
            var uri = new Uri(url.AbsoluteString);

            // Parse the query: myapp://payment-return?id=xxx
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var paymentId = query.Get("id");

            if (!string.IsNullOrEmpty(paymentId))
            {
                Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Microsoft.Maui.Controls.Application.Current?.MainPage?.BindingContext
                        is PaymentShipmentViewModel vm)
                    {
                        vm.HandleMollieRedirect(paymentId);
                    }
                });
            }
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
