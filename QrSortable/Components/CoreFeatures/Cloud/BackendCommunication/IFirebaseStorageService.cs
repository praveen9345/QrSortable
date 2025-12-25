namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadAsync(byte[] imageBytes);

        Task<IList<string>> UploadImagesAsync(IList<byte[]> imageList);

        Task DeleteAsync(string imageUrl);

        Task DeleteImagesAsync(IList<string> imageUrls);

        Task<byte[]> DownloadImageAsync(string url);

        Task<IList<byte[]>> DownloadImagesAsync(List<string> urls);
    }
}
