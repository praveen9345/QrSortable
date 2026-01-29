namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.Configuration;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text.Json;

    public class FirebaseStorageService : IFirebaseStorageService, IDisposable
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IGeneralInformationManager _generalInformationManager;
        private readonly HttpClient _client;
        private bool _disposed;

        private const string Error = "error";
        private const string TokenKey = "firebase_id_token";

        public FirebaseStorageService(
            IFirebaseAuthService firebaseAuthService,
            IGeneralInformationManager generalInformationManager)
        {
            _firebaseAuthService = firebaseAuthService;
            _generalInformationManager = generalInformationManager;

            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // -------------------- UPLOAD --------------------

        public async Task<string> UploadAsync(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return Error;

            try
            {
                var generalInfo = await _generalInformationManager.GetGeneralInformationAsync();
                var folderName = generalInfo?.MultiUserId ?? "default";
                var fileName = $"{folderName}/{Guid.NewGuid()}.jpg";

                var uploadUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o" +
                    $"?uploadType=media&name={Uri.EscapeDataString(fileName)}";

                var response = await SendAuthorizedAsync(
                    HttpMethod.Post,
                    uploadUrl,
                    imageBytes,
                    "image/jpeg");

                if (!response.IsSuccessStatusCode)
                    return Error;

                // Public download URL
                return
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/" +
                    $"{Uri.EscapeDataString(fileName)}?alt=media";
            }
            catch
            {
                return Error;
            }
        }

        public async Task<IList<string>> UploadImagesAsync(IList<byte[]> imageList)
        {
            if (imageList == null || imageList.Count == 0)
                return new List<string>();

            var tasks = imageList.Select(UploadAsync);
            var results = await Task.WhenAll(tasks);

            return results.Where(r => r != Error).ToList();
        }

        // -------------------- DELETE --------------------

        public async Task<bool> DeleteAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            try
            {
                var uri = new Uri(imageUrl);

                // Extract object path after `/o/`
                var objectName = Uri.UnescapeDataString(
                    uri.AbsolutePath.Split("/o/")[1]
                );

                var deleteUrl =
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/{Uri.EscapeDataString(objectName)}";

                var response = await SendAuthorizedAsync(HttpMethod.Delete, deleteUrl);

                // 404 = already deleted → treat as success
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return true;

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteImagesAsync(IList<string> imageUrls)
        {
            if (imageUrls == null || imageUrls.Count == 0)
                return false;

            var tasks = imageUrls.Select(DeleteAsync);
            var results = await Task.WhenAll(tasks);

            return results.All(r => r);
        }

        // -------------------- DOWNLOAD --------------------
        // ⚠️ Public URLs should NOT be authorized

        public async Task<byte[]> DownloadImageAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Array.Empty<byte>();

            try
            {
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return Array.Empty<byte>();

                return await response.Content.ReadAsByteArrayAsync();
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

            return results.Where(r => r.Length > 0).ToList();
        }

        // -------------------- AUTH CORE --------------------

        private async Task<HttpResponseMessage> SendAuthorizedAsync(
            HttpMethod method,
            string url,
            byte[]? contentBytes = null,
            string? contentType = null)
        {
            var token = await GetIdTokenAsync();

            var request = CreateRequest(method, url, token, contentBytes, contentType);
            var response = await _client.SendAsync(request);

            // Token invalid → refresh once
            if (response.StatusCode == HttpStatusCode.Forbidden ||
                response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Remove(TokenKey);

                token = await GetIdTokenAsync(forceRefresh: true);
                request = CreateRequest(method, url, token, contentBytes, contentType);

                response = await _client.SendAsync(request);
            }

            return response;
        }

        private static HttpRequestMessage CreateRequest(
            HttpMethod method,
            string url,
            string token,
            byte[]? contentBytes,
            string? contentType)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            if (contentBytes != null)
            {
                request.Content = new ByteArrayContent(contentBytes);
                request.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(contentType);
            }

            return request;
        }

        private async Task<string> GetIdTokenAsync(bool forceRefresh = false)
        {
            try
            {
                if (forceRefresh)
                    SecureStorage.Remove(TokenKey);

                var token = await SecureStorage.GetAsync(TokenKey);

                if (string.IsNullOrEmpty(token))
                {
                    token = await _firebaseAuthService.SignInAnonymouslyAsync();
                    if (!string.IsNullOrEmpty(token))
                        await SecureStorage.SetAsync(TokenKey, token);
                }

                return token ?? string.Empty;
            }
            catch
            {
                SecureStorage.Remove(TokenKey);
                return string.Empty;
            }
        }

        // -------------------- DISPOSE --------------------

        public void Dispose()
        {
            if (_disposed)
                return;

            _client.Dispose();
            _disposed = true;
        }
    }
}