using CommunityToolkit.Mvvm.Input;
using Godot;
using static GameTemplate.UI.Utilities;

namespace GameTemplate.UI.ViewModels;

public partial class MainMenuViewModel : ViewModel
{
	private readonly NavigatorViewModel _navigatorViewModel = null!;
	private readonly SceneTree _sceneTree = null!;
	private readonly ViewModelFactory _viewModelFactory = null!;

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
	private void ToGame()
	{
		_navigatorViewModel.NavigateTo(new GameViewModel());
	}

	[RelayCommand]
	private void ToOptions()
	{
		_navigatorViewModel.NavigateTo(_viewModelFactory.CreateOptions(),
			CreatePageTransition(TransitionType.Fade, 0.5f));
	}

	[RelayCommand]
	private void Quit()
	{
		_sceneTree.Quit();
	}
}
