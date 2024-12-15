namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    /// <summary>
    ///     The model of how a user information entry is represented in the database.
    /// </summary>
    public class UserInfos : DatabaseEntry
    {
        /// <summary>
        ///     Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the Google email address of the user.
        /// </summary>
        public string GoogleEmail { get; set; }

        /// <summary>
        ///     Gets or sets the user's avatar image.
        /// </summary>
        public string UserAvatar { get; set; }

        /// <summary>
        ///     Gets or sets the username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the gender of the user.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        ///     Gets or sets whether the user loged in or not.
        /// </summary>
        public bool IsUserLogedIn { get; set; } = false;
    }
}