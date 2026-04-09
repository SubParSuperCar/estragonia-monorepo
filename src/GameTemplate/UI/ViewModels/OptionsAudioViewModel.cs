using System;
using System.ComponentModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;
using GameTemplate.UI.Models;
using static GameTemplate.Main.AudioManager;

namespace GameTemplate.UI.ViewModels;

public partial class OptionsAudioViewModel : ViewModel, IOptionsTabViewModel
{
	private readonly UserInterface _dialogUserInterface = null!;

	private readonly FocusStack _focusStack = null!;

	private readonly Options _options = null!;

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

		MasterLevel = GetBusLinearEnergyPercentage(Bus.Master);
		MusicLevel = GetBusLinearEnergyPercentage(Bus.Music);
		SoundEffectsLevel = GetBusLinearEnergyPercentage(Bus.SFX);
		InterfaceLevel = GetBusLinearEnergyPercentage(Bus.UI);
	}

	[ObservableProperty] public partial int InterfaceLevel { get; set; }

	[ObservableProperty] public partial int MasterLevel { get; set; }

	[ObservableProperty] public partial int MusicLevel { get; set; }

	[ObservableProperty] public partial int SoundEffectsLevel { get; set; }

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
				UpdateBusDbLevelFromLinear(Bus.Master, MasterLevel);
				break;
			case nameof(MusicLevel):
				UpdateBusDbLevelFromLinear(Bus.Music, MusicLevel);
				break;
			case nameof(SoundEffectsLevel):
				UpdateBusDbLevelFromLinear(Bus.SFX, SoundEffectsLevel);
				break;
			case nameof(InterfaceLevel):
				UpdateBusDbLevelFromLinear(Bus.UI, InterfaceLevel);
				break;
		}
	}

	[RelayCommand]
	private void ResetToDefault()
	{
		var dialog = new DialogViewModel(
			"Are you sure you want to reset the audio levels to their defaults?\n" +
			"Any made changes will be lost.",
			"Cancel", confirmText: "Reset to default"
		);

		DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
		{
			if (response != DialogViewModel.Response.Confirm) return;
			MasterLevel = 100;
			MusicLevel = 100;
			SoundEffectsLevel = 100;
			InterfaceLevel = 100;
		});
	}
}
