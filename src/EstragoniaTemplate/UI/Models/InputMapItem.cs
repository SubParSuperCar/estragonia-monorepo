using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace EstragoniaTemplate.UI.Models;

public partial class InputMapItem : ObservableObject
{
    private readonly StringName _inputMapAction;
    private readonly InputMapGroup _inputMapGroup;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(JoyButtonName))]
    private int? _controllerEnumValue = (int)JoyButton.A;

    private InputEventJoypadButton? _inputMapJoypadEvent;

    private InputEventKey? _inputMapKeyEvent;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(KeyName))]
    private int? _keyEnumValue = (int)Key.Right;

    public InputMapItem(string inputMapAction, string inputName, InputMapGroup inputMapGroup,
        string[]? reservedKeyNames = null)
    {
        _inputMapAction = inputMapAction;
        InputName = inputName;
        _inputMapGroup = inputMapGroup;
        ReservedKeyPaths = reservedKeyNames;

        KeyEnumValue = null;
        ControllerEnumValue = null;

        var inputEvents = InputMap.ActionGetEvents(_inputMapAction);

        foreach (var inputEvent in inputEvents)
            if (inputEvent is InputEventKey inputKey)
            {
                _inputMapKeyEvent = inputKey;
                KeyEnumValue = (int)inputKey.PhysicalKeycode;
                inputMapGroup.KeyMappings.Add(inputKey.PhysicalKeycode, this);
            }
            else if (inputEvent is InputEventJoypadButton inputJoypad)
            {
                _inputMapJoypadEvent = inputJoypad;
                ControllerEnumValue = (int)inputJoypad.ButtonIndex;
                inputMapGroup.JoypadMappings.Add(inputJoypad.ButtonIndex, this);
            }
    }

    public InputMapItem(string inputName, Key? key = null, JoyButton? joyButton = null,
        string[]? reservedKeyNames = null)
    {
        InputName = inputName;
        ReservedKeyPaths = reservedKeyNames;

        KeyEnumValue = null;
        ControllerEnumValue = null;

        if (key != null) KeyEnumValue = (int)key;
        if (joyButton != null) ControllerEnumValue = (int)joyButton;
    }

    public string InputName { get; } = "";
    public string KeyName => KeyEnumValue == null ? "" : ((Key)KeyEnumValue).ToString();
    public string JoyButtonName => ControllerEnumValue == null ? "" : ((JoyButton)ControllerEnumValue).ToString();

    public HashSet<Key> GroupReservedKeys => _inputMapGroup.ReservedKeys;
    public string[]? ReservedKeyPaths { get; }

    public void SetKeyboardKey(Key? newKey)
    {
        Key? oldKey = KeyEnumValue == null ? null : (Key)KeyEnumValue;

        if (oldKey != null)
        {
            _inputMapGroup.KeyMappings.Remove(oldKey.Value);
            InputMap.ActionEraseEvent(_inputMapAction, _inputMapKeyEvent);
        }

        if (newKey == null)
        {
            KeyEnumValue = null;
            return;
        }

        if (_inputMapGroup.KeyMappings.TryGetValue(newKey.Value, out var existingInputMapItem))
            existingInputMapItem.SetKeyboardKey(oldKey);

        var inputKey = new InputEventKey
        {
            PhysicalKeycode = newKey.Value
        };

        InputMap.ActionAddEvent(_inputMapAction, inputKey);
        _inputMapGroup.KeyMappings.Add(newKey.Value, this);
        _inputMapKeyEvent = inputKey;
        KeyEnumValue = (int)newKey;
    }

    public void SetJoypadButton(JoyButton? newButton)
    {
        JoyButton? oldButton = ControllerEnumValue == null ? null : (JoyButton)ControllerEnumValue;

        if (oldButton != null)
        {
            if (oldButton == newButton)
                return;

            _inputMapGroup.JoypadMappings.Remove(oldButton.Value);
            InputMap.ActionEraseEvent(_inputMapAction, _inputMapJoypadEvent);
        }

        if (newButton == null)
        {
            ControllerEnumValue = null;
            return;
        }

        if (_inputMapGroup.JoypadMappings.TryGetValue(newButton.Value, out var existingInputMapItem))
            existingInputMapItem.SetJoypadButton(oldButton);

        var inputJoypad = new InputEventJoypadButton
        {
            ButtonIndex = newButton.Value
        };

        InputMap.ActionAddEvent(_inputMapAction, inputJoypad);
        _inputMapGroup.JoypadMappings.Add(newButton.Value, this);
        _inputMapJoypadEvent = inputJoypad;
        ControllerEnumValue = (int)newButton;
    }
}
