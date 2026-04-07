using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Template.Main;

namespace Template.UI.ViewModels;

public interface IOptionsTabViewModel
{
    public void TryClose(Action callOnClose);
}

public partial class OptionsViewModel : NavigatorViewModel
{
    public enum OptionsTab
    {
        Graphics = 0,
        Controls = 1,
        Audio = 2
    }

    private readonly ViewModelFactory _viewModelFactory;

    [ObservableProperty] private int _currentTabIndex;

    /// <summary>
    ///     Intended for designer usage only.
    /// </summary>
    public OptionsViewModel(ViewModel initialViewModel) : base(null)
    {
        NavigateTo(initialViewModel);
    }

    public OptionsViewModel(ViewModelFactory viewModelFactory, UserInterface userInterface) : base(userInterface)
    {
        _viewModelFactory = viewModelFactory;
        NavigateTo(viewModelFactory.CreateOptionsGraphics());
        ToOptionsTabCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public void ToOptionsTab(int tabIndex)
    {
        if (tabIndex == CurrentTabIndex)
            return;

        var tab = (OptionsTab)tabIndex;
        ViewModel newViewModel = tab switch
        {
            OptionsTab.Graphics => _viewModelFactory.CreateOptionsGraphics(),
            OptionsTab.Controls => _viewModelFactory.CreateOptionsControls(),
            OptionsTab.Audio => _viewModelFactory.CreateOptionsAudio(),
            _ => _viewModelFactory.CreateOptionsAudio()
        };

        if (CurrentViewModel is IOptionsTabViewModel currentViewModel)
            currentViewModel.TryClose(() =>
            {
                NavigateTo(newViewModel, replace: true);
                ToOptionsTabCommand.NotifyCanExecuteChanged();
                CurrentTabIndex = tabIndex;
            });
    }

    public override void Close()
    {
        if (CurrentViewModel is IOptionsTabViewModel viewModel) viewModel.TryClose(() => base.Close());
    }
}
