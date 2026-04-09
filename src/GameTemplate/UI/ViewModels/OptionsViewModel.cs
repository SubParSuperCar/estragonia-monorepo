using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;

namespace GameTemplate.UI.ViewModels;

public interface IOptionsTabViewModel
{
	public void TryClose(Action callOnClose);
}

public partial class OptionsViewModel : NavigatorViewModel
{
	public enum OptionsTab
	{
		// ReSharper disable InconsistentNaming
		Graphics = 0,
		Controls = 1,

		// ReSharper disable once UnusedMember.Global
		Audio = 2
		// ReSharper restore InconsistentNaming
	}

	private readonly ViewModelFactory _viewModelFactory = null!;

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

	[ObservableProperty] public partial int CurrentTabIndex { get; set; }

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

	protected override void Close()
	{
		if (CurrentViewModel is IOptionsTabViewModel viewModel) viewModel.TryClose(() => base.Close());
	}
}
