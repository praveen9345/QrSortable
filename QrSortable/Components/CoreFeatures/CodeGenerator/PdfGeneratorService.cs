namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Pdf;

    public class PdfGeneratorService : IPdfGeneratorService
    {
        // ── Label sheet physical dimensions (mm) ─────────────────────────────
        private const double PageWidthMm = 210.0;
        private const double PageHeightMm = 297.0;
        private const double CellWidthMm = 70.0;   // 3 × 70 = 210 → no side margin
        private const double CellHeightMm = 67.7;
        private const double TopMarginMm = 13.1;   // top white strip

        // ── Inner layout (mm) ────────────────────────────────────────────────
        private const double HorizPadMm = 2.0;    // left/right padding inside cell
        private const double VertPadMm = 2.0;    // top/bottom padding inside cell
        private const double LabelHeightMm = 5.5;    // space reserved for label text
        private const double LabelGapMm = 1.5;    // gap between label and QR/barcode
        private const double LabelFontPt = 9.0;

        // ── Helpers ──────────────────────────────────────────────────────────
        private static double Mm(double mm) => XUnit.FromMillimeter(mm).Point;

        // ────────────────────────────────────────────────────────────────────
        //  QR CODE PDF
        // ────────────────────────────────────────────────────────────────────
        public async Task<byte[]> GenerateQrPdfAsync(List<ImageSource> qrcodes)
        {
            return await Task.Run(() =>
            {
                using var document = new PdfDocument();

                const int perPage = 12;
                const int cols = 3;

                // Cell size in points
                double cellW = Mm(CellWidthMm);
                double cellH = Mm(CellHeightMm);
                double topM = Mm(TopMarginMm);

                // Padding & label in points
                double hPad = Mm(HorizPadMm);
                double vPad = Mm(VertPadMm);
                double labelH = Mm(LabelHeightMm);
                double labelGap = Mm(LabelGapMm);

                // Maximum square QR that fits inside the cell
                // Vertical space: vPad | labelH | labelGap | [QR] | vPad
                double maxW = cellW - 2 * hPad;
                double maxH = cellH - vPad - labelH - labelGap - vPad;
                double qrSize = Math.Min(maxW, maxH);   // square

                var labelFont = new XFont("OpenSans", LabelFontPt, XFontStyle.Bold);

                for (int i = 0; i < qrcodes.Count; i += perPage)
                {
                    var page = document.AddPage();
                    page.Width = Mm(PageWidthMm);
                    page.Height = Mm(PageHeightMm);
                    using var gfx = XGraphics.FromPdfPage(page);

                    for (int j = 0; j < perPage && i + j < qrcodes.Count; j++)
                    {
                        int col = j % cols;
                        int row = j / cols;

                        // ── Cell origin (top-left corner) ─────────────────────
                        double cellX = col * cellW;          // no left margin
                        double cellY = topM + row * cellH;

                        // ── Label baseline ────────────────────────────────────
                        double labelX = cellX + hPad;
                        double labelY = cellY + vPad + labelH; // DrawString uses baseline

                        // ── QR: centered horizontally in cell ─────────────────
                        double qrX = cellX + (cellW - qrSize) / 2.0;
                        double qrY = cellY + vPad + labelH + labelGap;

                        using var stream = ((StreamImageSource)qrcodes[i + j])
                            .Stream(CancellationToken.None).Result;
                        using var img = XImage.FromStream(() => stream);

                        gfx.DrawString(
                            "",   // ← your label text
                            labelFont, XBrushes.Black,
                            new XPoint(labelX, labelY));

                        gfx.DrawImage(img, qrX, qrY, qrSize, qrSize);
                    }
                }

                using var ms = new MemoryStream();
                document.Save(ms);
                return ms.ToArray();
            });
        }

        // ────────────────────────────────────────────────────────────────────
        //  BARCODE PDF
        // ────────────────────────────────────────────────────────────────────
        public async Task<byte[]> GenerateBarcodePdfAsync(List<ImageSource> barcodes)
        {
            return await Task.Run(() =>
            {
                using var document = new PdfDocument();

                const int perPage = 12;
                const int cols = 3;

                double cellW = Mm(CellWidthMm);
                double cellH = Mm(CellHeightMm);
                double topM = Mm(TopMarginMm);
                double hPad = Mm(HorizPadMm);
                double vPad = Mm(VertPadMm);
                double labelH = Mm(LabelHeightMm);
                double labelGap = Mm(LabelGapMm);

                // Barcode fills full available width; height = 60 % of available height
                double availW = cellW - 2 * hPad;
                double availH = cellH - vPad - labelH - labelGap - vPad;
                double bcW = availW;
                double bcH = availH * 0.60;

                var labelFont = new XFont("OpenSans", LabelFontPt, XFontStyle.Bold);

                for (int i = 0; i < barcodes.Count; i += perPage)
                {
                    var page = document.AddPage();
                    page.Width = Mm(PageWidthMm);
                    page.Height = Mm(PageHeightMm);
                    using var gfx = XGraphics.FromPdfPage(page);

                    for (int j = 0; j < perPage && i + j < barcodes.Count; j++)
                    {
                        int col = j % cols;
                        int row = j / cols;

                        double cellX = col * cellW;
                        double cellY = topM + row * cellH;

                        double labelX = cellX + hPad;
                        double labelY = cellY + vPad + labelH;

                        // Barcode: centered horizontally; vertically centered in availH
                        double bcX = cellX + (cellW - bcW) / 2.0;
                        double bcY = cellY + vPad + labelH + labelGap
                                          + (availH - bcH) / 2.0;

                        using var stream = ((StreamImageSource)barcodes[i + j])
                            .Stream(CancellationToken.None).Result;
                        using var img = XImage.FromStream(() => stream);

                        gfx.DrawString(
                            "",   // ← your label text
                            labelFont, XBrushes.Black,
                            new XPoint(labelX, labelY));

                        gfx.DrawImage(img, bcX, bcY, bcW, bcH);
                    }
                }

                using var ms = new MemoryStream();
                document.Save(ms);
                return ms.ToArray();
            });
        }
    }
}