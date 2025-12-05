namespace QrSortable.Components.CoreFeatures.OrdersPayments.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Globalization;

    public partial class BasketData : ObservableObject
    {
        [ObservableProperty] 
        private string _title;

        [ObservableProperty] 
        private string _description;

        [ObservableProperty] 
        private string _price;

        [ObservableProperty] 
        private int _productQuantity;

        [ObservableProperty] 
        private DateTime _dateTime;

        [ObservableProperty] 
        private decimal _totalPrice;

        partial void OnPriceChanged(string value) => RecalculateTotal();
        partial void OnProductQuantityChanged(int value) => RecalculateTotal();

        private void RecalculateTotal()
        {
            var unitPrice = ParsePrice(_price);
            TotalPrice = unitPrice * ProductQuantity;
        }

        private static decimal ParsePrice(string priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText)) return 0m;
            var cleaned = new string(priceText.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out var value))
                return value;
            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return value;

            return 0m;
        }

    }
}
