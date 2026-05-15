namespace QrSortable.Components.CoreFeatures.Settings.ViewModels
{
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Localization;
    using UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the feedback screen.
    /// </summary>
    public partial class HelpViewModel : BaseViewModel
    {
       
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;
        private readonly ISharedMethodService _sharedMethodService;
        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpViewModel" />.
        /// </summary>
        public HelpViewModel( IMauiEssentialsWrapper mauiEssentialsWrapper, ISharedMethodService sharedMethodService)
        {
            IsBackNavigationEnabled = true;
            _mauiEssentialsWrapper = mauiEssentialsWrapper;
            _sharedMethodService = sharedMethodService;
        }

        public AsyncRelayCommand UserMaualCommand => new AsyncRelayCommand(async () =>
        {
           if(!await _sharedMethodService.OpenUserManualAsync())
           {
                await DialogService.ShowAlertDialog(
               AppResources.Dialog_Error, AppResources.General_FileNotFoundErrorText, AppResources.Dialog_OK_Text);
           }
        });

        public AsyncRelayCommand EmailIdCommand => new AsyncRelayCommand(async () =>
        {
            var recipients = new List<string> { "qrsortable@gmail.com" };
            if (!await _mauiEssentialsWrapper.SendEmailAsync(AppResources.HelpViewModel_EmailTitleText, "", recipients))
            {
                await DialogService.ShowAlertDialog(
               AppResources.Dialog_Error, AppResources.HelpViewModel_EmailSendOutErrorText, AppResources.Dialog_OK_Text);
            }
        });  

    }
}