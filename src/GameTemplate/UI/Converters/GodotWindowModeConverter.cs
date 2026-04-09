using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Godot;

namespace GameTemplate.UI.Converters;

public class GodotWindowModeConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not DisplayServer.WindowMode windowMode)
			return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
		return windowMode switch
		{
			DisplayServer.WindowMode.ExclusiveFullscreen => 0,
			DisplayServer.WindowMode.Fullscreen => 1,
			DisplayServer.WindowMode.Windowed => 2,
			_ => new BindingNotification(new InvalidCastException(), BindingErrorType.Error)
		};
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not int index) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
		return index switch
		{
			0 => DisplayServer.WindowMode.ExclusiveFullscreen,
			1 => DisplayServer.WindowMode.Fullscreen,
			2 => DisplayServer.WindowMode.Windowed,
			_ => new BindingNotification(new InvalidCastException(), BindingErrorType.Error)
		};
	}
}
