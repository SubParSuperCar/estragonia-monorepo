using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;

namespace GameTemplate.UI.ViewModels;

public partial class EscapeMenuViewModel : ViewModel
{
    private readonly UserInterface _dialogUserInterface;
    private readonly FocusStack _focusStack;
    private readonly NavigatorViewModel _navigatorViewModel;
    private readonly ViewModelFactory _viewModelFactory;

    /// <summary>
    ///     Intended for designer usage only.
    /// </summary>
    public EscapeMenuViewModel()
    {
    }

    public EscapeMenuViewModel(ViewModelFactory viewModelFactory, NavigatorViewModel navigatorViewModel,
        FocusStack focusStack,
        UserInterface dialogUserInterface)
    {
        _viewModelFactory = viewModelFactory;
        _navigatorViewModel = navigatorViewModel;
        _focusStack = focusStack;
        _dialogUserInterface = dialogUserInterface;
    }

    [RelayCommand]
    public void ToOptions()
    {
        _navigatorViewModel.NavigateTo(_viewModelFactory.CreateOptions());
    }

    [RelayCommand]
    public void ToMainMenu()
    {
        var dialog = new DialogViewModel(
            "Are you sure you want to exit to the main menu?",
            "Cancel", confirmText: "Exit"
        );

        DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
        {
            if (response == DialogViewModel.Response.Confirm)
                _navigatorViewModel.NavigateTo(_viewModelFactory.CreateMainMenu(), clearStack: true);
        });
    }
}
