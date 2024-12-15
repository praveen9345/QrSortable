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
        ///     Updates the user's current OnboardingProgress.
        /// </summary>
        /// <param name="progress"> The user's current OnboardingProgress. </param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        Task<bool> UpdateOnboardingProgressAsync(OnboardingProgress progress);

    }
}
