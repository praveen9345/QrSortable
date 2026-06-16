namespace QrSortable.Components.CoreFeatures.Assistant
{

    using CommunityToolkit.Maui.Media;
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    public class StorageVoiceAssistantService : IStorageVoiceAssistantService
    {

        private readonly IStorageFinderService _storageFinderService;
        private CancellationTokenSource _searchCts;

        public StorageVoiceAssistantService(IStorageFinderService storageFinderService)
        {
            _storageFinderService = storageFinderService;
        }

        public async Task<List<StorageEntry>> SpeakMatchesAsync(double minConfidence = 0.2)
        {
            //TODO: pre-select the language
            _searchCts?.Cancel();

            var spokenText = await RecognizeSpeechAsync();

            if (string.IsNullOrWhiteSpace(spokenText))
                return new List<StorageEntry>();

            _searchCts = new CancellationTokenSource();

            return await _storageFinderService.FindGenericAsync(
                spokenText,
                _searchCts.Token
            );
        }


        // --- Speech Recognition ---
        private async Task<string?> RecognizeSpeechAsync()
        {
            var speechToText = SpeechToText.Default;

            var permissionGranted = await speechToText.RequestPermissions(CancellationToken.None);
            if (!permissionGranted)
                return null;

            string recognizedText = string.Empty;

            var options = new SpeechToTextOptions
            {
                Culture = CultureInfo.CurrentCulture
            };

            var tcs = new TaskCompletionSource<string?>();

            speechToText.RecognitionResultCompleted += OnCompleted;

            async void OnCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
            {

                recognizedText = e.RecognitionResult?.Text ?? string.Empty;

                tcs.TrySetResult(recognizedText);

                speechToText.RecognitionResultCompleted -= OnCompleted;

                await speechToText.StopListenAsync();
            }

            await speechToText.StartListenAsync(options, CancellationToken.None);

            return await tcs.Task;
        }


    }
}
