namespace QrSortable.Platforms.Android.Components.PlatformUtils
{
    using global::Android.Content;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using Uri = System.Uri;
    using QrSortable.Components.PlatformUtils;

    /// <summary>
    ///     The service used for checking the version of the app and to open the app store.
    /// </summary>
    public class AndroidVersionCheckService : IVersionCheckService
    {

        private string PackageName => "com.danfe.qrsortable";
        private Version InstalledVersion => AppInfo.Version;


        /// <summary>
        ///     Returns the current country code.
        /// </summary>
        /// <returns>The country code.</returns>
        public string GetCountryCode()
        {
            try
            {
                // Fetch the default locale's country code
                var locale = Java.Util.Locale.Default;
                return locale.Country ?? "US"; // Fallback to "US" if no country code is found
            }
            catch
            {
                return "US"; // Fallback in case of any exceptions
            }
        }

        /// <summary>
        ///     Returns whether the app used is the latest version by comparing it to the store version number.
        /// </summary>
        /// <returns>True, if the app version is equal or higher than the store version.</returns>
        public async Task<bool> IsUsingLatestVersion()
        {
            var storeVersion = await GetLatestVersionNumber();

            return Version.Parse(storeVersion) <= InstalledVersion;
        }

        private async Task<string> GetLatestVersionNumber()
        {
            var version = string.Empty;
            var url = $"https://play.google.com/store/apps/details?id={PackageName}&hl={GetCountryCode()}";

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string raw = await httpClient.GetStringAsync(new Uri(url));


                    var versionMatch = Regex.Match(raw, @"\[\[""\d+.\d+.\d+""\]\]"); //look for pattern [["X.Y.Z"]]
                    if (versionMatch.Groups.Count == 1)
                    {
                        var versionMatchGroup = versionMatch.Groups[0];
                        if (versionMatchGroup.Success)
                            version = versionMatch.Value.Replace("[", "").Replace("]", "").Replace("\"", "");
                    }

                }
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                Console.WriteLine("AndroidVersionCheckService.cs: " + ex);
            }

            return version;
        }

        /// <summary>
        ///     Opens the app store to the app.
        /// </summary>
        public async Task OpenAppInStore()
        {

            try
            {
                var intent = new Intent(Intent.ActionView,
                    global::Android.Net.Uri.Parse($"market://details?id={PackageName}"));
                intent.SetPackage("com.android.vending");
                intent.SetFlags(ActivityFlags.NewTask);
                if (Platform.CurrentActivity?.Application?.ApplicationContext != null)
                    Platform.CurrentActivity?.Application?.ApplicationContext.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                var intent = new Intent(Intent.ActionView,
                    global::Android.Net.Uri.Parse($"https://play.google.com/store/apps/details?id={PackageName}"));
                if (Platform.CurrentActivity?.Application?.ApplicationContext != null)
                    Platform.CurrentActivity?.Application?.ApplicationContext.StartActivity(intent);
            }
        }
    }
}
