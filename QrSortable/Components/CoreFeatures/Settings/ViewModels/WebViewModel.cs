namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{

    using CommunityToolkit.Mvvm.ComponentModel;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the feedback screen.
    /// </summary>
    public partial class WebViewModel : BaseViewModel<string>
    {
        private readonly IMauiEssentialsWrapper _mauiEssentialWrapper;
  
        /// <summary>
        ///     Initializes a new instance of the <see cref="WebViewModel" />.
        /// </summary>
        /// <param name="mauiEssentialsWrapper">An instance of <see cref="IMauiEssentialsWrapper" /> used to access platform-specific features.</param>
        public WebViewModel(IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            _mauiEssentialWrapper = mauiEssentialsWrapper;
            IsBackNavigationEnabled = true;
        }

        [ObservableProperty]
        private string _url;

        /// <summary>
        /// Prepares the specified view by setting the URL based on the view name. 
        /// Displays an alert if there is no internet connection.
        /// </summary>
        /// <param name="viewName">The name of the view to prepare. 
        /// Valid options are "feedback", "privacyPolicy", "termsAndCondition", and "license".</param>
        public override async void Prepare(string viewName)
        {
            if(!_mauiEssentialWrapper.IsInternetConnectionAvailable())
            {
                await DialogService.ShowAlertDialog("🌐 Connectivity",
                "Internet Connection is required.", AppResources.Dialog_OK_Text);
                await NavigationService.Close();
            }
            switch(viewName)
            {
                case "feedback":
                    Url = "https://forms.gle/rjApAC6Z69GpEo458";
                    break;
                case "privacyPolicy":
                    Url = "https://kidsytales.com/privacy-policy";
                    break;
                case "termsAndCondition":
                    Url = "https://kidsytales.com/terms-conditions";
                    break;
                case "license":
                    Url = "https://kidsytales.com/license-information";
                    break;
            }
        }
    }
}