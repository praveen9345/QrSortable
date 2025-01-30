namespace QrSortable.Components.UiFunctionality.Notification
{
    using CommunityToolkit.Maui.Alerts;
    using CommunityToolkit.Maui.Core;

    /// <summary>
    ///     Interface of the service providing toast functionality.
    /// </summary>
    public class ToastService: IToastService
    {
        /// <summary>
        ///     Displays a toast notification with the specified text.
        /// </summary>
        /// <param name="text">The text to display in the toast notification.</param>
        public async Task DisplayToast(string text)
        {
            CancellationTokenSource cancel = new CancellationTokenSource();
            var toast = Toast.Make(text, ToastDuration.Short, 18);
            await toast.Show(cancel.Token);
        }
    }
}