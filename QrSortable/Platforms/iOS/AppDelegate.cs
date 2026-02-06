using Foundation;
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
}
