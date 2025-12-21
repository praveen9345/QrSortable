namespace QrSortable.Components.CoreFeatures.Cloud
{
    using System;
    using System.Threading.Tasks;
    using PlatformUtils.Wrappers;

    /// <summary>
    ///     The implementation of the <see cref="IConnectivityService"/>, which
    ///     uses Maui Essentials to access the connectivity information of the device.
    /// </summary>
    public class ConnectivityService : IConnectivityService
    {
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        /// <summary>
        ///     Initializes an instance of <see cref="ConnectivityService"/> class.
        /// </summary>
        /// <param name="mauiEssentialsWrapper">The maui wrapper to access device connectivity information.</param>
        public ConnectivityService(IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            _mauiEssentialsWrapper = mauiEssentialsWrapper;
            _mauiEssentialsWrapper.ConnectivityChanged += (sender, args) => InternetConnectivityChanged?.Invoke(sender, args);
        }

        /// <summary>
        ///     Indicates whether an internet connection is currently available.
        /// </summary>
        /// <returns>True, if internet connection is available; otherwise false.</returns>
        public Task<bool> CheckInternetConnectionAvailableAsync()
            => Task.FromResult(_mauiEssentialsWrapper.IsInternetConnectionAvailable());


        /// <summary>
        ///     An event enabling listening for internet connectivity changes.
        /// </summary>
        public event EventHandler InternetConnectivityChanged;
    }
}
