using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;
using GameTemplate.UI.Models;

namespace GameTemplate.UI.ViewModels;

public partial class OptionsGraphicsViewModel : ViewModel, IOptionsTabViewModel
{
	private readonly UserInterface _dialogUserInterface;

	private readonly FocusStack _focusStack;

	private readonly Options _options;
	private readonly GraphicsOptions _savedGraphicsOptions;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
	private bool _canApply;

	private GraphicsOptions _currentlyAppliedGraphicsOptions;

	[ObservableProperty] private GraphicsOptions _graphicsOptions;

	public OptionsGraphicsViewModel(Options options, FocusStack focusStack, UserInterface dialogUserInterface) :
		this(options)
	{
		_focusStack = focusStack;
		_dialogUserInterface = dialogUserInterface;
	}

	/// <summary>
	///     Intended for designer usage only.
	/// </summary>
	public OptionsGraphicsViewModel(Options options)
	{
		_options = options;
		_savedGraphicsOptions = new GraphicsOptions(options.GraphicsOptions);
		_currentlyAppliedGraphicsOptions = new GraphicsOptions(options.GraphicsOptions);

		GraphicsOptions = options.GraphicsOptions;
		GraphicsOptions.PropertyChanged += OptionsPropertyChangedHandler;

		CanApply = false;
	}

	/// <summary>
	///     Intended for designer usage only.
	/// </summary>
	public OptionsGraphicsViewModel() : this(new Options())
	{
	}

	public void TryClose(Action callOnClose)
	{
		if (!_currentlyAppliedGraphicsOptions.Equals(_savedGraphicsOptions))
		{
			var dialog = new DialogViewModel(
				"You have applied changes to the graphics settings.\n" +
				"Revert the changes or save current settings?",
				"Cancel", "Revert changes", "Save current settings"
			);

			DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
			{
				switch (response)
				{
					case DialogViewModel.Response.Cancel:
						return;

					case DialogViewModel.Response.Deny:
						GraphicsOptions.SetFromOptions(_savedGraphicsOptions);
						GraphicsOptions.Apply();
						GraphicsOptions.PropertyChanged -= OptionsPropertyChangedHandler;
						callOnClose();
						return;

					case DialogViewModel.Response.Confirm:
						GraphicsOptions.SetFromOptions(_currentlyAppliedGraphicsOptions);
						GraphicsOptions.Apply();
						GraphicsOptions.PropertyChanged -= OptionsPropertyChangedHandler;
						_options.Save();
						callOnClose();
						return;
				}
			});
		}
		else
		{
			GraphicsOptions.SetFromOptions(_savedGraphicsOptions);
			callOnClose();
		}
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);

		if (e.PropertyName != nameof(CanApply)) CanApply = true;
	}

	private void OptionsPropertyChangedHandler(object? s, PropertyChangedEventArgs e)
	{
		OnPropertyChanged(e);
	}

	[RelayCommand(CanExecute = nameof(CanApply))]
	public void Apply()
	{
		GraphicsOptions.Apply();
		CanApply = false;
		_currentlyAppliedGraphicsOptions = new GraphicsOptions(GraphicsOptions);
	}

	[RelayCommand]
	public void ResetToDefault()
	{
		var dialog = new DialogViewModel(
			"Are you sure you want to reset the graphics settings to their defaults?\n" +
			"Any made changes will be lost.",
			"Cancel", confirmText: "Reset to default"
		);

		DialogViewModel.OpenDialog(_dialogUserInterface, _focusStack, dialog, response =>
		{
			if (response == DialogViewModel.Response.Confirm)
			{
				var defaultOptions = new GraphicsOptions();
				GraphicsOptions.SetFromOptions(defaultOptions);
				Apply();
			}
		});
	}
}
