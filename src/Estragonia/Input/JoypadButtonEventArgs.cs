using Avalonia.Interactivity;
using Godot;

namespace Estragonia.Input;

/// <summary>Provides information about a joypad button event.</summary>
public class JoypadButtonEventArgs : RoutedEventArgs
{
	public JoypadButtonEventArgs(RoutedEvent? routedEvent, object? source, IJoypadDevice device, JoyButton button)
		: base(routedEvent, source)
	{
		Device = device;
		Button = button;
	}

	/// <summary>Gets the device where the event comes from.</summary>
	public IJoypadDevice Device { get; }

	/// <summary>Gets the button that was pressed or released.</summary>
	public JoyButton Button { get; }
}
