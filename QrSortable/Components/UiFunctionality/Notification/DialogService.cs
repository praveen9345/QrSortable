namespace QrSortable.Components.UiFunctionality.Notification
{
    using System;
    using System.Threading.Tasks;
    using Views;
    using Navigation;
    using QrSortable.Core.Components.UiFunctionality.Notification.Models;
    using QrSortable.Components.UiFunctionality.Notification.Models;

    /// <summary>
    ///     Implements the IDialogService interface.
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly INavigationService _navigationService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DialogService" />.
        /// </summary>
        /// <param name="navigationService">The navigation service for opening dialogs.</param>
        public DialogService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        /// <summary>
        ///     Navigates to the viewmodel belonging to the dialog of the type Alert.
        ///     This is a dialog which contains a text and one button.
        /// </summary>
        /// <param name="text"> The text of the dialog. </param>
        /// <param name="buttonText"> The text of the button of the dialog. </param>
        /// <returns>A task to await the user interaction with the dialog. </returns>
        public Task ShowAlertDialog(string text, string buttonText)
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogAlertView, DialogAlertData, bool>(
                new DialogAlertData("", text, buttonText));
        }

        /// <summary>
        ///     Navigates to the viewmodel belonging to the dialog of the type Alert.
        ///     This is a dialog which contains a title, text and one button.
        /// </summary>
        /// <param name="title"> The title of the dialog. </param>
        /// <param name="text"> The text of the dialog. </param>
        /// <param name="buttonText"> The text of the button of the dialog. </param>
        /// <returns>A task to await the user interaction with the dialog. </returns>
        public Task ShowAlertDialog(string title, string text, string buttonText)
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogAlertView, DialogAlertData, bool>(
                new DialogAlertData(title, text, buttonText));
        }

        /// <summary>
        ///     Navigates to the viewmodel belonging to the dialog of the type Request.
        ///     This dialog contains a text and two buttons.
        /// </summary>
        /// <param name="text"> The text of the dialog. </param>
        /// <param name="cancelButtonText"> The text of the cancel button of the dialog. </param>
        /// <param name="confirmButtonText"> The text of the confirm of the dialog. </param>
        /// <returns>
        ///     A task to await the user interaction with the dialog. Awaiting it returns true if the confirm button is pressed
        ///     and false if the cancel button was pressed.
        /// </returns>
        public Task<bool> ShowRequestDialog(string text, string cancelButtonText, string confirmButtonText)
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogRequestView, DialogRequestData, bool>(
                new DialogRequestData("", text, cancelButtonText, confirmButtonText));
        }

        /// <summary>
        ///     Navigates to the viewmodel belonging to the dialog of the type Request.
        ///     That dialog contains a title, text and two buttons.
        /// </summary>
        /// <param name="title"> The title of the dialog. </param>
        /// <param name="text"> The text of the dialog. </param>
        /// <param name="cancelButtonText"> The text of the cancel button of the dialog. </param>
        /// <param name="confirmButtonText"> The text of the confirm of the dialog. </param>
        /// <returns>
        ///     A task to await the user interaction with the dialog. Awaiting it returns true if the confirm button is pressed
        ///     and false if the cancel button was pressed.
        /// </returns>
        public Task<bool> ShowRequestDialog(string title, string text, string cancelButtonText,
            string confirmButtonText)
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogRequestView, DialogRequestData, bool>(
                new DialogRequestData(title, text, cancelButtonText, confirmButtonText));
        }

        /// <summary>
        ///     Navigates to the viewmodel belonging to the dialog displaying an activity indicator. Once the viewmodel is
        ///     initialized, the completion of the task contained in the function given over as parameter is awaited. Once that
        ///     task is successfully completed, the viewmodel is closed and the result of the awaited function is returned.
        /// </summary>
        /// <param name="text"> The text of the dialog.</param>
        /// <param name="awaitableFunction"> The function which contains the task whose completion shall be awaited.</param>
        /// <returns>An awaitable task which signals the outcome of the task.</returns>
        public Task<object> ShowActivityIndicatorAndReturnResult(string text, Func<Task<object>> awaitableFunction)
        {
            var data = new DialogGenericActivityIndicatorData(text, awaitableFunction);
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogGenericActivityIndicatorView, DialogGenericActivityIndicatorData, object>(data);
        }

        /// <summary>
        /// Navigates to the viewmodel belonging to the selecting photo dialog.
        /// </summary>
        /// <returns>
        ///     Awaitable task of the navigation. Awaiting it returns the chosen response.
        /// </returns>
        public Task<PhotoSelectionResponse> ShowPhotoSelectionDialog()
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogPhotoSelectionView, PhotoSelectionResponse>();
        }

        public Task<string> ShowMoveToDialog(object objectData)
        {
            return _navigationService.OpenDialogAndAwaitResultAsync<DialogMoveToView, object, string>(objectData);
        }
    }
}