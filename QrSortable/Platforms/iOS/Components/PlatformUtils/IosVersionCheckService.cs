namespace QrSortable.Platforms.iOS.Components.PlatformUtils
{
    using Foundation;
    using Newtonsoft.Json;
    using QrSortable.Components.PlatformUtils;
    using UIKit;

    /// <summary>
    ///     The service used for checking the version of the app and to open the app store.
    /// </summary>
    public class IosVersionCheckService : IVersionCheckService
    {
        private StoreAppData _app;
        private string BundleIdentifier => "com.danfe.qrsortable";

        private string BundleVersion => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString()?.ToString() ?? "0.0.0";

        private class AppDataRoot
        {
            [JsonProperty("results")]
            public List<StoreAppData> Results { get; set; }
        }

        private class StoreAppData
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("trackViewUrl")]
            public string Url { get; set; }
        }

        /// <summary>
        ///     Returns the current country code.
        /// </summary>
        /// <returns>The country code.</returns>
        public string GetCountryCode()
        {
            try
            {
                var locale = NSLocale.CurrentLocale.CountryCode;
                return locale ?? "US";
            }
            catch
            {
                return "US";
            }
        }

        /// <summary>
        ///     Returns whether the app used is the latest version by comparing it to the store version number.
        /// </summary>
        /// <returns>True, if the app version is equal or higher than the store version.</returns>
        public async Task<bool> IsUsingLatestVersion()
        {
            try
            {
                _app = await LookupApp();

                if (_app == null)
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(_app.Version))
                {
                    return true;
                }

                if (!Version.TryParse(_app.Version, out var storeVersion))
                {
                    return true;
                }

                if (!Version.TryParse(BundleVersion, out var currentVersion))
                {
                    return true;
                }

                return currentVersion >= storeVersion;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                   $"IosVersionCheckService.IsUsingLatestVersion failed: {ex}");
                return true;
            }
        }
        /// <summary>
        /// Opens the App Store page.
        /// </summary>
        public async Task OpenAppInStore()
        {
            try
            {
                _app ??= await LookupApp();

                if (string.IsNullOrWhiteSpace(_app?.Url))
                {
                    return;
                }

                var url = new NSUrl(_app.Url);

                if (url == null)
                {
                    return;
                }

                UIApplication.SharedApplication.OpenUrl( url, new UIApplicationOpenUrlOptions(),null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"IosVersionCheckService.OpenAppInStore failed: {ex}");
            }
        }

        private async Task<StoreAppData?> LookupApp()
        {
            try
            {
                using var http = new HttpClient();

                var requestUrl =
                    $"https://itunes.apple.com/lookup?bundleId={BundleIdentifier}";

                var response = await http.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }

                var root = JsonConvert.DeserializeObject<AppDataRoot>(content);

                if (root?.Results == null || root.Results.Count == 0)
                {
                    return null;
                }

                return root.Results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"IosVersionCheckService.LookupApp failed: {ex}");
                return null;
            }
        }
    }
}