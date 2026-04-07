using Avalonia.Input;
using Avalonia.Input.Raw;
using Godot;

namespace JLeb.Estragonia.Input;

/// <summary>Represents raw input event arguments related to a joypad button.</summary>
public class RawJoypadButtonEventArgs : RawInputEventArgs
{
    public RawJoypadButtonEventArgs(
        IJoypadDevice device,
        ulong timestamp,
        IInputRoot root,
        RawJoypadButtonEventType type,
        JoyButton button
    ) : base(device, timestamp, root)
    {
        Button = button;
        Type = type;
    }

    /// <summary>Gets the associated device.</summary>
    public new IJoypadDevice Device
        => (IJoypadDevice)base.Device;

    /// <summary>Gets whether the button is pressed or released.</summary>
    public RawJoypadButtonEventType Type { get; }

    /// <summary>Gets the button that was pressed or released.</summary>
    public JoyButton Button { get; }
}
