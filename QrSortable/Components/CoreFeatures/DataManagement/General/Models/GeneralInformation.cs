namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using System;
    using DataManagement.Models;

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

    }
}
