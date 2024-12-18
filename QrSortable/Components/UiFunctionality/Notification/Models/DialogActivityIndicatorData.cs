namespace QrSortable.Core.Components.UiFunctionality.Notification.Models
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     A model storing all the data needed to display a dialog of the type activity indicator which navigates on
    ///     successful function completion.
    /// </summary>
    public class DialogActivityIndicatorData
    {
        /// <summary>
        ///     Gets the text of the dialog.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets the function of the dialog.
        /// </summary>
        public Func<Task<bool>> AwaitableFunction { get; }

        /// <summary>
        ///     Gets the task which navigates once the AwaitableFunction is finished.
        /// </summary>
        public Func<Task> NavigationFunction { get; }

        /// <summary>
        ///     Initializes a new instance of <see cref="DialogActivityIndicatorData" />.
        /// </summary>
        /// <param name="text"> The string which shall be displayed as the text of the dialog. </param>
        /// <param name="awaitableFunction"> The function which is awaited while the dialog is displayed. </param>
        /// <param name="navigationFunction"> The function containing the navigation task. </param>
        public DialogActivityIndicatorData(string text, Func<Task<bool>> awaitableFunction, Func<Task> navigationFunction)
        {
            Text = text;
            AwaitableFunction = awaitableFunction;
            NavigationFunction = navigationFunction;
        }
    }
}
