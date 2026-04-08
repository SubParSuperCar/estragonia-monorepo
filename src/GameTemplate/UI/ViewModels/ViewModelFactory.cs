using GameTemplate.Main;
using GameTemplate.UI.Models;
using Godot;

namespace GameTemplate.UI.ViewModels;

public class ViewModelFactory
{
    private readonly FocusStack _focusStack;
    private readonly KeyRepeater _keyRepeater;
    private readonly MainNode _mainNode;
    private readonly MainViewModel _mainViewModel;
    private readonly MainViewModel _mainViewModelDialog;
    private readonly Options _options;
    private readonly SceneTree _sceneTree;
    private readonly UserInterface _userInterfaceDialog;
    private readonly UserInterface _userInterfaceMain;

    public ViewModelFactory(MainNode mainNode, Options options, MainViewModel mainViewModel,
        MainViewModel mainViewModelDialog,
        UserInterface userInterfaceMain, UserInterface userInterfaceDialog, KeyRepeater keyRepeater,
        FocusStack focusStack, SceneTree sceneTree)
    {
        _mainNode = mainNode;
        _options = options;
        _mainViewModel = mainViewModel;
        _mainViewModelDialog = mainViewModelDialog;
        _userInterfaceMain = userInterfaceMain;
        _userInterfaceDialog = userInterfaceDialog;
        _keyRepeater = keyRepeater;
        _focusStack = focusStack;
        _sceneTree = sceneTree;
    }

    public virtual MainMenuViewModel CreateMainMenu()
    {
        return new MainMenuViewModel(this, _mainViewModel, _sceneTree);
    }

    public virtual OptionsViewModel CreateOptions()
    {
        return new OptionsViewModel(this, _userInterfaceMain);
    }

    /// <summary>
    ///     Assumes that this viewModel is created for the main UserInterface.
    /// </summary>
    public virtual OptionsGraphicsViewModel CreateOptionsGraphics()
    {
        return new OptionsGraphicsViewModel(_options, _focusStack, _userInterfaceDialog);
    }

    public virtual OptionsControlsViewModel CreateOptionsControls()
    {
        return new OptionsControlsViewModel(_focusStack, _userInterfaceDialog, _mainViewModelDialog, _keyRepeater);
    }

    public virtual OptionsAudioViewModel CreateOptionsAudio()
    {
        return new OptionsAudioViewModel(_options, _focusStack, _userInterfaceDialog);
    }

    public virtual EscapeMenuViewModel CreateEscapeMenu()
    {
        return new EscapeMenuViewModel(this, _mainViewModel, _focusStack, _userInterfaceDialog);
    }
}
