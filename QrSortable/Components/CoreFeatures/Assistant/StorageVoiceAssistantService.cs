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
            _searchCts?.Dispose();

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
            // Request microphone permission
            var permissionGranted = await SpeechToText.Default.RequestPermissions(CancellationToken.None);
            if (!permissionGranted) return null;

            string recognizedText = string.Empty;

            // Listen and capture result
            var result = await SpeechToText.Default.ListenAsync(
                CultureInfo.CurrentCulture,
                new Progress<string>(_ => { }), // optional partial updates
                CancellationToken.None
            );

            if (result.IsSuccessful)
                recognizedText = result.Text;

            return recognizedText;
        }

    }
}
