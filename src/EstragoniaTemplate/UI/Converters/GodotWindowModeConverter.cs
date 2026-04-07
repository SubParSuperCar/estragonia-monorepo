using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Godot;

namespace Template.UI.Converters;

public class GodotWindowModeConverter : IValueConverter
{
    public static readonly GodotWindowModeConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DisplayServer.WindowMode windowMode)
            switch (windowMode)
            {
                case DisplayServer.WindowMode.ExclusiveFullscreen:
                    return 0;
                case DisplayServer.WindowMode.Fullscreen:
                    return 1;
                case DisplayServer.WindowMode.Windowed:
                    return 2;
            }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
            switch (index)
            {
                case 0:
                    return DisplayServer.WindowMode.ExclusiveFullscreen;
                case 1:
                    return DisplayServer.WindowMode.Fullscreen;
                case 2:
                    return DisplayServer.WindowMode.Windowed;
            }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}
