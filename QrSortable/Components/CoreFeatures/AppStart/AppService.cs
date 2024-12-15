namespace QrSortable.Components.CoreFeatures.AppStart
{
    using System.Text;
    using FirebaseAdmin;
    using Google.Apis.Auth.OAuth2;
    using QrSortable.Components.CoreFeatures.DataManagement;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
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

        private const string DatabaseName = "QrSortable.sqlite3";

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

            ResetStorageAndDatabaseAfterReinstall();

        }

        /// <summary>
        ///     The method used for
        ///     - Performing the initial download of the ifu
        ///     - Redirecting to the Login screen.
        ///     - Setting the default culture
        /// </summary>
        public async Task OnStartAsync()
        {
            await ConfigureAndInitializeFirebaseAsync();
            await NavigateToFirstViewModelAsync();     
        }

        /// <summary>
        ///     The method used for
        ///     - Choosing and navigating to our first ViewModel
        /// </summary>
        private async Task NavigateToFirstViewModelAsync()
        {
           await _navigationService.Navigate<RootView>();
        }

        private async Task ConfigureAndInitializeFirebaseAsync()
        {
            var localPath = Path.Combine(FileSystem.CacheDirectory, "admin-sdk.json");
            if (File.Exists(localPath))
                File.Delete(localPath);
            using (var jsonStream = await FileSystem.OpenAppPackageFileAsync("admin-sdk.json"))
            using (var destStream = File.Create(localPath))
            {
                await jsonStream.CopyToAsync(destStream);
            }

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(localPath)
            });
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", localPath);
        }

        private BaseDatabaseContext CreateNewDbContext()
        {
            var path = GetDatabasePathForCurrentPlatform(DatabaseName);

            return new DatabaseContext(path);
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

        private void ResetStorageAndDatabaseAfterReinstall()
        {
            // Workaround: for clearing the storage and database after reinstalling
            var fileTask = _fileManager.WriteFileToFileSystemAsync("QrSortable.txt", Encoding.UTF8.GetBytes("QrSortable"));
            var file = fileTask.Result;

            if (file)
            {
                _mauiEssentialsWrapper.ClearSecureStorage();
                _databaseManager.ClearDatabaseAsync();
            }

            _databaseManager.Initialize(CreateNewDbContext);

            if (file)
            {
                _generalInformationManager.UpdateOnboardingProgressAsync(OnboardingProgress.NotStarted);
            }
        }

    }
}
