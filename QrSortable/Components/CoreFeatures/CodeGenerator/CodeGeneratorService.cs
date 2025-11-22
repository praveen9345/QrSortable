namespace QrSortable.Components.CoreFeatures.CodeGenerator
{
    using QRCoder;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Helper;
    using SkiaSharp;
    using System.IO;
    using System.Runtime.InteropServices;
    using ZXing;
    using ZXing.Common;

    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IAesHelper _aesHelper;

        public CodeGeneratorService(IAesHelper aesHelper)
        {
            _aesHelper = aesHelper;
        }

        public ImageSource GenerateQrCode(string input, string hexColor = "#000000")
        {
            int size = 300;

            string encrypted = _aesHelper.Encrypt(input);
            SKColor darkColor;
            try
            {
                darkColor = SKColor.Parse(hexColor);
            }
            catch
            {
                darkColor = SKColors.Black;
            }

            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(encrypted, QRCodeGenerator.ECCLevel.Q);

            int moduleCount = qrData.ModuleMatrix.Count;
            float moduleSize = (float)size / moduleCount;

            using var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint { Color = darkColor, Style = SKPaintStyle.Fill };

            for (int y = 0; y < moduleCount; y++)
            {
                for (int x = 0; x < moduleCount; x++)
                {
                    if (qrData.ModuleMatrix[y][x])
                    {
                        canvas.DrawRect(x * moduleSize, y * moduleSize, moduleSize, moduleSize, paint);
                    }
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var ms = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
            ms.Position = 0;

            // Return a factory to create a fresh stream to avoid ObjectDisposedException
            return ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
        }

        public ImageSource GenerateBarcode(string hexColor = "#000000")
        {

            string plainText = "ABC123XYZ789";

            string encryptedText = _aesHelper.Encrypt(plainText);

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 600,
                    Height = 300,
                    Margin = 20,
                    PureBarcode = false // Show human-readable text
                }
            };

            var pixelData = writer.Write(encryptedText);
            var handle = GCHandle.Alloc(pixelData.Pixels, GCHandleType.Pinned);


            try
            {

                // Create SKBitmap and install pixels from ZXing
                var bitmap = new SKBitmap(pixelData.Width, pixelData.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
                bitmap.InstallPixels(bitmap.Info, handle.AddrOfPinnedObject(), pixelData.Width * 4);

                using var image = SKImage.FromBitmap(bitmap);
                var ms = new MemoryStream();
                image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
                ms.Position = 0;

                return ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
            }
            finally
            {
                handle.Free();
            }

        }
    }
    
}
