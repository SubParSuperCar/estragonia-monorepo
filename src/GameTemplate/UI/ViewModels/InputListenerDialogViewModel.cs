using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;
using Godot;

namespace GameTemplate.UI.ViewModels;

public partial class InputListenerDialogViewModel : ViewModel
{
	private readonly EventHandler<InputEvent>? _inputEventHandler;
	private readonly HashSet<Key> _reservedKeys = null!;

	private readonly UserInterface _userInterface = null!;

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
		_inputEventHandler = (_, inputEvent) =>
		{
			if (OnInputEvent(inputEvent)) userInterface.InputEventReceived -= _inputEventHandler;
		};

		userInterface.InputEventReceived += _inputEventHandler;
	}

	public bool ListenToKeyboard { get; } = true;
	public string InputName { get; } = "Input Name";
	public event Action<(Key?, JoyButton?)>? InputPressed;

	/// <summary>
	///     Returns true if the input was valid.
	/// </summary>
	private bool OnInputEvent(InputEvent inputEvent)
	{
		(Key?, JoyButton?)? inputTuple = null;
		switch (ListenToKeyboard)
		{
			case true when inputEvent is InputEventKey { Pressed: true } keyEvent
			               && ButtonToIconName.TryGetKeyboard(keyEvent.Keycode, out _)
			               && !_reservedKeys.Contains(keyEvent.PhysicalKeycode):
				// UserInterface will process the inputEvent after this method:
				// set pressed to false to prevent instant press after this dialog is closed.
				keyEvent.Pressed = false;
				inputTuple = (keyEvent.PhysicalKeycode, null);
				break;
			case false when inputEvent is InputEventJoypadButton { Pressed: true } joypadEvent
			                && ButtonToIconName.TryGetXbox(joypadEvent.ButtonIndex, out _):
				joypadEvent.Pressed = false;
				inputTuple = (null, joypadEvent.ButtonIndex);
				break;
		}

		if (inputTuple == null) return false;
		InputPressed?.Invoke(inputTuple.Value);
		Close();
		return true;
	}

	[RelayCommand]
	private void Cancel()
	{
		_userInterface.InputEventReceived -= _inputEventHandler;
		InputPressed?.Invoke((null, null));
		Close();
	}
}
