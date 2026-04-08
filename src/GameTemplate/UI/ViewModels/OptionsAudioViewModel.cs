using System;
using System.ComponentModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Template.Main;
using Template.UI.Models;
using static Template.Main.AudioManager;

namespace Template.UI.ViewModels;

public partial class OptionsAudioViewModel : ViewModel, IOptionsTabViewModel
{
    private readonly UserInterface _dialogUserInterface;

    private readonly FocusStack _focusStack;

    private readonly Options _options;

    [ObservableProperty] private int _interfaceLevel;

    [ObservableProperty] private int _masterLevel;

    [ObservableProperty] private int _musicLevel;

    [ObservableProperty] private int _soundEffectsLevel;

    /// <summary>
    ///     Intended for designer usage only.
    /// </summary>
    public OptionsAudioViewModel()
    {
    }

    public OptionsAudioViewModel(Options options, FocusStack focusStack, UserInterface dialogUserInterface)
    {
        _options = options;
        _focusStack = focusStack;
        _dialogUserInterface = dialogUserInterface;

        MasterLevel = GetBusLinearEnergyPercentage(AudioManager.Bus.Master);
        MusicLevel = GetBusLinearEnergyPercentage(AudioManager.Bus.Music);
        SoundEffectsLevel = GetBusLinearEnergyPercentage(AudioManager.Bus.SFX);
        InterfaceLevel = GetBusLinearEnergyPercentage(AudioManager.Bus.UI);
    }

    public void TryClose(Action callOnClose)
    {
        _options.AudioOptions = new AudioOptions
        {
            MasterLevel = MasterLevel,
            MusicLevel = MusicLevel,
            SoundEffectsLevel = SoundEffectsLevel,
            InterfaceLevel = InterfaceLevel
        };
        _options.Save();

        callOnClose();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Design.IsDesignMode)
            return;

        switch (e.PropertyName)
        {
            case nameof(MasterLevel):
                UpdateBusDbLevelFromLinear(AudioManager.Bus.Master, MasterLevel);
                break;
            case nameof(MusicLevel):
                UpdateBusDbLevelFromLinear(AudioManager.Bus.Music, MusicLevel);
                break;
            case nameof(SoundEffectsLevel):
                UpdateBusDbLevelFromLinear(AudioManager.Bus.SFX, SoundEffectsLevel);
                break;
            case nameof(InterfaceLevel):
                UpdateBusDbLevelFromLinear(AudioManager.Bus.UI, InterfaceLevel);
                break;
        }
    }

    [RelayCommand]
    public void ResetToDefault()
    {
        var dialog = new DialogViewModel(
            "Are you sure you want to reset the audio levels to their defaults?\n" +
            "Any made changes will be lost.",
            "Cancel", confirmText: "Reset to default"
        );

        DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
        {
            if (response == DialogViewModel.Response.Confirm)
            {
                MasterLevel = 100;
                MusicLevel = 100;
                SoundEffectsLevel = 100;
                InterfaceLevel = 100;
            }
        });
    }
}
