namespace QrSortable.Components.UiFunctionality.Notification
{
    /// <summary>
    ///     Interface of the service providing toast functionality.
    /// </summary>
    public interface IToastService
    {
        /// <summary>
        ///     Displays a toast notification with the specified text.
        /// </summary>
        /// <param name="text">The text to display in the toast notification.</param>
        Task DisplayToast(string text);
    }
}