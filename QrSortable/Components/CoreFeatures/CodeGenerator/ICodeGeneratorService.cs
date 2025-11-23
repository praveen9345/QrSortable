namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    public interface ICodeGeneratorService
    {
        Task<List<ImageSource>> GenerateQrCodesAsync(string tag = "", int noOfPage = 1, string hexColor = "#000000");

        Task<List<ImageSource>> GenerateBarcodesAsync(string tag = "", int noOfPage = 1);
    }
}
