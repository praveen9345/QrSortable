namespace QrSortable.Components.CoreFeatures.Cloud
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     A service handling internet connectivity changes and information.
    /// </summary>
    public interface IConnectivityService
    {
        /// <summary>
        ///     Indicates whether an internet connection is currently available.
        /// </summary>
        /// <returns>True, if internet connection is available; otherwise false.</returns>
        Task<bool> CheckInternetConnectionAvailableAsync();

        /// <summary>
        ///     An event enabling listening for internet connectivity changes.
        /// </summary>
        event EventHandler InternetConnectivityChanged;
    }
}
