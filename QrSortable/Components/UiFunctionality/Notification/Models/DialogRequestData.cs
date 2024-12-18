namespace QrSortable.Components.UiFunctionality.Notification.Models
{
    /// <summary>
    ///     A model storing all the data needed to display a dialog of the type request.
    /// </summary>
    public class DialogRequestData
    {
        /// <summary>
        ///     Gets the title of the dialog.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Gets the text of the dialog.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets the text of the left button.
        /// </summary>
        public string CancelButtonText { get; }

        /// <summary>
        ///     Gets the text of the right button.
        /// </summary>
        public string ConfirmButtonText { get; }

        /// <summary>
        ///     Initializes a new instance of <see cref="DialogRequestData" />.
        /// </summary>
        /// <param name="title"> The string which shall be displayed as the title of the dialog. </param>
        /// <param name="text"> The string which shall be displayed as the text of the dialog. </param>
        /// <param name="cancelButtonText"> The string which shall be displayed as the text of the cancel button. </param>
        /// <param name="confirmButtonText"> The string which shall be displayed as the text of the confirm button. </param>
        public DialogRequestData(string title, string text, string cancelButtonText, string confirmButtonText)
        {
            Title = title;
            Text = text;
            CancelButtonText = cancelButtonText;
            ConfirmButtonText = confirmButtonText;
        }
    }
}