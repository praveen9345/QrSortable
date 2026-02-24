namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    /// <summary>
    ///     Specifies the various states the onboarding progress can have.
    /// </summary>
    public enum OnboardingProgress
    {
        /// <summary>
        ///     Means that the onboarding process was not started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        ///     Means that the user signing up with the registration.
        /// </summary>
        OnboardingStarted,

        /// <summary>
        ///     Means that the onboarding was completed completely.
        /// </summary>
        OnboardingCompleted
    }
}
