using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace GameTemplate.UI.Converters;

public class IgnoringNullConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value ?? BindingOperations.DoNothing;
}
