namespace QrSortable.Components.UiFunctionality.Notification.Models
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     A model storing all the data needed to display a dialog of the type activity indicator.
    /// </summary>
    public class DialogGenericActivityIndicatorData
    {
        /// <summary>
        ///     Gets the text of the dialog.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets the function of the dialog.
        /// </summary>
        public Func<Task<object>> AwaitableFunction { get; }

        /// <summary>
        ///     Initializes a new instance of <see cref="DialogGenericActivityIndicatorData" />.
        /// </summary>
        /// <param name="text"> The string which shall be displayed as the text of the dialog. </param>
        /// <param name="awaitableFunction"> The function which is awaited while the dialog is displayed. </param>
        public DialogGenericActivityIndicatorData(string text, Func<Task<object>> awaitableFunction)
        {
            Text = text;
            AwaitableFunction = awaitableFunction;
        }
    }
}