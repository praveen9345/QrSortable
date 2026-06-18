namespace QrSortable.Components.UiFunctionality.Navigation.Converter
{
    using System;
    using System.Globalization;
    using Microsoft.Maui.Controls;


    public class BoolInverterConverter : IValueConverter
    {
        // ViewModel → UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
                return !booleanValue;

            return value;
        }

        // UI → ViewModel (optional)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
                return !booleanValue;

            return value;
        }
    }
}
