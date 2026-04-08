using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using static Template.UI.Utilities;

namespace Template.UI.Controls;

public class ConfirmableNumericUpDown : NumericUpDown
{
    public static readonly StyledProperty<IInputElement?> TargetProperty =
        AvaloniaProperty.Register<ConfirmableNumericUpDown, IInputElement?>(nameof(Target));

    private decimal? _lastNonNullValue;
    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    public IInputElement? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override void OnValueChanged(decimal? oldValue, decimal? newValue)
    {
        if (newValue != null) _lastNonNullValue = newValue;

        base.OnValueChanged(oldValue, newValue);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Value ??= _lastNonNullValue;
            Target?.Focus(NavigationMethodBasedOnMouseOrKey);
            e.Handled = true;
        }

        base.OnKeyDown(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        Value ??= _lastNonNullValue;
        base.OnLostFocus(e);
    }
}
