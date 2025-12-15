namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using System;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// Provides logic for handling the general information entity.
    /// </summary>
    public interface IGeneralInformationManager
    {
        /// <summary>
        /// Gets the current general information entity.
        /// </summary>
        /// <returns>The current general information entity.</returns>
        Task<GeneralInformation> GetGeneralInformationAsync();

        /// <summary>
        ///     Resets the general information saved in this class.
        /// </summary>
        void ResetGeneralInformation();

        /// <summary>
        ///     Updates the status of the notification permission with a value.
        /// </summary>
        /// <param name="permissionStatus">The value indicating the current status of the permission.</param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        Task<bool> UpdateNotificationPermissionStatusAsync(PermissionStatus permissionStatus);
        
            /// <summary>
        ///     Updates the user's current OnboardingProgress.
        /// </summary>
        /// <param name="progress"> The user's current OnboardingProgress. </param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        Task<bool> UpdateOnboardingProgressAsync(OnboardingProgress progress);

        /// <summary>
        /// Asynchronously determines whether a multiuser identifier is available for the current context.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a multiuser
        /// identifier is available; otherwise, <see langword="false"/>.</returns>
        Task<bool> GenerateTheMultiuserIdAsync();

    }
}
