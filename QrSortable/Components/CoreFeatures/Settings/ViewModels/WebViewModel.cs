namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{

    using CommunityToolkit.Mvvm.ComponentModel;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the feedback screen.
    /// </summary>
    public partial class WebViewModel : BaseViewModel<string>
    {
        private readonly IMauiEssentialsWrapper _mauiEssentialWrapper;
        private readonly IGeneralInformationManager _generalInformationManager;
  
        /// <summary>
        ///     Initializes a new instance of the <see cref="WebViewModel" />.
        /// </summary>
        /// <param name="mauiEssentialsWrapper">An instance of <see cref="IMauiEssentialsWrapper" /> used to access platform-specific features.</param>
        public WebViewModel(IMauiEssentialsWrapper mauiEssentialsWrapper, IGeneralInformationManager generalInformationManager)
        {
            _mauiEssentialWrapper = mauiEssentialsWrapper;
            _generalInformationManager = generalInformationManager;

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
                await DialogService.ShowAlertDialog(AppResources.Dialog_InternetConnection_Title,
              AppResources.Dialog_InternetConnection_Message, AppResources.Dialog_OK_Text);
                await NavigationService.Close();
            }

            var langCode = (await _generalInformationManager.GetGeneralInformationAsync()).SelectedLanguageCode;
            
            switch (viewName)
            {       
                case "feedback":
                    Url = "https://www.qrsortable.com/#/app-feedback";
                    break;
                case "privacyPolicy":
                    Url = langCode switch
                    {
                        "de" => "https://sites.google.com/view/qrsortable-privacy-de?usp=sharing",// german
                        "es" => "https://sites.google.com/view/qrsortable-privacy-es?usp=sharing",// spanish
                        "fr" => "https://sites.google.com/view/qrsortable-privacy-fr?usp=sharing",// french
                        _ => "https://sites.google.com/view/qrsortable-privacy-en?usp=sharing" // default: English
                    };
                    break;
                case "termsAndCondition":
                    Url = langCode switch
                    {
                        "de" => "https://sites.google.com/view/qrsortable-terms-de?usp=sharing",// german
                        "es" => "https://sites.google.com/view/qrsortable-terms-es?usp=sharing",// spanish
                        "fr" => "https://sites.google.com/view/qrsortable-terms-fr?usp=sharing",// french
                        _ => "https://sites.google.com/view/qrsortable-terms-en?usp=sharing" // default: English
                    };
                    break;
                case "license":
                    Url = langCode switch
                    {
                        "de" => "https://sites.google.com/view/qrsortable-license-de?usp=sharing",// german
                        "es" => "https://sites.google.com/view/qrsortable-license-es?usp=sharing",// spanish
                        "fr" => "https://sites.google.com/view/qrsortable-license-fr?usp=sharing",// french
                        _ => "https://sites.google.com/view/qrsortable-license-en?usp=sharing" // default: English
                    };
                    break;
            }
        }
    }
}