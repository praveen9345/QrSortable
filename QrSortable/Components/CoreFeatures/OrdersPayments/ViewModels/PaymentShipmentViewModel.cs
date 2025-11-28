namespace QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using QrSortable.Components.CoreFeatures.CodeGenerator.Models;
    using QrSortable.Components.CoreFeatures.OrdersPayments.Views;
    using QrSortable.Components.UiFunctionality.Navigation.ViewModels;

    /// <summary>
    ///     The view model of the Select Product view screen.
    /// </summary>
    public partial class PaymentShipmentViewModel : BaseViewModel<Product>
    {
        private Product _product;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentShipmentViewModel" />.
        /// </summary>
        public PaymentShipmentViewModel()
        {
            IsBackNavigationEnabled = true;
           
        }

        /// <summary>
        /// Initializes the component asynchronously, ensuring proper initialization of general information
        /// and notification permissions.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            
        }

        public override void Prepare(Product parameter)
        {
            _product = parameter;
            TotalAmount = _product.TotalPrice.ToString() + "€";
            if (_product.Title == "Generate A4 QR or bar code yourself!") 
            { 
                BankTransferVisible = true; 
            }
            else { BankTransferVisible = true; }
        }

        /// <summary>
        /// Represents the currently total amount to pay in the application.
        /// </summary>
        [ObservableProperty]
        private string _totalAmount;

        /// <summary>
        /// Represents the visisble of the bank transfer button in the application.
        /// </summary>
        [ObservableProperty]
        private bool _bankTransferVisible = true;

        /// <summary>
        /// Represents the currently name in the application.
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Represents the currently street name in the application.
        /// </summary>
        [ObservableProperty]
        private string _streetName;

        /// <summary>
        /// Represents the currently house number in the application.
        /// </summary>
        [ObservableProperty]
        private string _houseNumber;

        /// <summary>
        /// Represents the currently zip code in the application.
        /// </summary>
        [ObservableProperty]
        private string _zipCode;

        /// <summary>
        /// Represents the currently city name in the application.
        /// </summary>
        [ObservableProperty]
        private string _cityName;

        /// <summary>
        /// Represents the currently Country Name in the application.
        /// </summary>
        [ObservableProperty]
        private string _countryName;

        /// <summary>
        /// Represents the currently Email in the application.
        /// </summary>
        [ObservableProperty]
        private string _email;

        public AsyncRelayCommand PaymentByBankCommand => new AsyncRelayCommand(async () =>
        {

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(HouseNumber) ||
                string.IsNullOrWhiteSpace(ZipCode) ||
                string.IsNullOrWhiteSpace(CityName) ||
                string.IsNullOrWhiteSpace(CountryName) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowAlertDialog("Missing Information",
                    "Please fill in all fields before proceeding.", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await DialogService.ShowAlertDialog("Invalid Email",
                    "Please enter a valid email address.", "OK");
                return;
            }

            // Add info to product
            AddInformationToProducts();

            // Navigate to BankTransferView
            await NavigationService.Navigate<BankTransferView>(_product);
        });

        private void AddInformationToProducts()
        {
            _product.Name = Name;
            _product.Street = StreetName;
            _product.HouseNo = HouseNumber;
            _product.ZipCode = ZipCode;
            _product.City = CityName;
            _product.Country = CountryName;
            _product.Email = Email;
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}