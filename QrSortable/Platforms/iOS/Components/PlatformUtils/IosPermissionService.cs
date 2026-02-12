namespace QrSortable.Platforms.iOS.Components.PlatformUtils
{
    using AVFoundation;
    using Photos;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Models;
    using UserNotifications;

    /// <summary>
    /// The iOS specific implementation of the permission service.
    /// </summary>
    public class IosPermissionService : IPermissionService
    {

        /// <summary>
        ///     Requests the given permission.
        /// </summary>
        /// <param name="permission">The permission to request.</param>
        /// <returns>The permission status after request.</returns>
        public async Task<PermissionStatus> RequestPermissionAsync(Permission permission)
        {
            switch (permission)
            {
                case Permission.Camera:
                    return await RequestCameraPermissionAsync();
                case Permission.Notification:
                    return await RequestNotificationPermissionAsync();
                case Permission.Photos:
                    return await RequestPhotoPermissionAsync();
                default:
                    return PermissionStatus.Granted;
            }
        }

        /// <summary>
        ///     Checks the current status of the given permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The current status of the given permission.</returns>
        public async Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            switch (permission)
            {
                case Permission.Camera:
                    return await CheckCameraPermissionAsync();
                case Permission.Notification:
                    return await CheckNotificationPermissionAsync();
                case Permission.Photos:
                    return await CheckPhotoPermissionAsync();
                default:
                    return PermissionStatus.Granted;
            }
        }

        /// <summary>
        ///     Placeholder implementation for the interface.
        /// </summary>
        /// <returns> True. </returns>
        public bool CheckIfLocationIsEnabled()
        {
            return true;
        }

        private async Task<PermissionStatus> RequestCameraPermissionAsync()
        {
            try
            {
                var isAccessAuthorized = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Video);
                return isAccessAuthorized ? PermissionStatus.Granted : PermissionStatus.Denied;
            }
            catch (Exception ex)
            {
                return PermissionStatus.Unknown;
            }
        }

        private async Task<PermissionStatus> CheckCameraPermissionAsync()
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video);
            return status switch
            {
                AVAuthorizationStatus.Authorized => PermissionStatus.Granted,
                AVAuthorizationStatus.Denied => PermissionStatus.Denied,
                AVAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                AVAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                _ => PermissionStatus.Unknown
            };
        }

        private async Task<PermissionStatus> RequestNotificationPermissionAsync()
        {
            try
            {
                UNAuthorizationOptions options = UNAuthorizationOptions.Alert |
                                                 UNAuthorizationOptions.Sound;
                if (DeviceInfo.Version.Major >= 15)
                {
                    options |= UNAuthorizationOptions.TimeSensitive;
                }

                var isAccessAuthorized = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(options);
                return isAccessAuthorized.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
            }
            catch (Exception ex)
            {
                return PermissionStatus.Unknown;
            }

        }

        private async Task<PermissionStatus> CheckNotificationPermissionAsync()
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            return settings.AuthorizationStatus switch
            {
                UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
                UNAuthorizationStatus.Denied => PermissionStatus.Denied,
                UNAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                _ => PermissionStatus.Unknown
            };
        }

        private async Task<PermissionStatus> RequestPhotoPermissionAsync()
        {
            var status = await PHPhotoLibrary.RequestAuthorizationAsync();

            return status switch
            {
                PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
                PHAuthorizationStatus.Limited => PermissionStatus.Granted,
                PHAuthorizationStatus.Denied => PermissionStatus.Denied,
                PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                PHAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                _ => PermissionStatus.Unknown
            };
        }

        private Task<PermissionStatus> CheckPhotoPermissionAsync()
        {
            var status = PHPhotoLibrary.AuthorizationStatus;

            return Task.FromResult(status switch
            {
                PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
                PHAuthorizationStatus.Limited => PermissionStatus.Granted,
                PHAuthorizationStatus.Denied => PermissionStatus.Denied,
                PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                PHAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                _ => PermissionStatus.Unknown
            });
        }

    }
}