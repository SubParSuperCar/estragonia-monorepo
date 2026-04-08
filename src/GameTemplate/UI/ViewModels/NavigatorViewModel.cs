using System.Collections.Generic;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTemplate.Main;
using Godot;

namespace GameTemplate.UI.ViewModels;

public abstract partial class NavigatorViewModel : ViewModel
{
    protected readonly UserInterface _userInterface;

    protected readonly Stack<ViewModel> _viewModels = new();

    [ObservableProperty] private ViewModel? _currentViewModel;

    private Utilities.PageTransitionWithDuration? _pageTransition;

    [ObservableProperty] private IPageTransition? _transition;

    public NavigatorViewModel(UserInterface userInterface)
    {
        _userInterface = userInterface;
    }

    protected virtual void OnViewModelsAddedOrRemoved()
    {
        if (_userInterface == null)
            return;

        _userInterface.FocusMode = Control.FocusModeEnum.All;
        if (_viewModels.Count == 0)
        {
            _userInterface.FocusMode = Control.FocusModeEnum.None;
            _userInterface.ReleaseFocus();
        }
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


        void OnViewModelClosed(bool forced)
        {
            viewModel.Closed -= OnViewModelClosed;
            _viewModels.Pop();

            if (_viewModels.Count > 0)
                CurrentViewModel = _viewModels.Peek();
            else
                CurrentViewModel = null;

            if (forced)
                return;

            CurrentViewModel?.OnNavigatorFocusReturned();
            OnViewModelsAddedOrRemoved();

            if (_pageTransition != null) DisableInputForTransitionDuration(_pageTransition);
        }
    }

    public async void DisableInputForTransitionDuration(Utilities.PageTransitionWithDuration transition)
    {
        _userInterface.InputEnabled = false;

        await transition.StartToEnd();
        _userInterface.InputEnabled = true;
    }

    public override void Close()
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
