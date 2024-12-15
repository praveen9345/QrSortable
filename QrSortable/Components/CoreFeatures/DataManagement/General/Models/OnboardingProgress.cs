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
        SignUp,

        /// <summary>
        ///     Means that the user signing in with the log in.
        /// </summary>
        LogIn,

        /// <summary>
        ///     Means that the user providing user profile in with the child profile.
        /// </summary>
        UserProfile,

        /// <summary>
        ///     Means that the onboarding was completed completely.
        /// </summary>
        OnboardingCompleted
    }
}
