using System.ComponentModel;
using Estragonia;
using GameMenu.UI;
using Godot;

namespace GameMenu;

public sealed partial class UserInterface : AvaloniaControl
{
	private MainViewModel _mainViewModel = null!;

	private UiOptions _uiOptions = null!;

	public override void _Ready()
	{
		GetWindow().MinSize = new Vector2I(1152, 648);

		GrabFocus();

		_uiOptions = new UiOptions
		{
			UIScale = RenderScaling,
			VSync = DisplayServer.WindowGetVsyncMode() != DisplayServer.VSyncMode.Disabled,
			Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen
		};
		_uiOptions.PropertyChanged += OnUIOptionsPropertyChanged;

		_mainViewModel = new MainViewModel(_uiOptions)
		{
			SceneTree = GetTree()
		};
		_ = _mainViewModel.EnsureLoadedAsync();

		Control = new MainView
		{
			DataContext = _mainViewModel
		};

		base._Ready();
	}

	private void OnUIOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(UiOptions.VSync):
				var vSyncMode = _uiOptions.VSync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
				DisplayServer.WindowSetVsyncMode(vSyncMode);
				break;

			case nameof(UiOptions.Fullscreen):
				var windowMode = _uiOptions.Fullscreen
					? DisplayServer.WindowMode.Fullscreen
					: DisplayServer.WindowMode.Windowed;
				DisplayServer.WindowSetMode(windowMode);
				break;

			case nameof(UiOptions.UIScale):
				RenderScaling = _uiOptions.UIScale;
				break;
		}
	}

	public override void _Process(double delta)
	{
		_mainViewModel.ProcessFrame();

		base._Process(delta);
	}
}
