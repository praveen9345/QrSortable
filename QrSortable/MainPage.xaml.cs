namespace QrSortable 
{
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
            var navigationService = ServiceHelper.GetService<INavigationService>();

            await navigationService.Navigate<RootView>();
        }
    }

}

