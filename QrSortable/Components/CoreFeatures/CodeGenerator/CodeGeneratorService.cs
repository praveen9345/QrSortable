namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    using QRCoder;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using SkiaSharp;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using ZXing;
    using ZXing.Common;

    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IAesHelper _aesHelper;

        // Number of codes per page for A4 and A5
        private const int noOfCodesA4 = 12;
        private const int noOfCodesA5 = 6;

        public CodeGeneratorService(IAesHelper aesHelper)
        {
            _aesHelper = aesHelper;
        }

        public async Task<List<ImageSource>> GenerateQrCodesAsync(string tag = "", int noOfPage = 1, string hexColor = "#000000", string pageType = "A4")
        {
            return await Task.Run(() =>
            {
                var noOfCodes = pageType.Contains("A5") ? noOfCodesA5 : noOfCodesA4;
                var noOfQRPerPage = noOfPage * noOfCodes;
                var qrImages = new List<ImageSource>();

                int qrSize = 620;
                int textHeight = 100;
                int totalHeight = qrSize + (textHeight * 2);

                SKColor darkColor = SKColors.Black;
                if (!string.IsNullOrWhiteSpace(hexColor))
                {
                    try { darkColor = SKColor.Parse(hexColor); } catch { }
                }

                var random = new Random();

                // Fonts for top and bottom text
                var topFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 100);
                var bottomFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 80);

                using var qrGenerator = new QRCodeGenerator();

                for (int i = 0; i < noOfQRPerPage; i++)
                {
                    // 1. Generate random code
                    string input = GenerateRandomCode(random);

                    string inputColor = input+ hexColor;

                    // 2. Encrypt
                    string encrypted;
                    try { encrypted = _aesHelper.Encrypt(inputColor); }
                    catch { encrypted = inputColor; }

                    // 3. Create QR code data
                    var qrData = qrGenerator.CreateQrCode(encrypted, QRCodeGenerator.ECCLevel.Q);
                    int moduleCount = qrData.ModuleMatrix.Count;
                    float moduleSize = (float)qrSize / moduleCount;

                    using var bitmap = new SKBitmap(qrSize, totalHeight);
                    using var canvas = new SKCanvas(bitmap);
                    canvas.Clear(SKColors.White);

                    using var paint = new SKPaint { Color = darkColor, IsAntialias = true };

                    // Draw top text (random code)
                    float topTextWidth = topFont.MeasureText(input);
                    float topX = (qrSize - topTextWidth) / 2;
                    float topY = textHeight - 20;
                    canvas.DrawText(input, topX, topY, topFont, paint);

                    // Draw QR code
                    using var qrPaint = new SKPaint { Color = darkColor, Style = SKPaintStyle.Fill };
                    for (int y = 0; y < moduleCount; y++)
                    {
                        for (int x = 0; x < moduleCount; x++)
                        {
                            if (qrData.ModuleMatrix[y][x])
                            {
                                canvas.DrawRect(x * moduleSize, y * moduleSize + textHeight, moduleSize, moduleSize, qrPaint);
                            }
                        }
                    }

                    // Draw bottom text (tag)
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        float bottomTextWidth = bottomFont.MeasureText(tag);
                        float bottomX = (qrSize - bottomTextWidth) / 2;
                        float bottomY = qrSize + textHeight + 60;
                        canvas.DrawText(tag, bottomX, bottomY, bottomFont, paint);
                    }

                    using var image = SKImage.FromBitmap(bitmap);
                    using var ms = new MemoryStream();
                    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
                    ms.Position = 0;

                    qrImages.Add(ImageSource.FromStream(() => new MemoryStream(ms.ToArray())));
                }

                return qrImages;
            });
        }

        public async Task<List<ImageSource>> GenerateBarcodesAsync(string tag = "", int noOfPage = 1, string pageType = "A4")
        {
            return await Task.Run(() =>
            {
                var noOfCodes = pageType.Contains("A5") ? noOfCodesA5 : noOfCodesA4;
                int noOfBarcodesPerPage = noOfPage * noOfCodes;
                var barcodeImages = new List<ImageSource>();

                int barcodeWidth = 1400;
                int barcodeHeight = 300;
                int textHeight = 100;
                int totalHeight = barcodeHeight + (textHeight * 2);

                var random = new Random();

                var topFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 80);
                var bottomFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 65);

                for (int i = 0; i < noOfBarcodesPerPage; i++)
                {
                    string input = GenerateRandomCode(random);

                    string inputColor = input + "#000000";
                    string encrypted;
                    try { encrypted = _aesHelper.Encrypt(inputColor); }
                    catch { encrypted = inputColor; }

                    var writer = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new EncodingOptions
                        {
                            Width = barcodeWidth,
                            Height = barcodeHeight,
                            Margin = 20,
                            PureBarcode = true 
                        }
                    };

                    var pixelData = writer.Write(encrypted);

                    // ✅ ZXing outputs BGRA — use Bgra8888 to match
                    using var barcodeBitmap = new SKBitmap(
                        pixelData.Width, pixelData.Height,
                        SKColorType.Bgra8888, SKAlphaType.Opaque);

                    // ✅ Marshal.Copy is safer than GCHandle + InstallPixels
                    var bitmapPixels = barcodeBitmap.GetPixels();
                    Marshal.Copy(pixelData.Pixels, 0, bitmapPixels, pixelData.Pixels.Length);

                    using var bitmap = new SKBitmap(barcodeWidth, totalHeight);
                    using var canvas = new SKCanvas(bitmap);
                    canvas.Clear(SKColors.White);

                    using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };

                    // Top text — plain human-readable code
                    float topTextWidth = topFont.MeasureText(input);
                    canvas.DrawText(
                        input,
                        (barcodeWidth - topTextWidth) / 2f,
                        textHeight - 20,
                        topFont, paint);

                    // Barcode — contains the encrypted value
                    canvas.DrawBitmap(barcodeBitmap, new SKPoint(0, textHeight));

                    // Bottom tag
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        float bottomTextWidth = bottomFont.MeasureText(tag);
                        canvas.DrawText(
                            tag,
                            (barcodeWidth - bottomTextWidth) / 2f,
                            barcodeHeight + textHeight + 70,
                            bottomFont, paint);
                    }

                    using var image = SKImage.FromBitmap(bitmap);
                    using var ms = new MemoryStream();
                    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
                    ms.Position = 0;

                    barcodeImages.Add(ImageSource.FromStream(() => new MemoryStream(ms.ToArray())));
                }

                return barcodeImages;
            });
        }

        // Helper to generate random code
        private string GenerateRandomCode(Random random)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";

            var sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
                sb.Append(letters[random.Next(letters.Length)]);
            for (int i = 0; i < 3; i++)
                sb.Append(digits[random.Next(digits.Length)]);

            return sb.ToString();
        }
    }
    
}
