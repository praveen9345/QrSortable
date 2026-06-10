namespace QrSortable.Components.CoreFeatures.AppStart
{
    using QrSortable.Components.CoreFeatures.DataManagement;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Onboarding.Views;
    using QrSortable.Components.CoreFeatures.Settings;
    using QrSortable.Components.CoreFeatures.Settings.Models;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using QrSortable.Components.UiFunctionality.Navigation;
    using QrSortable.Components.UiFunctionality.Navigation.Views;

    /// <summary>
    /// Represents a service responsible for initializing and managing various application components.
    /// We use this in favor of the base class to migrate the databases on app start and to initialize
    /// the connection to App Center for error logging.
    /// </summary>
    public class AppService : IAppService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly INavigationService _navigationService;
        private readonly IFileManager _fileManager;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly ILanguageProvider _languageProvider;

        private const string DatabaseName = "QrSortable.sqlite3";
        private const string BackendDatabaseName = "QrSortableBackend.sqlite3";

        /// <summary>
        ///     Initializes the application.
        /// </summary>
        public AppService()
        {
            _navigationService = ServiceHelper.GetService<INavigationService>();
            _mauiEssentialsWrapper = ServiceHelper.GetService<IMauiEssentialsWrapper>();
            _databaseManager = ServiceHelper.GetService<IDatabaseManager>();
            _fileManager = ServiceHelper.GetService<IFileManager>();
            _generalInformationManager = ServiceHelper.GetService<IGeneralInformationManager>();
            _languageProvider = ServiceHelper.GetService<ILanguageProvider>();

            var backendDatabaseManager = ServiceHelper.GetService<IBackendDatabaseManager>();
            backendDatabaseManager.Initialize(CreateNewBackendDbContext);

            _ = InitializeAsync();

        }

        /// <summary>
        ///     Ensures DB initialization and reset completes before setting the language,
        ///     avoiding race conditions between the two operations.
        /// </summary>
        private async Task InitializeAsync()
        {
            await ResetStorageAndDatabaseAfterReinstallAsync();
            _generalInformationManager.ResetGeneralInformation();
            await SetLanguageAsync();
        }

        /// <summary>
        ///     The method used for
        ///     - Performing the initial download of the ifu
        ///     - Redirecting to the Login screen.
        ///     - Setting the default culture
        /// </summary>
        public async Task OnStartAsync()
        {

            await NavigateToFirstViewModelAsync();
        }

        /// <summary>
        ///     The method used for
        ///     - Choosing and navigating to our first ViewModel
        /// </summary>
        private async Task NavigateToFirstViewModelAsync()
        {
            var generalInformation = await _generalInformationManager.GetGeneralInformationAsync();

            switch (generalInformation.OnboardingProgress)
            {
                case OnboardingProgress.NotStarted:
                    await _navigationService.Navigate<SelectLanguageView>(false);
                    break;
                case OnboardingProgress.OnboardingStarted:
                case OnboardingProgress.OnboardingCompleted:
                    await _navigationService.Navigate<RootView>();
                    break;
            }
        }

        private BaseDatabaseContext CreateNewDbContext()
        {
            var path = GetDatabasePathForCurrentPlatform(DatabaseName);

            return new DatabaseContext(path);
        }

        private BaseDatabaseContext CreateNewBackendDbContext()
        {
            var path = GetDatabasePathForCurrentPlatform(BackendDatabaseName);
            return new BackendDatabaseContext(path);
        }


        private string GetDatabasePathForCurrentPlatform(string name)
        {
            var currentPlatform = _mauiEssentialsWrapper.GetDevicePlatform();
            if (currentPlatform == _mauiEssentialsWrapper.AndroidDevicePlatform || currentPlatform == _mauiEssentialsWrapper.WindowsDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name);
            }

            if (currentPlatform == _mauiEssentialsWrapper.IosDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", "Database", name);

            }
            throw new NotImplementedException("The current platform is not supported");
        }

        private async Task ResetStorageAndDatabaseAfterReinstallAsync()
        {
            // Initialize the database first
            _databaseManager.Initialize(CreateNewDbContext);

            // Check if this is the first run using Preferences
            bool isFirstRun = !Preferences.ContainsKey("AppInitialized");

            // Also check if multiuser ID exists in the database
            var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
            bool needsMultiuserId = string.IsNullOrWhiteSpace(generalInfo?.MultiUserId);

            if (isFirstRun || needsMultiuserId)
            {
                // Mark as initialized
                Preferences.Set("AppInitialized", true);

                if (isFirstRun)
                {
                    // Only clear on true first run
                    _mauiEssentialsWrapper.ClearSecureStorage();
                    await _databaseManager.ClearDatabaseAsync();
                }

                // Set initial onboarding state (only if not already set)
                if (generalInfo == null || generalInfo.OnboardingProgress == OnboardingProgress.NotStarted || isFirstRun)
                {
                    await _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.NotStarted);
                }

                // Generate multiuser ID if missing
                if (needsMultiuserId)
                {
                    await _generalInformationManager.UpdateTheMultiuserIdAsync(GenerateMultiuserId());
                }
            }
        }


        private string GenerateMultiuserId()
        {
            string seg1 = GenerateNumber(4);
            string seg2 = GenerateNumber(5);

            return $"QS-{seg1}-{seg2}";
        }

        private static string GenerateNumber(int digits)
        {
            Random random = new Random();
            int max = (int)Math.Pow(10, digits);
            int min = max / 10;
            return random.Next(min, max).ToString();
        }

        /// <summary>
        ///     Sets the language saved in <see cref="GeneralInformation"/>.
        /// </summary>
        private async Task SetLanguageAsync()
        {
            var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
            // Use the string code from DB to recreate the LanguageItem
            if (string.IsNullOrEmpty(generalInfo.SelectedLanguageCode))
            {
                _languageProvider.SetDefaultLanguage();
            }
            else
            {
                _languageProvider.SelectedLanguage = new LanguageItem(generalInfo.SelectedLanguageCode);
            }
        }
    }
}
