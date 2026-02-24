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
      /// ..................
      /// </summary>
      /// <param name="multiuserId">..........................</param>
      /// <returns></returns>
        Task<bool> UpdateTheMultiuserIdAsync(string multiuserId);

        /// <summary>
        ///     Updates whether the backend is used or not.
        /// </summary>
        /// <param name="isBackendUsed">The value indicating whether the backend is used or not.</param>
        /// <returns> True, if the update was successful. False, otherwise. </returns>
        Task<bool> UpdateIsBackendUsedAsync(bool isBackendUsed);

        /// <summary>
        ///    Sets the LanguageItem when the user selects a language.
        /// </summary>
        /// <param name="languageCode"> New unique string representing the language.</param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        Task<bool> SetLanguageAsync(string languageCode);

    }
}
