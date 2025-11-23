namespace QrSortable.Components.CoreFeatures.CodeGenerator.Models
{
    using PdfSharpCore.Fonts;
    using System.Reflection;

    public class PdfFontResolver : IFontResolver
    {
        // Required property
        public string DefaultFontName => "OpenSans";

        public byte[] GetFont(string faceName)
        {
            // Match the faceName used in XFont
            if (faceName == "OpenSans")
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "QrSortable.Resources.Fonts.OpenSans-Regular.ttf";

                using var stream = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new FileNotFoundException($"Font resource '{resourceName}' not found.");

                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }

            throw new InvalidOperationException($"Font '{faceName}' not found");
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Map any requested style to your available font
            if (familyName == "OpenSans")
                return new FontResolverInfo("OpenSans");

            // Use default font if not found
            return new FontResolverInfo(DefaultFontName);
        }
    }
}
