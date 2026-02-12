namespace QrSortable.Components.CoreFeatures.Scanner
{
    using Microsoft.Maui.Storage;
    public class FilePickerService :IFilePickerService
    {
        public async Task<Stream> ImageAsync()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Image Please",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) 
                return null;

            return await result.OpenReadAsync();
        }
    }
}
