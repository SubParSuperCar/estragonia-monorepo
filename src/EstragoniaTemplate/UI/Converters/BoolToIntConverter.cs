using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Template.UI.Converters;

public class BoolToIntConverter : IValueConverter
{
    public static readonly BoolToIntConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue) return booleanValue ? 1 : 0;

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int num) return System.Convert.ToBoolean(num);

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}
