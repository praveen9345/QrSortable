namespace QrSortable.Components.CoreFeatures.Scanner
{
    using Microsoft.Maui.Storage;
    using QrSortable.Components.UiFunctionality.Localization;

    public class FilePickerService :IFilePickerService
    {
        public async Task<Stream> ImageAsync()
        {
            try
            {
                FileResult result = null;

                // Use MediaPicker for iOS, FilePicker for others
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // MediaPicker works better on iOS
                    result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = AppResources.FilePickerService_PickImage
                    });
                }
                else
                {
                    // FilePicker for Android and other platforms
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = AppResources.FilePickerService_PickImage,
                        FileTypes = FilePickerFileType.Images
                    });
                }

                if (result == null)
                {
                    Console.WriteLine("FilePickerService: User cancelled");
                    return null;
                }

                Console.WriteLine($"FilePickerService: Picked file: {result.FileName}");

                // Open and copy to MemoryStream
                using var sourceStream = await result.OpenReadAsync();
                if (sourceStream == null)
                {
                    Console.WriteLine("FilePickerService: OpenReadAsync returned null");
                    return null;
                }

                var memoryStream = new MemoryStream();
                await sourceStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                Console.WriteLine($"FilePickerService: Stream size: {memoryStream.Length} bytes");
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FilePickerService Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
