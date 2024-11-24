namespace QrSortable.Components.PlatformUtils.Wrappers
{
    /// <summary>
    ///     Wrapper interface for essential functionalities for Maui applications. 
    /// </summary>
    public interface IMauiEssentialsWrapper
    {
        /// <summary>
        ///     Gets the device platform Android.
        /// </summary>
        DevicePlatform AndroidDevicePlatform { get; }

        /// <summary>
        ///     Gets the device platform iOS.
        /// </summary>
        DevicePlatform IosDevicePlatform { get; }

            /// <summary>
        ///     Gets the device platform windows.
        /// </summary>
        DevicePlatform WindowsDevicePlatform { get; }

        /// <summary>
        ///     Gets the current device platform.
        /// </summary>
        /// <returns>An instance of DevicePlatform according to the current device.</returns>
        DevicePlatform GetDevicePlatform();

        /// <summary>
        ///     Runs the given action on the main thread.
        /// </summary>
        /// <param name="action"> The action to be run on the main thread. </param>
        void RunOnMainThread(Action action);

        /// <summary>
        ///     Dials the given phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to dial.</param>
        /// <returns>A value indicating whether the call succeeded.</returns>
        bool DialPhoneNumberAsync(string phoneNumber);

        /// <summary>
        ///     Gets the AppVersion number.
        /// </summary>
        /// <returns>The current AppVersion number</returns>
        string GetCurrentAppVersion();

        /// <summary>
        ///     Accesses the email functionality with the predefined parameters.
        /// </summary>
        /// <param name="subject"> The subject of the mail. </param>
        /// <param name="body"> The body of the mail. </param>
        /// <param name="recipients"> The recipients of the mail. </param>
        /// <returns>
        ///     A task to await the completion of the functionality. Awaiting this task returns true if no exception
        ///     occurred. False, otherwise.
        /// </returns>
        Task<bool> SendEmailAsync(string subject, string body, List<string> recipients);

        /// <summary>
        ///     Opens the link passed as a parameter.
        /// </summary>
        /// <param name="uri"> The uri to be opened. </param>
        /// <returns>
        ///     A task to await the completion of the functionality. Awaiting this task returns true if no exception
        ///     occurred. False, otherwise.
        /// </returns>
        Task<bool> OpenLinkAsync(Uri uri);

        /// <summary>
        ///    Triggers the authentication process against the given URL.
        /// </summary>
        /// <param name="url">The authorization URL.</param>
        /// <param name="callbackUrl">The callback URL to enable back navigation to the app.</param>
        /// <returns>The property of the authentication.</returns>
        Task<WebAuthenticatorResult> AuthenticateAsync(Uri url, Uri callbackUrl);

        /// <summary>
        ///     Stores given key-value pair into the Secure Storage.
        /// </summary>
        /// <param name="key">The key to access the value by.</param>
        /// <param name="value">The value.</param>
        /// <returns>An awaitable task.</returns>
        Task SetSecureStorageValueAsync(string key, string value);

        /// <summary>
        ///     Retrieves value for given key from the Secure Storage.
        /// </summary>
        /// <param name="key">The key to access the value by.</param>
        /// <returns>An awaitable task with the value set as its result.</returns>
        Task<string> GetSecureStorageValueAsync(string key);

        /// <summary>
        ///     Checks whether an internet connection is available.
        /// </summary>
        /// <returns>A value indicating whether unconstrained internet is available.</returns>
        bool IsInternetConnectionAvailable();

        /// <summary>
        ///     Triggers in case of a connectivity change.
        /// </summary>
        public event EventHandler ConnectivityChanged;

        /// <summary>
        ///     Checks if the app was started the first time.
        /// </summary>
        /// <returns> True if it is the first launch of the app. False, otherwise.</returns>
        bool IsFirstLaunchEver();

        /// <summary>
        ///     Clears the secure storage.
        /// </summary>
        void ClearSecureStorage();
    }
}