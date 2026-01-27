namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.Configuration;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using System.Net;
    using System.Net.Http.Headers;

    public class FirebaseStorageService : IFirebaseStorageService, IDisposable
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly HttpClient _client;
        private bool _disposed;

        public FirebaseStorageService(
            IFirebaseAuthService firebaseAuthService,
            IGeneralInformationManager generalInformationManager)
        {
            _firebaseAuthService = firebaseAuthService;
            _generalInformationManager = generalInformationManager;
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string> UploadAsync(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return string.Empty;

            try
            {
                var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
                var folderName = generalInfo?.MultiUserId ?? "default";
                var fileName = $"{folderName}/{Guid.NewGuid()}.jpg";

                var url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o" +
                         $"?uploadType=media&name={Uri.EscapeDataString(fileName)}";

                var token = await GetIdTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return string.Empty;

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = new ByteArrayContent(imageBytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                return $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/" +
                       $"{Uri.EscapeDataString(fileName)}?alt=media";
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<IList<string>> UploadImagesAsync(IList<byte[]> imageList)
        {
            if (imageList == null || imageList.Count == 0)
                return new List<string>();

            var tasks = imageList.Select(image => UploadAsync(image));
            var results = await Task.WhenAll(tasks);
            return results.Where(url => !string.IsNullOrEmpty(url)).ToList();
        }

        public async Task DeleteAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                // Extract file path from URL
                var baseUrl = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/";
                if (!imageUrl.StartsWith(baseUrl))
                    return;

                var encodedPath = imageUrl.Substring(baseUrl.Length);
                var questionMarkIndex = encodedPath.IndexOf("?");
                if (questionMarkIndex >= 0)
                    encodedPath = encodedPath.Substring(0, questionMarkIndex);

                var url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/{encodedPath}";
                var token = await GetIdTokenAsync();

                if (string.IsNullOrEmpty(token))
                    return;

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                await _client.SendAsync(request);
            }
            catch
            {
                // Ignore delete errors
            }
        }

        public async Task DeleteImagesAsync(IList<string> imageUrls)
        {
            if (imageUrls == null || imageUrls.Count == 0)
                return;

            var tasks = imageUrls.Select(url => DeleteAsync(url));
            await Task.WhenAll(tasks);
        }

        public async Task<byte[]> DownloadImageAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                return Array.Empty<byte>();

            try
            {
                return await _client.GetByteArrayAsync(url);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<IList<byte[]>> DownloadImagesAsync(List<string> urls)
        {
            if (urls == null || urls.Count == 0)
                return new List<byte[]>();

            var tasks = urls.Select(DownloadImageAsync);
            var results = await Task.WhenAll(tasks);
            return results.Where(img => img.Length > 0).ToList();
        }

        private async Task<string> GetIdTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("firebase_id_token");
                if (string.IsNullOrEmpty(token))
                {
                    token = await _firebaseAuthService.SignInAnonymouslyAsync();
                    if (!string.IsNullOrEmpty(token))
                    {
                        await SecureStorage.SetAsync("firebase_id_token", token);
                    }
                }
                return token ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _client?.Dispose();
                _disposed = true;
            }
        }
    }
}