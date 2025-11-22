namespace QrSortable.Components.CoreFeatures.CodeGenerator.Helper
{
    public interface IAesHelper
    {
        string Encrypt(string plainText);

        string Decrypt(string cipherText);
    }
}
