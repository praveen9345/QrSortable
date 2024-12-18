namespace QrSortable.Core.Components.UiFunctionality.Notification.Models
{
    /// <summary>
    ///     A model storing all the data needed to display a dialog of the type alert.
    /// </summary>
    public class DialogAlertData
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
        ///     Gets the text of the button.
        /// </summary>
        public string ButtonText { get; }

        /// <summary>
        ///     Initializes a new instance of <see cref="DialogAlertData" />.
        /// </summary>
        /// <param name="title"> The string which shall be displayed as the title of the dialog. </param>
        /// <param name="text"> The string which shall be displayed as the text of the dialog. </param>
        /// <param name="buttonText"> The string which shall be displayed as the text of the button. </param>
        public DialogAlertData(string title, string text, string buttonText)
        {
            Title = title;
            Text = text;
            ButtonText = buttonText;
        }
    }
}