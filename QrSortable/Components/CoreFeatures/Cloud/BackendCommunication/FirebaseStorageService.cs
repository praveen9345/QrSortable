namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.Configuration;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using System.Net.Http.Headers;

    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IGeneralInformationManager _generalInformationManager;

        private readonly HttpClient _client = new();

        public FirebaseStorageService(IFirebaseAuthService firebaseAuthService, IGeneralInformationManager generalInformationManager) 
        {
            _firebaseAuthService = firebaseAuthService;
            _generalInformationManager = generalInformationManager;
        }


        public async Task<string> UploadAsync(byte[] imageBytes)
        {
            try { 
                var folderName = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
                
                var fileName = $"{folderName}/{Guid.NewGuid()}.jpg";

                var url =
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o" +
                    $"?uploadType=media&name={Uri.EscapeDataString(fileName)}";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", await GetIdTokenAsync());

                request.Content = new ByteArrayContent(imageBytes);
                request.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/" +
                    $"{Uri.EscapeDataString(fileName)}?alt=media";
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image to Firebase Storage.", ex);
            }
           
        }

        public async Task<IList<string>> UploadImagesAsync(IList<byte[]> imageList)
            {
                var uploadTasks = imageList
                    .Select(image => UploadAsync(image))
                    .ToList();

                var urls = await Task.WhenAll(uploadTasks);
                return urls.ToList();
        }

        public async Task DeleteAsync(string imageUrl)
        {
            try
            {
                // Extract the file path from the URL
                var baseUrl = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/";
                if (!imageUrl.StartsWith(baseUrl))
                    throw new Exception("Invalid Firebase Storage URL.");

                // Get the encoded path
                var encodedPath = imageUrl.Substring(baseUrl.Length);
                var questionMarkIndex = encodedPath.IndexOf("?");
                if (questionMarkIndex >= 0)
                    encodedPath = encodedPath.Substring(0, questionMarkIndex);

                var url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/{encodedPath}";

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", await GetIdTokenAsync());

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete image: {imageUrl}", ex);
            }
        }

        public async Task DeleteImagesAsync(IList<string> imageUrls)
        {
            var deleteTasks = imageUrls.Select(url => DeleteAsync(url)).ToList();
            await Task.WhenAll(deleteTasks);
        }

        public async Task<byte[]> DownloadImageAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetByteArrayAsync(url);
        }

        public async Task<IList<byte[]>> DownloadImagesAsync(List<string> urls)
        {
            if (urls == null || urls.Count == 0)
                return new List<byte[]>();

            // Download images in parallel
            var downloadTasks = urls.Select(DownloadImageAsync);
            var results = await Task.WhenAll(downloadTasks);
            return results.ToList();
        }

        private async Task<string> GetIdTokenAsync()
        {
            var token = await SecureStorage.GetAsync("firebase_id_token");
            if (string.IsNullOrEmpty(token))
            {
                token = await _firebaseAuthService.SignInAnonymouslyAsync();
                await SecureStorage.SetAsync("firebase_id_token", token);
            }
            return token;
        }
    }
}
