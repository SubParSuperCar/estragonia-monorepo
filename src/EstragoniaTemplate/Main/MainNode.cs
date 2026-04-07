using System;
using EstragoniaTemplate.UI.Models;
using EstragoniaTemplate.UI.ViewModels;
using Godot;
using static EstragoniaTemplate.Main.AudioManager;

namespace EstragoniaTemplate.Main;

public partial class MainNode : Node2D
{
    private FocusStack _focusStack;
    private SceneTree _sceneTree;

    private ViewModelFactory _viewModelFactory;

    [Export] private UserInterface? UserInterfaceMain { get; set; }

    [Export] private UserInterface? UserInterfaceDialog { get; set; }

    public override void _Ready()
    {
        if (UserInterfaceMain == null || UserInterfaceDialog == null)
            throw new NullReferenceException();

        MusicManager.Instance?.PlayMusic(this, MusicManager.Music.MainMenu);

        SerializableInputMap.LoadAndApplyInputMap();
        var options = Options.LoadOrCreate();

        var keyRepeater = new KeyRepeater();
        GetWindow().FocusExited += keyRepeater.ClearRepeatingAndBlockedInput;

        _sceneTree = GetTree();
        _focusStack = new FocusStack();

        var mainViewModelDialog = new MainViewModel(UserInterfaceDialog);
        var mainViewModel = new MainViewModel(UserInterfaceMain);
        var viewModelFactory = new ViewModelFactory(
            this,
            options,
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
            if ((@event is InputEventKey key && key.PhysicalKeycode == Key.Escape && key.Pressed && !key.Echo)
                || (@event is InputEventJoypadButton button && button.ButtonIndex == JoyButton.Start))
            {
                var leafViewModel = UserInterfaceMain?.MainViewModel?.CurrentViewModel;
                while (leafViewModel is NavigatorViewModel navigator) leafViewModel = navigator.CurrentViewModel;

                if (leafViewModel is not (
                    MainMenuViewModel
                    or EscapeMenuViewModel
                    or OptionsViewModel
                    or IOptionsTabViewModel))
                {
                    _sceneTree.Paused = true;
                    Instance?.PauseOrResumeAudioPlayersBus(true, [Bus.SFX]);

                    var escapeMenu = _viewModelFactory.CreateEscapeMenu();
                    UserInterfaceMain.MainViewModel?.NavigateTo(escapeMenu);
                    _focusStack.Push(UserInterfaceMain);
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
    }
}
