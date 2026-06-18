namespace QrSortable 
{
    using QrSortable.Components.Logging;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using Microsoft.Maui.Controls;

#if ANDROID
    using Microsoft.Maui.ApplicationModel;
#endif

#if IOS
    using UIKit;
#endif


    public partial class MainPage : ContentPage
    {
	    public MainPage()
	    {
		    InitializeComponent();

#if ANDROID
            var activity = Platform.CurrentActivity;
            activity?.Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#525252"));
#endif

#if IOS
        UIKit.UIApplication.SharedApplication.StatusBarStyle = UIKit.UIStatusBarStyle.LightContent;
#endif
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            var logger = ServiceHelper.GetService<ILogger>();

            logger.Log(" MainPage: Before Navigation");
            var navigationService = ServiceHelper.GetService<INavigationService>();

            logger.Log(" MainPage: After Navigation");

            logger.Log(" MainPage: Before RootView");
            await navigationService.Navigate<RootView>();
            logger.Log(" MainPage: After RootView");
        }
    }

}

