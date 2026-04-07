using CommunityToolkit.Mvvm.Input;
using Godot;
using static EstragoniaTemplate.UI.Utilities;

namespace EstragoniaTemplate.UI.ViewModels;

public partial class MainMenuViewModel : ViewModel
{
    private readonly NavigatorViewModel _navigatorViewModel;
    private readonly SceneTree _sceneTree;
    private readonly ViewModelFactory _viewModelFactory;

    /// <summary>
    ///     Intended for designer usage only.
    /// </summary>
    public MainMenuViewModel()
    {
    }

    public MainMenuViewModel(ViewModelFactory viewModelFactory, NavigatorViewModel navigatorViewModel,
        SceneTree sceneTree)
    {
        _viewModelFactory = viewModelFactory;
        _navigatorViewModel = navigatorViewModel;
        _sceneTree = sceneTree;
    }

    [RelayCommand]
    public void ToGame()
    {
        _navigatorViewModel.NavigateTo(new GameViewModel());
    }

    [RelayCommand]
    public void ToOptions()
    {
        _navigatorViewModel.NavigateTo(_viewModelFactory.CreateOptions(),
            CreatePageTransition(TransitionType.Fade, 0.5f));
    }

    [RelayCommand]
    public void Quit()
    {
        _sceneTree.Quit();
    }
}
