namespace QrSortable.Components.PlatformUtils.Wrappers
{
    /// <summary>
    ///     Wrapper class for essential functionalities for Maui applications. 
    /// </summary>
    public class MauiEssentialsWrapper : IMauiEssentialsWrapper
    {
        /// <summary>
        ///     Gets the device platform Android.
        /// </summary>
        public DevicePlatform AndroidDevicePlatform => DevicePlatform.Android;

        /// <summary>
        ///     Gets the device platform iOS.
        /// </summary>
        public DevicePlatform IosDevicePlatform => DevicePlatform.iOS;

          /// <summary>
        ///     Gets the device platform Windows.
        /// </summary>
        public DevicePlatform WindowsDevicePlatform => DevicePlatform.WinUI;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MauiEssentialsWrapper" /> class.
        /// </summary>
        public MauiEssentialsWrapper()
        {
            Connectivity.ConnectivityChanged += (sender, args) => ConnectivityChanged?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        ///     Dials the given phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to dial.</param>
        public bool DialPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                PhoneDialer.Open(phoneNumber);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MauiEssentialsWrapper.cs: DialPhoneNumberAsync:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Gets the current AppVersion.
        /// </summary>
        /// <returns>A string representing the current AppVersion.</returns>
        public string GetCurrentAppVersion()
        {
            return VersionTracking.CurrentVersion;
        }

        /// <summary>
        ///     Gets the current device platform.
        /// </summary>
        /// <returns>An instance of DevicePlatform according to the current device.</returns>
        public DevicePlatform GetDevicePlatform()
        {
            return DeviceInfo.Platform;
        }

        /// <summary>
        ///     Runs the given action on the main thread.
        /// </summary>
        /// <param name="action"> The action to be run on the main thread. </param>
        public void RunOnMainThread(Action action)
        {
            MainThread.BeginInvokeOnMainThread(action);
        }

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
        public async Task<bool> SendEmailAsync(string subject, string body, List<string> recipients)
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients
            };

            try
            {
                await Email.ComposeAsync(message);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("MauiEssentialsWrapper.cs: SendEmailAsync:" + exception.Message);
                return false;
            }
        }

        /// <summary>
        ///     Opens the link passed as a parameter.
        /// </summary>
        /// <param name="uri"> The uri to be opened. </param>
        /// <returns>
        ///     A task to await the completion of the functionality. Awaiting this task returns true if no exception
        ///     occurred. False, otherwise.
        /// </returns>
        public async Task<bool> OpenLinkAsync(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MauiEssentialsWrapper.cs: OpenLinkAsync:" + ex.Message);
                return false;
            }
        }

		/// <summary>
        ///    Triggers the authentication or password reset process against the given URL.
        ///    AADB2C90118 is the error code for "user forgot his password"
        ///    AADB2C90091 is the error code for "user cancelled the process"
        /// </summary>
        /// <param name="url">The authorization URL.</param>
        /// <param name="callbackUrl">The callback URL to enable back navigation to the app.</param>
        /// <returns>The result of the authentication.</returns>
        public async Task<WebAuthenticatorResult> AuthenticateAsync(Uri url, Uri callbackUrl)
        {
            return await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions
            {
                Url = url,
                CallbackUrl = callbackUrl,
                PrefersEphemeralWebBrowserSession = true
            });
        }

        /// <summary>
        ///     Stores given key-value pair into the Secure Storage.
        /// </summary>
        /// <param name="key">The key to access the value by.</param>
        /// <param name="value">The value.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SetSecureStorageValueAsync(string key, string value)
        {
            await SecureStorage.SetAsync(key, value);
        }

        /// <summary>
        ///     Retrieves value for given key from the Secure Storage.
        /// </summary>
        /// <param name="key">The key to access the value by.</param>
        /// <returns>An awaitable task with the value set as its result.</returns>
        public async Task<string> GetSecureStorageValueAsync(string key)
        {
            return await SecureStorage.GetAsync(key);
        }

        /// <summary>
        ///     Checks whether an internet connection is available.
        /// </summary>
        /// <returns>A value indicating whether unconstrained internet is available.</returns>
        public bool IsInternetConnectionAvailable()
        {
            try
            {
                return Connectivity.NetworkAccess == NetworkAccess.Internet;
            }
            catch (PermissionException exception)
            {
                Console.WriteLine("MauiEssentialsWrapper.cs: IsInternetConnectionAvailableAsync:" + exception);
                return false;
            }
        }

        /// <summary>
        ///     Triggers in case of a connectivity change.
        /// </summary>
        public event EventHandler ConnectivityChanged;

        /// <summary>
        ///     Checks if the app was started the first time.
        /// </summary>
        /// <returns> True if it is the first launch of the app. False, otherwise.</returns>
        public bool IsFirstLaunchEver()
        {
            return VersionTracking.IsFirstLaunchEver;
        }

        /// <summary>
        ///     Clears the secure storage.
        /// </summary>
        public void ClearSecureStorage()
        {
            SecureStorage.RemoveAll();
        }
    }
}
