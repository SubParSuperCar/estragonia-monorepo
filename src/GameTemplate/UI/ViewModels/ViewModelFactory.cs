using GameTemplate.Main;
using GameTemplate.UI.Models;
using Godot;

namespace GameTemplate.UI.ViewModels;

public sealed class ViewModelFactory(
	Options options,
	MainViewModel mainViewModel,
	MainViewModel mainViewModelDialog,
	UserInterface userInterfaceMain,
	UserInterface userInterfaceDialog,
	KeyRepeater keyRepeater,
	FocusStack focusStack,
	SceneTree sceneTree)
{
	public MainMenuViewModel CreateMainMenu() => new(this, mainViewModel, sceneTree);

	public OptionsViewModel CreateOptions() => new(this, userInterfaceMain);

	/// <summary>
	///     Assumes that this viewModel is created for the main UserInterface.
	/// </summary>
	public OptionsGraphicsViewModel CreateOptionsGraphics() => new(options, focusStack, userInterfaceDialog);

	public OptionsControlsViewModel CreateOptionsControls() =>
		new(focusStack, userInterfaceDialog, mainViewModelDialog, keyRepeater);

	public OptionsAudioViewModel CreateOptionsAudio() => new(options, focusStack, userInterfaceDialog);

	public EscapeMenuViewModel CreateEscapeMenu() =>
		new(this, mainViewModel, focusStack, userInterfaceDialog);
}
