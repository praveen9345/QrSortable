namespace QrSortable.Components.UiFunctionality.Navigation.Converter
{
    using System.Globalization;
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                try
                {
                    return Color.FromArgb(hex);
                }
                catch
                {
                    return Colors.White; // fallback if invalid hex
                }
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
