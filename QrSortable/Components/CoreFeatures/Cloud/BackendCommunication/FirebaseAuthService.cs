namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using QrSortable.Components.Configuration;
    using System.Text;
    using System.Text.Json;

    public class FirebaseAuthService: IFirebaseAuthService
    {
        private readonly HttpClient _http = new();

        public async Task<string> SignInAnonymouslyAsync()
        {
            var url =
                $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseConfig.ApiKey}";

            var response = await _http.PostAsync(
                url,
                new StringContent("{}", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("idToken").GetString();
        }
    }
}
