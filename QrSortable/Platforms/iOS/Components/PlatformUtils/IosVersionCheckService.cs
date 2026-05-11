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

        private string BundleVersion => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();

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

                return Version.Parse(_app.Version) <= Version.Parse(BundleVersion);
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                Console.WriteLine("IosVersionCheckService .cs: " + ex);
                return true;
            }
        }

        /// <summary>
        ///     Opens the app store to the app.
        /// </summary>
        public async Task OpenAppInStore()
        {
            try
            {
                _app ??= await LookupApp();
                var options = new UIApplicationOpenUrlOptions();
                UIKit.UIApplication.SharedApplication.OpenUrl(new NSUrl($"{_app.Url}"), options, null);
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                Console.WriteLine("IosVersionCheckService.cs: OpenAppInStore :" + ex);
            }
        }

        private async Task<StoreAppData> LookupApp()
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"http://itunes.apple.com/{GetCountryCode()}/lookup?bundleId={BundleIdentifier}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AppDataRoot>(content).Results.FirstOrDefault();
        }
    }
}