namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateQrPdfAsync(List<ImageSource> qrcodes);

        Task<byte[]> GenerateBarcodePdfAsync(List<ImageSource> barcodes);
    }
}
