namespace QrSortable.Components.CoreFeatures.Scanner
{
    public interface IFilePickerService
    {
        Task<Stream> ImageAsync();
    }
}
