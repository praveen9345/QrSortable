namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateQrPdfA4Async(List<ImageSource> qrcodes);
        Task<byte[]> GenerateBarcodePdfA4Async(List<ImageSource> barcodes);
        Task<byte[]> GenerateQrPdfA5Async(List<ImageSource> qrcodes);
        Task<byte[]> GenerateBarcodePdfA5Async(List<ImageSource> barcodes);
    }
}
