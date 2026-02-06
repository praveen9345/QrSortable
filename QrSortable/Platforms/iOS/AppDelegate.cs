using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

namespace QrSortable;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        // Initialize ONLY Crashes (no analytics)
        AppCenter.Start("ios={744f43a3-1c43-4dc2-bb92-d55acdb20fe6}", typeof(Crashes));

        return MauiProgram.CreateMauiApp(); 
    }
}
