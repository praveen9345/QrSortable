namespace QrSortable 
{
    using QrSortable.Components.Logging;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation;
    using QrSortable.Components.UiFunctionality.Navigation.Views;

    public partial class MainPage : ContentPage
    {
	    public MainPage()
	    {
		    InitializeComponent();
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

