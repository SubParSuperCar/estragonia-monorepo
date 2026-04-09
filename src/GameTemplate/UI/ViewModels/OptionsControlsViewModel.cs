using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;
using GameTemplate.UI.Models;
using Godot;

namespace GameTemplate.UI.ViewModels;

public partial class OptionsControlsViewModel : ViewModel, IOptionsTabViewModel
{
	private readonly UserInterface _dialogUserInterface = null!;

	private readonly FocusStack _focusStack = null!;
	private readonly KeyRepeater _keyRepeater = null!;

	private readonly MainViewModel _mainViewModelDialog = null!;

	/// <summary>
	///     Intended for designer usage only.
	/// </summary>
	public OptionsControlsViewModel()
	{
		NavigationInputMapItems =
		[
			new InputMapItem("Confirm", Key.Enter, JoyButton.A, ["Keyboard/keyboard_a", "Keyboard/keyboard_b"]),
			new InputMapItem("Cancel", Key.Escape, JoyButton.X),
			new InputMapItem("A", Key.A),
			new InputMapItem("B", Key.B)
		];

		GameplayInputMapItems =
		[
			new InputMapItem("A", Key.A),
			new InputMapItem("B", Key.B),
			new InputMapItem("C", Key.C),
			new InputMapItem("D", Key.D)
		];
	}

	public OptionsControlsViewModel(FocusStack focusStack, UserInterface dialogUserInterface,
		MainViewModel mainViewModelDialog, KeyRepeater keyRepeater)
	{
		_focusStack = focusStack;
		_dialogUserInterface = dialogUserInterface;
		_mainViewModelDialog = mainViewModelDialog;
		_keyRepeater = keyRepeater;

		SetInputMapItems();
	}

	[ObservableProperty] public partial ObservableCollection<InputMapItem> GameplayInputMapItems { get; set; } = null!;

	[ObservableProperty]
	public partial ObservableCollection<InputMapItem> NavigationInputMapItems { get; set; } = null!;

	public void TryClose(Action callOnClose)
	{
		SerializableInputMap.SaveCurrentInputMap();
		callOnClose();
	}

	private void SetInputMapItems()
	{
		HashSet<Key> navigationReservedKeys =
		[
			Key.Escape,
			Key.Enter,
			Key.Space
		];
		var navigationGroup = new InputMapGroup(navigationReservedKeys);
		NavigationInputMapItems =
		[
			new InputMapItem("ui_accept", "Confirm", navigationGroup,
				["Keyboard/keyboard_enter", "Keyboard/keyboard_space"]),
			new InputMapItem("ui_cancel", "Cancel", navigationGroup, ["Keyboard/keyboard_escape"]),
			new InputMapItem("ui_left", "Left", navigationGroup, ["Keyboard/keyboard_arrow_left"]),
			new InputMapItem("ui_right", "Right", navigationGroup, ["Keyboard/keyboard_arrow_right"]),
			new InputMapItem("ui_up", "Up", navigationGroup, ["Keyboard/keyboard_arrow_up"]),
			new InputMapItem("ui_down", "Down", navigationGroup, ["Keyboard/keyboard_arrow_down"])
		];

		var gameplayGroup = new InputMapGroup();
		GameplayInputMapItems =
		[
			new InputMapItem("game_accept", "Confirm", gameplayGroup),
			new InputMapItem("game_cancel", "Cancel", gameplayGroup)
		];
	}

	[RelayCommand]
	private void ResetToDefault()
	{
		var dialog = new DialogViewModel(
			"Are you sure you want to reset all control bindings to their defaults?\n" +
			"Any made changes will be lost.",
			"Cancel", confirmText: "Reset to default"
		);

		DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
		{
			if (response == DialogViewModel.Response.Confirm)
			{
				InputMap.LoadFromProjectSettings();
				SetInputMapItems();
			}

			_keyRepeater.UpdateDirectionalKeys();
		});
	}

	[RelayCommand]
	private void InputPromptKeyboard(InputMapItem inputMapItem)
	{
		InputPrompt(inputMapItem, true);
	}

	[RelayCommand]
	private void InputPromptJoypad(InputMapItem inputMapItem)
	{
		InputPrompt(inputMapItem, false);
	}

	private void InputPrompt(InputMapItem inputMapItem, bool listenToKeyboard)
	{
		var dialog = new InputListenerDialogViewModel(_dialogUserInterface, inputMapItem.GroupReservedKeys,
			listenToKeyboard, inputMapItem.InputName);
		dialog.InputPressed += OnInput;

		_mainViewModelDialog.NavigateTo(dialog);
		_focusStack.Push(_dialogUserInterface);
		dialog.Closed += _ => _focusStack.Pop();
		return;

		void OnInput((Key?, JoyButton?) inputTuple)
		{
			dialog.InputPressed -= OnInput;
			var (key, joyButton) = inputTuple;
			if (key != null)
				inputMapItem.SetKeyboardKey(key.Value);
			else if (joyButton != null) inputMapItem.SetJoypadButton(joyButton.Value);

			_keyRepeater.UpdateDirectionalKeys();
		}
	}
}
