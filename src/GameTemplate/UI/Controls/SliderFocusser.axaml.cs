using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using static GameTemplate.UI.Utilities;

namespace GameTemplate.UI.Controls;

public partial class SliderFocusser : UserControl
{
	public static readonly StyledProperty<IInputElement?> SliderProperty =
		AvaloniaProperty.Register<ConfirmableNumericUpDown, IInputElement?>(nameof(Slider));

	private static readonly DirectProperty<SliderFocusser, string> XyFocusModeProperty =
		AvaloniaProperty.RegisterDirect<SliderFocusser, string>(
			nameof(XyFocusMode),
			o => o.XyFocusMode,
			(o, v) => o.XyFocusMode = v,
			defaultBindingMode: BindingMode.OneWay);

	public SliderFocusser()
	{
		InitializeComponent();
	}

	public IInputElement? Slider
	{
		get => GetValue(SliderProperty);
		set => SetValue(SliderProperty, value);
	}

	public string XyFocusMode
	{
		get;
		set => SetAndRaise(XyFocusModeProperty, ref field, value);
	} = "Disabled";

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);

		XyFocusMode = "Disabled";
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		if (SliderContentControl.Content is Slider slider) slider.Focus(NavigationMethodBasedOnMouseOrKey);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);

		if (e.Handled || e is not { Key: Key.Up or Key.Down or Key.Left or Key.Right })
			return;

		XyFocusMode = "Enabled";
	}
}
