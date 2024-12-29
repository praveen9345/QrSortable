namespace QrSortable.Platforms.Android.Components.PlatformUtils
{
    using global::Android.Graphics;
    using Microsoft.Maui.Graphics.Platform;
    using QrSortable.Components.PlatformUtils;
    
    /// <summary>
    /// ...........................................
    /// </summary>
    public class AndroidImageService : IImageService
    {
        /// <summary>
        /// .................................................
        /// </summary>
        /// <param name="platformImage"></param>
        /// <returns>.......................</returns>
        public async Task<byte[]> PlatformImageConvertAsync(PlatformImage platformImage)
        {
            try
            {
                var imageStream = platformImage.ToPlatformImage().AsStream();

                using (var memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    return ConvertToJpeg(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AndroidImageService: PlatformImageConvertAsync: Error processing image: {ex.Message}");
                return null;
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
                using (var memoryStream = new MemoryStream())
                {
                    await inputStream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    return ConvertToJpeg(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AndroidImageService: ConvertToJpegBytes :Error processing image: {ex.Message}");
                return null;
            }
        }

        private byte[] ConvertToJpeg(byte[] imageBytes)
        {
            using (var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length))
            {
                // Compress the Bitmap into a JPEG format and store it in a MemoryStream
                using (var jpegStream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, jpegStream); 
                    return jpegStream.ToArray();
                }
            }
        }
    }
}
