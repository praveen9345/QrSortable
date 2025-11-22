namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the qr code or bar code view.
    /// </summary>
    public partial class CodeGeneratorView : BaseView
    {

        private readonly ICodeGeneratorService _codeService;

        /// <summary>
        ///  Initializes a new instance of the CodeGeneratorViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The CodeGeneratorViewModel associated with this view.</param>
        public CodeGeneratorView(CodeGeneratorViewModel viewModel, ICodeGeneratorService codeService) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _codeService = codeService;
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
                ResultImage.Source = _codeService.GenerateQrCode(code, colorHex);
            }
            else
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ResultImage.Source = _codeService.GenerateBarcode();
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