namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Pdf;

    public class PdfGeneratorService : IPdfGeneratorService
    {

        public async Task<byte[]> GenerateQrPdfAsync(List<ImageSource> qrcodes)
        {

            return await Task.Run(() =>
            {
                using var document = new PdfDocument();

                int qrPerPage = 12; // 3x4 grid
                int columns = 3;
                int rows = 4;
                double qrWidth = 150;
                double qrHeight = 150;
                double margin = 20;

                for (int i = 0; i < qrcodes.Count; i += qrPerPage)
                {
                    var page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(210); // A4 width
                    page.Height = XUnit.FromMillimeter(297); // A4 height
                    var gfx = XGraphics.FromPdfPage(page);

                    for (int j = 0; j < qrPerPage && i + j < qrcodes.Count; j++)
                    {
                        int col = j % columns;
                        int row = j / columns;

                        double x = margin + col * (qrWidth + margin);
                        double y = margin + row * (qrHeight + 40); // extra space for label

                        using var stream = ((StreamImageSource)qrcodes[i + j]).
                        Stream(CancellationToken.None).Result;

                        using var img = XImage.FromStream(() => stream);

                        gfx.DrawImage(img, x, y, qrWidth, qrHeight);

                        // Draw label under QR
                        gfx.DrawString("", new XFont("OpenSans", 12, XFontStyle.Bold),
                            XBrushes.Black, new XPoint(x, y + qrHeight + 15));
                    }
                }

                using var ms = new MemoryStream();
                document.Save(ms);
                return ms.ToArray();
            });
        }

        public async Task<byte[]> GenerateBarcodePdfAsync(List<ImageSource> barcodes)
        {
            return await Task.Run(() =>
            {
                using var document = new PdfDocument();

                int barcodesPerPage = 12; // 3x4 grid
                int columns = 3;
                int rows = 4;
                double barcodeWidth = 180;  // Slightly wider than QR
                double barcodeHeight = 80;  // Barcodes are shorter
                double margin = 20;

                for (int i = 0; i < barcodes.Count; i += barcodesPerPage)
                {
                    var page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(210); // A4 width
                    page.Height = XUnit.FromMillimeter(297); // A4 height
                    var gfx = XGraphics.FromPdfPage(page);

                    for (int j = 0; j < barcodesPerPage && i + j < barcodes.Count; j++)
                    {
                        int col = j % columns;
                        int row = j / columns;

                        double x = margin + col * (barcodeWidth + margin);
                        double y = margin + row * (barcodeHeight + 40); // Extra space for label

                        using var stream = ((StreamImageSource)barcodes[i + j]).
                        Stream(CancellationToken.None).Result;

                        using var img = XImage.FromStream(() => stream);

                        // Draw barcode image
                        gfx.DrawImage(img, x, y, barcodeWidth, barcodeHeight);

                        // Optional: Draw label under barcode (if needed)
                        gfx.DrawString("", new XFont("OpenSans", 12, XFontStyle.Bold),
                            XBrushes.Black, new XPoint(x, y + barcodeHeight + 15));
                    }
                }

                using var ms = new MemoryStream();
                document.Save(ms);
                return ms.ToArray();
            });
        }

    }
}
