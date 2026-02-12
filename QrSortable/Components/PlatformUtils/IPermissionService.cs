namespace QrSortable.Components.PlatformUtils
{
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    ///     The permission service interface providing the possibility to request permissions and check their status.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        ///     Requests the given permission.
        /// </summary>
        /// <param name="permission">The permission to request.</param>
        /// <returns>The permission status after request.</returns>
        Task<PermissionStatus> RequestPermissionAsync(Permission permission);

        /// <summary>
        ///     Checks the current status of the given permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The current status of the given permission.</returns>
        Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission);

        /// <summary>
        ///     Checks if the location services are enabled. (This is only properly implemented in Android. On iOS, there is placeholder method)
        /// </summary>
        /// <returns> True, if the location services are enabled, false otherwise. </returns>
        bool CheckIfLocationIsEnabled();
    }
}