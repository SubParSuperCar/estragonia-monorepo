using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;
using Godot;

namespace GameTemplate.UI.ViewModels;

public partial class InputListenerDialogViewModel : ViewModel
{
    private readonly EventHandler<InputEvent>? _inputEventHandler;
    private readonly HashSet<Key> _reservedKeys;

    private readonly UserInterface _userInterface;

    /// <summary>
    ///     Intended for designer usage only.
    /// </summary>
    public InputListenerDialogViewModel()
    {
    }

    public InputListenerDialogViewModel(UserInterface userInterface, HashSet<Key> reservedKeys, bool listenToKeyboard,
        string inputName)
    {
        _reservedKeys = reservedKeys;
        ListenToKeyboard = listenToKeyboard;
        InputName = inputName;

        _userInterface = userInterface;
        _inputEventHandler = null;
        _inputEventHandler = (sender, inputEvent) =>
        {
            if (OnInputEvent(sender, inputEvent)) userInterface.InputEventReceived -= _inputEventHandler;
        };

        userInterface.InputEventReceived += _inputEventHandler;
    }

    public bool ListenToKeyboard { get; } = true;
    public string InputName { get; } = "Input Name";
    public event Action<(Key?, JoyButton?)>? InputPressed;

    /// <summary>
    ///     Returns true if the input was valid.
    /// </summary>
    private bool OnInputEvent(object? sender, InputEvent inputEvent)
    {
        var userInterface = (UserInterface)sender!;

        (Key?, JoyButton?)? inputTuple = null;
        if (ListenToKeyboard && inputEvent is InputEventKey keyEvent && keyEvent.Pressed
            && ButtonToIconName.TryGetKeyboard(keyEvent.Keycode, out var name)
            && !_reservedKeys.Contains(keyEvent.PhysicalKeycode))
        {
            // UserInterface will process the inputEvent after this method:
            // set pressed to false to prevent instant press after this dialog is closed.
            keyEvent.Pressed = false;
            inputTuple = (keyEvent.PhysicalKeycode, null);
        }
        else if (!ListenToKeyboard && inputEvent is InputEventJoypadButton joypadEvent && joypadEvent.Pressed
                 && ButtonToIconName.TryGetXbox(joypadEvent.ButtonIndex, out _))
        {
            joypadEvent.Pressed = false;
            inputTuple = (null, joypadEvent.ButtonIndex);
        }

        if (inputTuple != null)
        {
            InputPressed?.Invoke(inputTuple.Value);
            Close();
            return true;
        }

        return false;
    }

    [RelayCommand]
    public void Cancel()
    {
        _userInterface.InputEventReceived -= _inputEventHandler;
        InputPressed?.Invoke((null, null));
        Close();
    }
}
