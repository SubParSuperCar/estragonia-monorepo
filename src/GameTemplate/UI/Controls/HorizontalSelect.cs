using System;
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

namespace GameTemplate.UI.Controls;

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

	private bool _focusEngaged;

	private Button? _valueDecrementer;
	private Button? _valueIncrementer;

	static HorizontalSelect()
	{
		FocusableProperty.OverrideDefaultValue(typeof(HorizontalSelect), true);
	}

	public IEnumerable<int> Values
	{
		get;
		set => SetAndRaise(ValuesProperty, ref field, value);
	} = Enumerable.Range(0, 1);

	public int Value
	{
		get;
		set => SetAndRaise(ValueProperty, ref field, value);
	}

	public string DisplayedText
	{
		get;
		set => SetAndRaise(DisplayedTextProperty, ref field, value);
	} = "";

	public List<string> ValueNames
	{
		get;
		set => SetAndRaise(ValueNamesProperty, ref field, value);
	} = [""];

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

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
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

		if (_valueDecrementer == null || _valueIncrementer == null) return;
		_valueDecrementer.Click += DecrementValue;
		_valueIncrementer.Click += IncrementValue;
		_valueDecrementer.IsEnabled = Value != 0;
		_valueIncrementer.IsEnabled = Value != ValueNames.Count - 1;
	}

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		base.OnPropertyChanged(change);

		if (change.Property == ValueNamesProperty) Values = Enumerable.Range(0, ValueNames.Count);

		if (change.Property != ValueProperty && change.Property != ValueNamesProperty) return;
		if (_valueDecrementer != null && _valueIncrementer != null)
		{
			_valueDecrementer.IsEnabled = Value != 0;
			_valueIncrementer.IsEnabled = Value != ValueNames.Count - 1;
		}

		DisplayedText = ValueNames[MathUtilities.Clamp(Value, 0, ValueNames.Count - 1)];
	}

	[Obsolete("Obsolete")]
	private void DecrementValue()
	{
		Value = MathUtilities.Clamp(Value - 1, 0, ValueNames.Count - 1);
	}

	[Obsolete("Obsolete")]
	private void DecrementValue(object? sender, RoutedEventArgs e)
	{
		DecrementValue();
	}

	[Obsolete("Obsolete")]
	private void IncrementValue()
	{
		Value = MathUtilities.Clamp(Value + 1, 0, ValueNames.Count - 1);
	}

	[Obsolete("Obsolete")]
	private void IncrementValue(object? sender, RoutedEventArgs e)
	{
		IncrementValue();
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		FocusEngaged = false;
		base.OnLostFocus(e);
	}

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnKeyDown(KeyEventArgs e)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		if (e.Handled || e is { KeyModifiers: KeyModifiers.None, Key: Key.Up or Key.Down })
			return;

		if (e.Key == Key.Enter) FocusEngaged = !FocusEngaged;

		if (!_focusEngaged)
			return;

		// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
		switch (e.Key)
		{
			case Key.Left:
				DecrementValue();
				e.Handled = true;
				break;
			case Key.Right:
				IncrementValue();
				e.Handled = true;
				break;
		}
	}
}
