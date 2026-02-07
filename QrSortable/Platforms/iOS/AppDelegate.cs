using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace QrSortable;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {

        AppCenter.LogLevel = LogLevel.Verbose;

        AppCenter.Start("ios=744f43a3-1c43-4dc2-bb92-d55acdb20fe6",
            typeof(Analytics),
            typeof(Crashes));

        return MauiProgram.CreateMauiApp(); 
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
}
