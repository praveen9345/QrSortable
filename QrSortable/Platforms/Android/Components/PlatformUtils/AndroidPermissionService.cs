namespace QrSortable.Platforms.Android.Components.PlatformUtils
{
    using System.Threading.Tasks;
    using AndroidX.Core.App;
    using AndroidX.Core.Content;
    using global::Android;
    using global::Android.OS;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Components.PlatformUtils.Models;
    using QrSortable.Components.PlatformUtils.Wrappers;
    using Permission = QrSortable.Components.PlatformUtils.Models.Permission;

    /// <summary>
    ///     The Android specific implementation of the permission service.
    /// </summary>
    public class AndroidPermissionService : IPermissionService
    {
        private const int PermissionsCode = 99;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        private TaskCompletionSource<Dictionary<Permission, PermissionStatus>> _taskCompletionSource;

        /// <summary>
        ///     Initialize a new instance of the <see cref="AndroidPermissionService" /> class.
        /// </summary>
        /// <param name="mauiEssentialsWrapper"> The wrapper for the maui specific functionality. </param>
        public AndroidPermissionService(IMauiEssentialsWrapper mauiEssentialsWrapper)
        {
            _mauiEssentialsWrapper = mauiEssentialsWrapper;
        }

        /// <summary>
        ///     Requests the given permission.
        /// </summary>
        /// <param name="permission">The permission to request.</param>
        /// <returns>The permission status after request.</returns>
        public async Task<PermissionStatus> RequestPermissionAsync(Permission permission)
        {
            // Already granted? Return immediately.
            var currentStatus = await CheckPermissionStatusAsync(permission);
            if (currentStatus == PermissionStatus.Granted)
                return PermissionStatus.Granted;

            // If a permission request is already running, reuse it
            if (_taskCompletionSource != null && !_taskCompletionSource.Task.IsCompleted)
            {
                var runningResult = await _taskCompletionSource.Task;
                return runningResult.ContainsKey(permission)
                    ? runningResult[permission]
                    : PermissionStatus.Unknown;
            }

            var permissionNames = GetManifestNames(permission);
            if (permissionNames == null)
                return PermissionStatus.Unknown;

            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
                return PermissionStatus.Unknown;

            // Create new task source (NO cancel anymore)
            _taskCompletionSource =
                new TaskCompletionSource<Dictionary<Permission, PermissionStatus>>();

            _mauiEssentialsWrapper.RunOnMainThread(() =>
            {
                ActivityCompat.RequestPermissions(
                    currentActivity,
                    permissionNames.ToArray(),
                    PermissionsCode);
            });

            var result = await _taskCompletionSource.Task;

            return result.ContainsKey(permission)
                ? result[permission]
                : PermissionStatus.Unknown;
        }

        /// <summary>
        ///     Checks the current status of the given permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The current status of the given permission.</returns>
        public Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            var context = Platform.CurrentActivity?.Application?.ApplicationContext;

            if (permission == Permission.Notification)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                    return Task.FromResult(PermissionStatus.Granted);

                var enabled = AndroidX.Core.App.NotificationManagerCompat
                    .From(context)
                    .AreNotificationsEnabled();

                return Task.FromResult(enabled
                    ? PermissionStatus.Granted
                    : PermissionStatus.Denied);
            }

            var permissionNames = GetManifestNames(permission);
            if (permissionNames == null || context == null)
                return Task.FromResult(PermissionStatus.Unknown);

            if (permissionNames.Any(name =>
                ContextCompat.CheckSelfPermission(context, name)
                != global::Android.Content.PM.Permission.Granted))
            {
                return Task.FromResult(PermissionStatus.Denied);
            }

            return Task.FromResult(PermissionStatus.Granted);
        }

        /// <summary>
        ///     Callback that is executed when the permission request is finished.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="permissions">The permissions to request.</param>
        /// <param name="grantResults">The grant results.</param>
        public void OnRequestPermissionsResult(int requestCode, string[] permissions,
            global::Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode != PermissionsCode || _taskCompletionSource == null)
                return;

            var results = new Dictionary<Permission, PermissionStatus>();

            for (var i = 0; i < permissions.Length; i++)
            {
                var permission = GetPermissionForManifestName(permissions[i]);
                if (permission == Permission.Unknown)
                    continue;

                var resultStatus =
                    grantResults[i] == global::Android.Content.PM.Permission.Granted
                        ? PermissionStatus.Granted
                        : PermissionStatus.Denied;

                results[permission] = resultStatus;
            }

            _taskCompletionSource.TrySetResult(results);
        }

        /// <summary>
        ///     Checks if the location services are enabled.
        /// </summary>
        /// <returns> True, if the location services are enabled, false otherwise. </returns>
        public bool CheckIfLocationIsEnabled()
        {
            var manager =
              (global::Android.Locations.LocationManager)
              global::Android.App.Application.Context
                  .GetSystemService(global::Android.Content.Context.LocationService);

            return manager?.IsProviderEnabled(
                global::Android.Locations.LocationManager.GpsProvider) ?? false;
        }

        private Permission GetPermissionForManifestName(string permissionName)
        {
            switch (permissionName)
            {
                case Manifest.Permission.Camera:
                    return Permission.Camera;

                case Manifest.Permission.PostNotifications:
                    return Permission.Notification;

                case Manifest.Permission.ReadExternalStorage:
                case Manifest.Permission.ReadMediaImages:
                    return Permission.Photos;

                default:
                    return Permission.Unknown;
            }
        }

        private List<string> GetManifestNames(Permission permission)
        {
            var permissionsNames = new List<string>();

            switch (permission)
            {
                case Permission.Camera:
                    permissionsNames.Add(Manifest.Permission.Camera);
                    break;

                case Permission.Notification:
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                        permissionsNames.Add(Manifest.Permission.PostNotifications);
                    break;

                case Permission.Photos:
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                        permissionsNames.Add(Manifest.Permission.ReadMediaImages);
                    else
                        permissionsNames.Add(Manifest.Permission.ReadExternalStorage);
                    break;

                default:
                    return null;
            }

            return permissionsNames;
        }
    }
}
