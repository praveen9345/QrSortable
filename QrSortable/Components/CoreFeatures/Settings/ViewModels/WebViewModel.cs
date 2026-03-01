namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{

    using CommunityToolkit.Mvvm.ComponentModel;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using UiFunctionality.Navigation.ViewModels;
    using static QRCoder.PayloadGenerator;

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
            switch(viewName)
            {
                case "feedback":
                    var langCode = (await _generalInformationManager.GetGeneralInformationAsync()).SelectedLanguageCode;
                    
                    Url = langCode switch
                    {
                        "de" => "https://forms.gle/FSoQkoajwYNASEMq8",// german
                        "es" => "https://forms.gle/aiHxRbrnfugz2ddKA",// spanish
                        "fr" => "https://forms.gle/GzjQw6CyMhExqkWg6",// french
                        _ => "https://forms.gle/5Cx6NTnz7boGvLUK8" // default: English
                    };
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