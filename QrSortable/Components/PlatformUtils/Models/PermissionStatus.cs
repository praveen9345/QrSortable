namespace QrSortable.Components.PlatformUtils.Models
{
    /// <summary>
    ///     An enum representing the status of a permission.
    /// </summary>
    public enum PermissionStatus
    {
        /// <summary>
        ///     Represents that the permission is denied by the user.
        /// </summary>
        Denied,

        /// <summary>
        ///     Represents that the corresponding feature is disabled on the device.
        /// </summary>
        Disabled,

        /// <summary>
        ///     Represents that the permission is granted by the user.
        /// </summary>
        Granted,

        /// <summary>
        ///     Represents that the permission is restricted (only iOS).
        /// </summary>
        Restricted,

        /// <summary>
        ///     Represents that the permission is in an unknown state.
        /// </summary>
        Unknown
    }
}