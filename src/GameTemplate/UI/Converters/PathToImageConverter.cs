using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace GameTemplate.UI.Converters;

public class PathToImageConverter : IValueConverter
{
	private const string ImageFolderPath = "UI/Images";

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not string path)
			return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

		return Utilities.LoadImageFromResource(new Uri($"avares://GameTemplate/{ImageFolderPath}/{path}.png"));
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		BindingOperations.DoNothing;
}
