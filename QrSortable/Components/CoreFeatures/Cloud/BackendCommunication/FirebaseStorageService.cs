namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.Configuration;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using System.Net;
    using System.Net.Http.Headers;

    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IGeneralInformationManager _generalInformationManager;

        private readonly HttpClient _client = new();

        // Retry configuration
        private const int MaxRetryAttempts = 3;
        private const int BaseDelayMilliseconds = 500;

        public FirebaseStorageService(IFirebaseAuthService firebaseAuthService, IGeneralInformationManager generalInformationManager)
        {
            _firebaseAuthService = firebaseAuthService;
            _generalInformationManager = generalInformationManager;
        }

        public async Task<string> UploadAsync(byte[] imageBytes)
        {
            try
            {
                var folderName = (await _generalInformationManager.GetGeneralInformationAsync()).MultiUserId;
                var fileName = $"{folderName}/{Guid.NewGuid()}.jpg";

                var endpointBase =
                    $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o";
                var url =
                    $"{endpointBase}?uploadType=media&name={Uri.EscapeDataString(fileName)}";

                for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, url);

                        var token = await GetIdTokenAsync();
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Create fresh content for each attempt
                        request.Content = new ByteArrayContent(imageBytes);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                        // Ignore if file does not exist (unexpected for upload, but preserve existing behavior)
                        if (response.StatusCode == HttpStatusCode.NotFound)
                            return string.Empty;

                        if (response.IsSuccessStatusCode)
                        {
                            return
                                $"{endpointBase}/" +
                                $"{Uri.EscapeDataString(fileName)}?alt=media";
                        }

                        // If unauthorized, refresh token once and retry immediately
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            await SecureStorage.SetAsync("firebase_id_token", string.Empty);
                            // log and continue to next attempt which will request new token
                            Console.WriteLine($"Upload attempt {attempt} unauthorized; refreshing token and retrying.");
                        }
                        else
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Upload attempt {attempt} failed ({response.StatusCode}): {body}");
                        }

                        // Decide whether to retry based on status code
                        if (attempt == MaxRetryAttempts || !IsTransientStatusCode(response.StatusCode))
                            return string.Empty;
                    }
                    catch (HttpRequestException hre)
                    {
                        Console.WriteLine($"Upload attempt {attempt} encountered network error: {hre.Message}");
                        if (attempt == MaxRetryAttempts)
                            return string.Empty;
                    }
                    catch (TaskCanceledException tce)
                    {
                        Console.WriteLine($"Upload attempt {attempt} timed out or canceled: {tce.Message}");
                        if (attempt == MaxRetryAttempts)
                            return string.Empty;
                    }

                    // Backoff before next attempt
                    await DelayWithJitterAsync(attempt);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload image to Firebase Storage. {ex}");
                return string.Empty;
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
                    return;

                // Get the encoded path
                var encodedPath = imageUrl.Substring(baseUrl.Length);
                var questionMarkIndex = encodedPath.IndexOf("?");
                if (questionMarkIndex >= 0)
                    encodedPath = encodedPath.Substring(0, questionMarkIndex);

                var url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.BUCKET}/o/{encodedPath}";

                for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Delete, url);

                        var token = await GetIdTokenAsync();
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                        // Ignore if file does not exist
                        if (response.StatusCode == HttpStatusCode.NotFound)
                            return;

                        if (response.IsSuccessStatusCode)
                            return;

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            await SecureStorage.SetAsync("firebase_id_token", string.Empty);
                            Console.WriteLine($"Delete attempt {attempt} unauthorized; refreshing token and retrying.");
                        }
                        else
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Delete attempt {attempt} failed ({response.StatusCode}): {body}");
                        }

                        if (attempt == MaxRetryAttempts || !IsTransientStatusCode(response.StatusCode))
                            return;
                    }
                    catch (HttpRequestException hre)
                    {
                        Console.WriteLine($"Delete attempt {attempt} encountered network error: {hre.Message}");
                        if (attempt == MaxRetryAttempts)
                            return;
                    }
                    catch (TaskCanceledException tce)
                    {
                        Console.WriteLine($"Delete attempt {attempt} timed out or canceled: {tce.Message}");
                        if (attempt == MaxRetryAttempts)
                            return;
                    }

                    await DelayWithJitterAsync(attempt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete image: {imageUrl}. {ex}");
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

        private static bool IsTransientStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout || // 408
                   statusCode == (HttpStatusCode)429 || // Too Many Requests
                   statusCode == HttpStatusCode.InternalServerError || // 500
                   statusCode == HttpStatusCode.BadGateway || // 502
                   statusCode == HttpStatusCode.ServiceUnavailable || // 503
                   statusCode == HttpStatusCode.GatewayTimeout; // 504
        }

        private static Task DelayWithJitterAsync(int attempt)
        {
            // exponential backoff with jitter
            var rand = Random.Shared;
            var exponential = BaseDelayMilliseconds * Math.Pow(2, attempt - 1);
            var jitter = rand.Next(0, 250);
            var delay = TimeSpan.FromMilliseconds(exponential + jitter);
            return Task.Delay(delay);
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