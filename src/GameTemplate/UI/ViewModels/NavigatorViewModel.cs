using System;
using System.Collections.Generic;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTemplate.Main;
using Godot;

namespace GameTemplate.UI.ViewModels;

public abstract partial class NavigatorViewModel(UserInterface? userInterface) : ViewModel
{
	private readonly Stack<ViewModel> _viewModels = new();

	private Utilities.PageTransitionWithDuration? _pageTransition;

	[ObservableProperty] public partial ViewModel? CurrentViewModel { get; set; }

	[ObservableProperty] public partial IPageTransition? Transition { get; set; }

	private void OnViewModelsAddedOrRemoved()
	{
		if (userInterface == null)
			return;

		userInterface.FocusMode = Control.FocusModeEnum.All;
		if (_viewModels.Count != 0) return;
		userInterface.FocusMode = Control.FocusModeEnum.None;
		userInterface.ReleaseFocus();
	}

	public void NavigateTo(ViewModel viewModel, Utilities.PageTransitionWithDuration? transition = null,
		bool replace = false, bool clearStack = false)
	{
		_pageTransition = transition;
		Transition = transition;

		if (clearStack)
			while (CurrentViewModel != null)
				CurrentViewModel.ForcedClose();
		else if (replace) CurrentViewModel?.ForcedClose();

		viewModel.Closed += OnViewModelClosed;
		_viewModels.Push(viewModel);
		CurrentViewModel = viewModel;

		OnViewModelsAddedOrRemoved();
		CurrentViewModel.FirstNavigationByNavigator();

		if (_pageTransition != null) DisableInputForTransitionDuration(_pageTransition);
		return;


		void OnViewModelClosed(bool forced)
		{
			viewModel.Closed -= OnViewModelClosed;
			_viewModels.Pop();

			CurrentViewModel = _viewModels.Count > 0 ? _viewModels.Peek() : null;

			if (forced)
				return;

			CurrentViewModel?.OnNavigatorFocusReturned();
			OnViewModelsAddedOrRemoved();

			if (_pageTransition != null) DisableInputForTransitionDuration(_pageTransition);
		}
	}

	private async void DisableInputForTransitionDuration(Utilities.PageTransitionWithDuration transition)
	{
		try
		{
			userInterface?.InputEnabled = false;

			await transition.StartToEnd();
			userInterface?.InputEnabled = true;
		}
		catch (Exception)
		{
			// ignored
		}
	}

	protected override void Close()
	{
		while (CurrentViewModel != null) CurrentViewModel.ForcedClose();
		base.Close();
	}

	public override void OnNavigatorFocusReturned()
	{
		base.OnNavigatorFocusReturned();
		CurrentViewModel?.OnNavigatorFocusReturned();
	}

	public override void OnUserInterfaceFocusReturned()
	{
		base.OnUserInterfaceFocusReturned();
		CurrentViewModel?.OnUserInterfaceFocusReturned();
	}

	public override void OnUserInterfaceFocusLost()
	{
		base.OnUserInterfaceFocusLost();
		CurrentViewModel?.OnUserInterfaceFocusLost();
	}
}
