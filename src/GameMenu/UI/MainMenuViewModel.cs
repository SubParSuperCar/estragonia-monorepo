using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class MainMenuViewModel(INavigator navigator, UiOptions uiOptions) : ViewModel
{
	protected override Task LoadAsync() => Task.CompletedTask;

	[RelayCommand]
	private void StartNewGame()
	{
		navigator.NavigateTo(new DifficultyViewModel(navigator));
	}

	[RelayCommand]
	private void OpenOptions()
	{
		navigator.NavigateTo(new OptionsViewModel(uiOptions));
	}

	[RelayCommand]
	private void Exit()
	{
		navigator.Quit();
	}
}
