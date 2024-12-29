namespace QrSortable.Platforms.iOS.Components.PlatformUtils
{
    using System.Threading.Tasks;
    using Foundation;
    using Microsoft.Maui.Graphics.Platform;
    using QrSortable.Components.PlatformUtils;
    using UIKit;

    public class IosImageService : IImageService
    {
        public async Task<byte[]> PlatformImageConvertAsync(PlatformImage platformImage)
        {
            try
            {
                var imageStream = platformImage.ToPlatformImage().AsStream();
                var imageData = NSData.FromStream(imageStream);
                var uiImage = new UIImage(imageData);
                var jpegImage = uiImage.AsJPEG(0.5f);
                return jpegImage.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IosImageService: Error processing image: {ex.Message}");
                return await Task.FromResult<byte[]>(null);
            }
        }

        /// <summary>
        /// ...............................
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public async Task<byte[]> ConvertToJpegBytes(Stream inputStream)
        {
            try
            {
                var imageData = NSData.FromStream(inputStream);
                var uiImage = new UIImage(imageData);
                var jpegImage = uiImage.AsJPEG(0.5f);
                return jpegImage.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IosImageService: ConvertToJpegBytes :Error processing image: {ex.Message}");
                return await Task.FromResult<byte[]>(null);
            }
        }
    }
}
