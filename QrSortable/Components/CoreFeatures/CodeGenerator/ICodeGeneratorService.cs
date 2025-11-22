namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    public interface ICodeGeneratorService
    {
        ImageSource GenerateQrCode(string input, string hexColor = "#000000");

        ImageSource GenerateBarcode(string hexColor = "#000000");
    }
}
