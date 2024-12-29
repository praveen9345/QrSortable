namespace QrSortable.Components.PlatformUtils
{
    using Microsoft.Maui.Graphics.Platform;

    /// <summary>
    /// ..................................
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platformImage">..........</param>
        /// <returns>........................</returns>
        Task<byte[]> PlatformImageConvertAsync(PlatformImage platformImage);

        Task<byte[]> ConvertToJpegBytes(Stream inputStream);
    }
}
