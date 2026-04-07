using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Utilities;

namespace Template.UI.Controls;

[TemplatePart("PART_ValueDecrementer", typeof(Button))]
[TemplatePart("PART_ValueIncrementer", typeof(Button))]
[TemplatePart("PART_Container", typeof(Control))]
internal class HorizontalSelect : TemplatedControl
{
    public static readonly DirectProperty<HorizontalSelect, IEnumerable<int>> ValuesProperty =
        AvaloniaProperty.RegisterDirect<HorizontalSelect, IEnumerable<int>>(
            nameof(Values),
            o => o.Values,
            (o, v) => o.Values = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<HorizontalSelect, int> ValueProperty =
        AvaloniaProperty.RegisterDirect<HorizontalSelect, int>(
            nameof(Value),
            o => o.Value,
            (o, v) => o.Value = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<HorizontalSelect, string> DisplayedTextProperty =
        AvaloniaProperty.RegisterDirect<HorizontalSelect, string>(
            nameof(DisplayedText),
            o => o.DisplayedText,
            (o, v) => o.DisplayedText = v,
            defaultBindingMode: BindingMode.OneWay);

    public static readonly DirectProperty<HorizontalSelect, List<string>> ValueNamesProperty =
        AvaloniaProperty.RegisterDirect<HorizontalSelect, List<string>>(
            nameof(DisplayedText),
            o => o.ValueNames,
            (o, v) => o.ValueNames = v,
            defaultBindingMode: BindingMode.OneWay);

    private Control? _container;

    private string _displayedText = "";

    private bool _focusEngaged;

    private int _value;

    private Button? _valueDecrementer;
    private Button? _valueIncrementer;

    private List<string> _valueNames = [""];

    private IEnumerable<int> _values = Enumerable.Range(0, 1);

    static HorizontalSelect()
    {
        FocusableProperty.OverrideDefaultValue(typeof(HorizontalSelect), true);
    }

    public IEnumerable<int> Values
    {
        get => _values;
        set => SetAndRaise(ValuesProperty, ref _values, value);
    }

    public int Value
    {
        get => _value;
        set => SetAndRaise(ValueProperty, ref _value, value);
    }

    public string DisplayedText
    {
        get => _displayedText;
        set => SetAndRaise(DisplayedTextProperty, ref _displayedText, value);
    }

    public List<string> ValueNames
    {
        get => _valueNames;
        set => SetAndRaise(ValueNamesProperty, ref _valueNames, value);
    }

    private bool FocusEngaged
    {
        get => _focusEngaged;
        set
        {
            if (_focusEngaged != value)
            {
                if (_focusEngaged)
                    _container?.Classes.Remove("engaged");
                else
                    _container?.Classes.Add("engaged");
            }

            _focusEngaged = value;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_valueDecrementer != null)
        {
            _valueDecrementer.Click -= DecrementValue;
            _valueIncrementer!.Click -= IncrementValue;
        }

        _valueDecrementer = e.NameScope.Find("PART_ValueDecrementer") as Button;
        _valueIncrementer = e.NameScope.Find("PART_ValueIncrementer") as Button;
        _container = e.NameScope.Find("PART_Container") as Control;

        if (_valueDecrementer != null && _valueIncrementer != null)
        {
            _valueDecrementer.Click += DecrementValue;
            _valueIncrementer.Click += IncrementValue;
            _valueDecrementer.IsEnabled = Value != 0;
            _valueIncrementer.IsEnabled = Value != ValueNames.Count - 1;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueNamesProperty) Values = Enumerable.Range(0, ValueNames.Count);

        if (change.Property == ValueProperty || change.Property == ValueNamesProperty)
        {
            if (_valueDecrementer != null && _valueIncrementer != null)
            {
                _valueDecrementer.IsEnabled = Value == 0 ? false : true;
                _valueIncrementer.IsEnabled = Value == ValueNames.Count - 1 ? false : true;
            }

            DisplayedText = ValueNames[MathUtilities.Clamp(Value, 0, ValueNames.Count - 1)];
        }
    }

    private void DecrementValue()
    {
        Value = MathUtilities.Clamp(Value - 1, 0, ValueNames.Count - 1);
    }

    private void DecrementValue(object? sender, RoutedEventArgs e)
    {
        DecrementValue();
    }

    private void IncrementValue()
    {
        Value = MathUtilities.Clamp(Value + 1, 0, ValueNames.Count - 1);
    }

    private void IncrementValue(object? sender, RoutedEventArgs e)
    {
        IncrementValue();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        FocusEngaged = false;
        base.OnLostFocus(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Handled || e is { KeyModifiers: KeyModifiers.None, Key: Key.Up or Key.Down })
            return;

        if (e.Key == Key.Enter) FocusEngaged = !FocusEngaged;

        if (!_focusEngaged)
            return;

        if (e.Key == Key.Left)
        {
            DecrementValue();
            e.Handled = true;
        }
        else if (e.Key == Key.Right)
        {
            IncrementValue();
            e.Handled = true;
        }
    }
}
