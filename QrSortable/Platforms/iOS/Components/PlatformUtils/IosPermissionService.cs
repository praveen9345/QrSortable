namespace QrSortable.Platforms.iOS.Components.PlatformUtils
{
    using AVFoundation;
    using Photos;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Models;
    using UserNotifications;
    using Microsoft.Maui.Devices;

    public class IosPermissionService : IPermissionService
    {
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

        public async Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            switch (permission)
            {
                case Permission.Camera:
                    return CheckCameraPermission();

                case Permission.Notification:
                    return await CheckNotificationPermissionAsync();

                case Permission.Photos:
                    return CheckPhotoPermission();

                default:
                    return PermissionStatus.Granted;
            }
        }

        public bool CheckIfLocationIsEnabled()
        {
            return true;
        }

        // =========================
        // CAMERA
        // =========================

        private async Task<PermissionStatus> RequestCameraPermissionAsync()
        {
            var currentStatus = CheckCameraPermission();

            if (currentStatus != PermissionStatus.Unknown)
                return currentStatus;

            try
            {
                var granted = await AVCaptureDevice
                    .RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Video);

                return granted
                    ? PermissionStatus.Granted
                    : PermissionStatus.Denied;
            }
            catch
            {
                return PermissionStatus.Unknown;
            }
        }

        private PermissionStatus CheckCameraPermission()
        {
            var status =
                AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video);

            return status switch
            {
                AVAuthorizationStatus.Authorized => PermissionStatus.Granted,
                AVAuthorizationStatus.Denied => PermissionStatus.Denied,
                AVAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                AVAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                _ => PermissionStatus.Unknown
            };
        }

        // =========================
        // NOTIFICATIONS
        // =========================

        private async Task<PermissionStatus> RequestNotificationPermissionAsync()
        {
            var current = await CheckNotificationPermissionAsync();
            if (current != PermissionStatus.Unknown)
                return current;

            try
            {
                UNAuthorizationOptions options =
                    UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Sound |
                    UNAuthorizationOptions.Badge;

                if (DeviceInfo.Version.Major >= 15)
                    options |= UNAuthorizationOptions.TimeSensitive;

                var result = await UNUserNotificationCenter
                    .Current
                    .RequestAuthorizationAsync(options);

                return result.Item1
                    ? PermissionStatus.Granted
                    : PermissionStatus.Denied;
            }
            catch
            {
                return PermissionStatus.Unknown;
            }
        }

        private async Task<PermissionStatus> CheckNotificationPermissionAsync()
        {
            var settings =
                await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();

            return settings.AuthorizationStatus switch
            {
                UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
                UNAuthorizationStatus.Denied => PermissionStatus.Denied,
                UNAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
                UNAuthorizationStatus.Provisional => PermissionStatus.Granted,
                _ => PermissionStatus.Unknown
            };
        }

        // =========================
        // PHOTOS
        // =========================

        private async Task<PermissionStatus> RequestPhotoPermissionAsync()
        {
            var current = CheckPhotoPermission();

            if (current != PermissionStatus.Unknown)
                return current;

            var status = await PHPhotoLibrary.RequestAuthorizationAsync();

            return ConvertPhotoStatus(status);
        }

        private PermissionStatus CheckPhotoPermission()
        {
            var status = PHPhotoLibrary.AuthorizationStatus;
            return ConvertPhotoStatus(status);
        }

        private PermissionStatus ConvertPhotoStatus(PHAuthorizationStatus status)
        {
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
    }
}