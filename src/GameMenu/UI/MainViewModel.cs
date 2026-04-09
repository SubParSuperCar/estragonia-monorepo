using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace GameMenu.UI;

public sealed partial class MainViewModel(UiOptions uiOptions) : ViewModel, INavigator
{
	private readonly List<ViewModel> _openViewModels = [];

	[ObservableProperty] public partial int FramesPerSecond { get; set; }

	public UiOptions UiOptions { get; } = uiOptions;

	public ViewModel? CurrentViewModel
		=> _openViewModels.Count > 0 ? _openViewModels[^1] : null;

	public void NavigateTo(ViewModel viewModel)
	{
		viewModel.SceneTree = SceneTree;
		_ = viewModel.EnsureLoadedAsync();

		_openViewModels.Add(viewModel);
		viewModel.Closed += OnViewModelClosed;
		OnPropertyChanged(nameof(CurrentViewModel));
		return;

		void OnViewModelClosed(object? sender, EventArgs e)
		{
			viewModel.Closed -= OnViewModelClosed;
			viewModel.SceneTree = null;

			var isCurrent = CurrentViewModel == viewModel;
			_openViewModels.Remove(viewModel);

			if (isCurrent)
				OnPropertyChanged(nameof(CurrentViewModel));
		}
	}

	public void Quit()
	{
		SceneTree?.Quit();
	}

	protected override async Task<bool> TryCloseCoreAsync()
	{
		while (CurrentViewModel is not null)
			if (!await TryCloseCurrentAsync())
				return false;

		return true;
	}

	private async Task<bool> TryCloseCurrentAsync() =>
		CurrentViewModel is { } viewModel && await viewModel.TryCloseAsync();

	protected override Task LoadAsync()
	{
		NavigateTo(new MainMenuViewModel(this, UiOptions));
		return Task.CompletedTask;
	}

	public override void ProcessFrame()
	{
		FramesPerSecond = (int)Engine.GetFramesPerSecond();
		CurrentViewModel?.ProcessFrame();
	}
}
