using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class DifficultyViewModel(INavigator navigator) : ViewModel
{
	[ObservableProperty] public partial GameDifficulty SelectedDifficulty { get; set; } = GameDifficulty.Normal;

	public ObservableCollection<GameDifficulty> Difficulties { get; } = new(Enum.GetValues<GameDifficulty>());

	protected override Task LoadAsync() => Task.CompletedTask;

	[RelayCommand]
	private async Task StartGameAsync()
	{
		navigator.NavigateTo(new GameLoadingViewModel(navigator));
		await TryCloseAsync();
	}
}
