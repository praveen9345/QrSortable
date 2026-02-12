namespace QrSortable.Components.PlatformUtils.Models
{
    /// <summary>
    ///     An enum representing the different permissions needed for this app.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        ///     The unknown permission only used as return type, never requested
        /// </summary>
        Unknown,

        /// <summary>
        ///     The needed permission for using the device's camera.
        /// </summary>
        Camera,

        /// <summary>
        ///     The needed permission for sending local notifications (iOS only).
        /// </summary>
        Notification,

        Photos
    }
}