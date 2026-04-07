using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EstragoniaTemplate.Main;

public class KeyRepeater
{
    private const float SecondsUntilRepeat = 0.4f;
    private const float RepeatIntervalSeconds = 0.1f;
    private readonly HashSet<JoyButton> _blockedJoyButtons = new();
    private readonly HashSet<Key> _blockedKeys = new();

    private readonly StringName[] _directionalInputEventNames = ["ui_left", "ui_right", "ui_up", "ui_down"];

    private readonly Dictionary<InputEvent, float> _inputDownDurations = new();

    private readonly InputEventKey[] _reservedInputEvents =
    [
        new() { PhysicalKeycode = Key.Up, Keycode = Key.Up },
        new() { PhysicalKeycode = Key.Down, Keycode = Key.Down },
        new() { PhysicalKeycode = Key.Left, Keycode = Key.Left },
        new() { PhysicalKeycode = Key.Right, Keycode = Key.Right }
    ];

    private HashSet<InputEvent> _directionalInputEvents = new();

    public KeyRepeater()
    {
        UpdateDirectionalKeys();
    }

    /// <summary>
    ///     Returns false if the InputEvent should be handled further.
    /// </summary>
    public bool Input(InputEvent inputEvent)
    {
        var inputEventKey = inputEvent as InputEventKey;
        var joypadButton = inputEvent as InputEventJoypadButton;

        if (inputEventKey == null && joypadButton == null) return false;

        var pressed = inputEventKey?.Pressed ?? joypadButton!.Pressed;
        InputEvent? correspondingDirectionalEvent = null;
        foreach (var directionalEvent in _directionalInputEvents)
        {
            if (directionalEvent is InputEventKey keyEvent &&
                keyEvent.PhysicalKeycode == inputEventKey?.PhysicalKeycode)
            {
                correspondingDirectionalEvent = directionalEvent;
                break;
            }

            if (directionalEvent is InputEventJoypadButton joypadEvent &&
                joypadEvent.ButtonIndex == joypadButton?.ButtonIndex)
            {
                correspondingDirectionalEvent = directionalEvent;
                break;
            }
        }

        if (correspondingDirectionalEvent == null)
        {
            if (inputEventKey != null)
            {
                if (pressed) return !_blockedKeys.Add(inputEventKey.PhysicalKeycode);
                _blockedKeys.Remove(inputEventKey.PhysicalKeycode);
                return false;
            }

            if (pressed) return !_blockedJoyButtons.Add(joypadButton!.ButtonIndex);
            _blockedJoyButtons.Remove(joypadButton!.ButtonIndex);
            return false;
        }

        if (_inputDownDurations.ContainsKey(correspondingDirectionalEvent))
        {
            if (!pressed)
            {
                _inputDownDurations.Remove(correspondingDirectionalEvent);
                return false;
            }
        }
        else if (pressed)
        {
            _inputDownDurations.Add(correspondingDirectionalEvent, 0);
            return false;
        }

        return true;
    }

    public void ClearRepeatingAndBlockedInput()
    {
        _inputDownDurations.Clear();
        _blockedKeys.Clear();
        _blockedJoyButtons.Clear();
    }

    public void UpdateDirectionalKeys()
    {
        List<InputEvent> directionalEvents = new();
        foreach (var directionalName in _directionalInputEventNames)
        {
            var directionEvents = InputMap.ActionGetEvents(directionalName);
            directionalEvents.AddRange(directionEvents);
        }

        directionalEvents.AddRange(_reservedInputEvents);
        _directionalInputEvents = directionalEvents.ToHashSet();
        ClearRepeatingAndBlockedInput();
    }

    /// <summary>
    ///     Calls _GuiInput on the userInterface for repeating keys.
    /// </summary>
    public void Process(float delta, UserInterface userInterface)
    {
        foreach (var (directionalInputEvent, duration) in _inputDownDurations)
        {
            var newDuration = duration + delta;

            if (newDuration > SecondsUntilRepeat - RepeatIntervalSeconds)
            {
                var remainder = newDuration - (SecondsUntilRepeat - RepeatIntervalSeconds);
                if (remainder > RepeatIntervalSeconds)
                {
                    userInterface.ForceGuiInput(CreatePressedInputEvent(directionalInputEvent));

                    newDuration -= RepeatIntervalSeconds;
                }
            }

            _inputDownDurations[directionalInputEvent] = newDuration;
        }
    }

    private InputEvent CreatePressedInputEvent(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey keyEvent)
            return new InputEventKey
            {
                Echo = true,
                Pressed = true,
                PhysicalKeycode = keyEvent.PhysicalKeycode,
                Keycode = keyEvent.Keycode
            };

        if (inputEvent is InputEventJoypadButton joypadEvent)
            return new InputEventJoypadButton
            {
                Pressed = true,
                ButtonIndex = joypadEvent.ButtonIndex,
                Device = joypadEvent.Device
            };

        throw new ArgumentException("Argument was neither InputEventKey nor InputEventJoypadButton",
            nameof(inputEvent));
    }
}
