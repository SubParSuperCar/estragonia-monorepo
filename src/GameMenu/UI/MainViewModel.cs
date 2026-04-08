using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace GameMenu.UI;

public sealed partial class MainViewModel : ViewModel, INavigator
{
	private readonly List<ViewModel> _openViewModels = new();

	[ObservableProperty] private int _framesPerSecond;

	public MainViewModel(UIOptions uiOptions)
	{
		UIOptions = uiOptions;
	}

	public UIOptions UIOptions { get; }

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

	public async Task<bool> TryCloseCurrentAsync() =>
		CurrentViewModel is { } viewModel && await viewModel.TryCloseAsync();

	protected override Task LoadAsync()
	{
		NavigateTo(new MainMenuViewModel(this, UIOptions));
		return Task.CompletedTask;
	}

	public override void ProcessFrame()
	{
		FramesPerSecond = (int)Engine.GetFramesPerSecond();
		CurrentViewModel?.ProcessFrame();
	}
}
