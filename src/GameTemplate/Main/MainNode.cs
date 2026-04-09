using System;
using GameTemplate.UI.Models;
using GameTemplate.UI.ViewModels;
using Godot;
using static GameTemplate.Main.AudioManager;

namespace GameTemplate.Main;

public partial class MainNode : Node2D
{
	private FocusStack _focusStack = null!;
	private SceneTree _sceneTree = null!;

	private ViewModelFactory _viewModelFactory = null!;

	[Export] private UserInterface? UserInterfaceMain { get; set; }

	[Export] private UserInterface? UserInterfaceDialog { get; set; }

	public override void _Ready()
	{
		if (UserInterfaceMain == null || UserInterfaceDialog == null)
#pragma warning disable CA2201
			throw new NullReferenceException();
#pragma warning restore CA2201

		MusicManager.Instance?.PlayMusic(this, MusicManager.Music.MainMenu);

		SerializableInputMap.LoadAndApplyInputMap();
		var options = Options.LoadOrCreate();

		var keyRepeater = new KeyRepeater();
		GetWindow().FocusExited += keyRepeater.ClearRepeatingAndBlockedInput;

		_sceneTree = GetTree();
		_focusStack = new FocusStack();

		var mainViewModelDialog = new MainViewModel(UserInterfaceDialog);
		var mainViewModel = new MainViewModel(UserInterfaceMain);
		var viewModelFactory = new ViewModelFactory(options,
			mainViewModel,
			mainViewModelDialog,
			UserInterfaceMain,
			UserInterfaceDialog,
			keyRepeater,
			_focusStack,
			_sceneTree);
		_viewModelFactory = viewModelFactory;

		UserInterfaceDialog.Initialize(mainViewModelDialog, keyRepeater);
		UserInterfaceMain.Initialize(
			mainViewModel,
			keyRepeater,
			viewModelFactory.CreateMainMenu());

		_focusStack.Push(UserInterfaceMain);
	}

	public override void _Input(InputEvent @event)
	{
		using (@event)
		{
			if ((@event is not InputEventKey { PhysicalKeycode: Key.Escape, Pressed: true } key || key.Echo)
			    && @event is not InputEventJoypadButton { ButtonIndex: JoyButton.Start }) return;
			var leafViewModel = UserInterfaceMain?.MainViewModel?.CurrentViewModel;
			while (leafViewModel is NavigatorViewModel navigator) leafViewModel = navigator.CurrentViewModel;

			if (leafViewModel is MainMenuViewModel
			    or EscapeMenuViewModel
			    or OptionsViewModel
			    or IOptionsTabViewModel) return;
			_sceneTree.Paused = true;
			Instance?.PauseOrResumeAudioPlayersBus(true, [Bus.SFX]);

			var escapeMenu = _viewModelFactory.CreateEscapeMenu();
			UserInterfaceMain?.MainViewModel?.NavigateTo(escapeMenu);
			if (UserInterfaceMain != null) _focusStack.Push(UserInterfaceMain);
			GetViewport().SetInputAsHandled();

			escapeMenu.Closed += OnClose;

			void OnClose(bool _)
			{
				escapeMenu.Closed -= OnClose;

				_focusStack.Pop();
				Instance?.ResumeAllAudio();
				_sceneTree.Paused = false;
			}
		}
	}
}
