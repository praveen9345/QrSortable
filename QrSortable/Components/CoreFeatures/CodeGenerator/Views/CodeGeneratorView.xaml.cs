namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using Microsoft.Maui.Controls;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.IO;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class CodeGeneratorView : BaseView
    {

        private readonly ICodeGeneratorService _codeService;
        private readonly IPdfGeneratorService _pdfService;

        /// <summary>
        ///  Initializes a new instance of the CodeGeneratorViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The CodeGeneratorViewModel associated with this view.</param>
        public CodeGeneratorView(CodeGeneratorViewModel viewModel, ICodeGeneratorService codeService, IPdfGeneratorService pdfGeneratorService) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            _codeService = codeService;
            _pdfService = pdfGeneratorService;

            TypePicker.SelectedIndex = 0; // default QR Code
        }

        private async void OnGenerateClicked(object sender, EventArgs e)
        {
            string code = CodeEntry.Text?.Trim();
            if (string.IsNullOrEmpty(code))
            {
                DisplayAlert("Error", "Please enter a 6-digit code", "OK");
                return;
            }

            if (TypePicker.SelectedIndex == -1)
            {
                DisplayAlert("Error", "Please select QR or Barcode", "OK");
                return;
            }

            ResultImage.IsVisible = true;
            ResultLabel.IsVisible = true;

            string selectedType = TypePicker.SelectedItem.ToString();
            ResultLabel.Text = selectedType;

            if (selectedType == "QR Code")
            {
                string colorHex = ColorEntry.Text?.Trim() ?? "#000000";

                List<ImageSource> qrCodesCustom =  await _codeService.GenerateQrCodesAsync(tag:"Kitchen", 1 ,hexColor:colorHex);
                ResultImage.Source = qrCodesCustom.FirstOrDefault();


                var pdfBytes = await _pdfService.GenerateQrPdfAsync(qrCodesCustom);

                string filePath = Path.Combine(FileSystem.AppDataDirectory, "QrCodes.pdf");
                File.WriteAllBytes(filePath, pdfBytes);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Your QR Code PDF",
                    File = new ShareFile(filePath)
                });

            }
            else
            {
                try
                {
                    string colorHex = ColorEntry.Text?.Trim() ?? "#000000";

                    // Generate multiple barcodes similar to QR codes
                    List<ImageSource> barcodesCustom = await _codeService.GenerateBarcodesAsync(tag: "Kitchen", 1);
                    ResultImage.Source = barcodesCustom.FirstOrDefault();

                    // Create PDF from barcodes
                    var pdfBytes = await _pdfService.GenerateBarcodePdfAsync(barcodesCustom);
                    string filePath = Path.Combine(FileSystem.AppDataDirectory, "Barcodes.pdf");
                    File.WriteAllBytes(filePath, pdfBytes);

                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Your Barcode PDF",
                        File = new ShareFile(filePath)
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "OK");
                }
            }
        }
      

    }
}