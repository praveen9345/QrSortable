namespace QrSortable.Components.CoreFeatures.AppStart
{
    using FirebaseAdmin;
    using Google.Apis.Auth.OAuth2;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.UiFunctionality.Navigation;
    using QrSortable.Components.UiFunctionality.Navigation.Views;


    /// <summary>
    /// Represents a service responsible for initializing and managing various application components.
    /// We use this in favor of the base class to migrate the databases on app start and to initialize
    /// the connection to App Center for error logging.
    /// </summary>
    public class AppService : IAppService
    {
        private readonly INavigationService _navigationService;
        
        /// <summary>
        ///     Initializes the application.
        /// </summary>
        public AppService()
        {
            _navigationService = ServiceHelper.GetService<INavigationService>();

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

    }
}
