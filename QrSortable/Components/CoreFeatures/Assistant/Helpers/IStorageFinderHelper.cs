namespace QrSortable.Components.CoreFeatures.Assistant.Helpers
{
    public interface IStorageFinderHelper
    {
        string Normalize(string text);

        double CalculateSimilarity(string source, string target);
    }
}
