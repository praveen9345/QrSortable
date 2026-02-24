namespace QrSortable.Components.CoreFeatures.Onboarding.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Onboarding.Views;
    using QrSortable.Components.CoreFeatures.Settings;
    using QrSortable.Components.CoreFeatures.Settings.Models;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The view model of the language screen providing selection of application language from multiple language.
    /// </summary>
    public partial class SelectLanguageViewModel : BaseViewModel<bool>
    {
        private readonly ILanguageProvider _languageProvider;
        private readonly IGeneralInformationManager _generalInformationManager;

        private bool _isInitializing = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectLanguageViewModel" /> class.
        /// </summary>
        /// <param name="languageProvider">The <see cref="LanguageProvider" /> to provide the information of selected language.</param>
        /// <param name="generalInformationManager">The <see cref="GeneralInformationManager" /> to access general information.</param>
        public SelectLanguageViewModel(ILanguageProvider languageProvider, IGeneralInformationManager generalInformationManager)
        {
            _languageProvider = languageProvider;
            _generalInformationManager = generalInformationManager;

            // Load languages from provider
            LanguageItemList = new ObservableCollection<LanguageItem>(
                _languageProvider.AvailableLanguages);

            // Preselect current language
            SelectedLanguageItem = _languageProvider.SelectedLanguage;

            _isInitializing = false;

        }

        [ObservableProperty]
        public bool _isFromApp;

        [ObservableProperty]
        private ObservableCollection<LanguageItem> _languageItemList;

        [ObservableProperty]
        private LanguageItem _selectedLanguageItem;

        /// <summary>
        ///     Prepares the viewmodel with a boolean.
        /// </summary>
        /// <param name="isFromApp">
        ///     True when the view model is opened from the application and false when the view model is opened from onboarding.
        /// </param>
        public override void Prepare(bool isFromApp)
        {
            IsFromApp = isFromApp;
            IsBackNavigationEnabled = isFromApp;
        }

        public IAsyncRelayCommand<LanguageItem> OnSelectionCodeChangedCommand =>
         new AsyncRelayCommand<LanguageItem>(async selectedItem =>
         {
             if (selectedItem == null || _isInitializing)
                 return;

             // Persist language & set culture
             await _languageProvider.SetSelectedLanguageAsync(selectedItem);

             // Optionally update onboarding progress
             await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.OnboardingStarted);

              // Clear selection in UI
              SelectedLanguageItem = null;

              // If not using Shell, fallback to NavigationService
              await NavigationService.Navigate<OnboardingView>(false);
         });
    }
}
