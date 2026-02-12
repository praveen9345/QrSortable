namespace QrSortable.Components.CoreFeatures.Scanner
{
    using Microsoft.Maui.Storage;
    public class FilePickerService :IFilePickerService
    {

        public async Task<Stream> ImageAsync()
        {
            try
            {
                // Pick a photo from gallery
                var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick Image Please"
                });

                if (photo == null)
                    return null;

                return await photo.OpenReadAsync();
            }
            catch (FeatureNotSupportedException)
            {
                // Device does not support picking
                return null;
            }
            catch (PermissionException)
            {
                // Permissions not granted
                return null;
            }
            catch (Exception)
            {
                // Other errors
                return null;
            }
        }
    }
}
