namespace QrSortable.Components.CoreFeatures.Cloud.AccessManagement
{
    using System;
    using System.IO;
    using Google.Cloud.Firestore;
    using Google.Cloud.Firestore.V1;

    public static class FirestoreDbFactory
    {
        public static async Task<FirestoreDb> CreateAsync(string projectId)
        {
            try
            {
                // Load the JSON file from the MAUI app package
                using var stream = await FileSystem.OpenAppPackageFileAsync("firebase-storage-adminsdk.json");
                using var reader = new StreamReader(stream);
                string jsonCredentials = await reader.ReadToEndAsync();

                // Use the FirestoreDbBuilder to initialize with the JSON string
                var builder = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    JsonCredentials = jsonCredentials
                };

                return await builder.BuildAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Auth Error: {ex.Message}");
                throw;
            }
        }
    }
}