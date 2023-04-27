using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppRelink.Converters;

[ValueConversion(typeof(string), typeof(Visibility))]
public class NonEmptyStringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value as string)) return Visibility.Collapsed;
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null!;
    }
}