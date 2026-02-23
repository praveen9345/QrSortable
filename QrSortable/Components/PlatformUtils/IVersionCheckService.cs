namespace QrSortable.Components.PlatformUtils
{
    /// <summary>
    ///     The service used for checking the version of the app and to open the app store.
    /// </summary>
    public interface IVersionCheckService
    {
        /// <summary>
        ///     Returns the current country code.
        /// </summary>
        /// <returns>The country code.</returns>
        string GetCountryCode();

        /// <summary>
        ///     Returns whether the app used is the latest version by comparing it to the store version number.
        /// </summary>
        /// <returns>True, if the app version is equal or higher than the store version.</returns>
        Task<bool> IsUsingLatestVersion();

        /// <summary>
        ///     Opens the app store to the app.
        /// </summary>
        Task OpenAppInStore();
    }
}