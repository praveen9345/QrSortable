using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
using QrSortable.Components.CoreFeatures.Scanner.Models;

namespace QrSortable.Components.CoreFeatures.Scanner
{
    public interface ISuggestionService
    {
        /// <summary>Builds the index from the DB once (safe to call many times / fire-and-forget).</summary>
        Task EnsureBuiltAsync();

        /// <summary>
        /// Returns up to 6 ranked suggestions (already in the current UI language for seeded terms).
        /// <paramref name="context"/> = values of the OTHER fields that are already filled in.
        /// </summary>
        IReadOnlyList<string> Suggest(SuggestionField target, string? typed,
            params (SuggestionField Field, string? Value)[] context);

        /// <summary>O(1) incremental update after a successful save/move.</summary>
        void RecordSaved(StorageEntry entry);
    }

}
