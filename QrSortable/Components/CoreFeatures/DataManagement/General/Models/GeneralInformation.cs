namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using DataManagement.Models;
    using QrSortable.Components.CoreFeatures.Settings.Models;

    /// <summary>
    ///     The model of how the general information will be stored in the database.
    /// </summary>
    public class GeneralInformation : DatabaseEntry
    {

        /// <summary>
        ///     Gets or sets the status of the notification permissions.
        /// </summary>
        public PermissionStatus NotificationPermissionStatus { get; set; }
        
        /// <summary>
        ///     Gets or sets the current onboarding progress.
        /// </summary>
        public OnboardingProgress OnboardingProgress { get; set; }

        public string MultiUserId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets whether the backend is being used.
        /// </summary>
        public bool IsBackendUsed { get; set; } = false;

        /// <summary>
        ///     Gets or sets the LanguageItem for the currently selected language.
        /// </summary>
        public string SelectedLanguageCode { get; set; } = "en";

    }
}
