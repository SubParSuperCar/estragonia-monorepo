using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class OptionsViewModel(UiOptions uiOptions) : ViewModel
{
	[ObservableProperty] public partial bool Fullscreen { get; set; } = uiOptions.Fullscreen;

	[ObservableProperty] public partial bool ShowFps { get; set; } = uiOptions.ShowFps;

	[ObservableProperty]
	[field: SuppressMessage("ReSharper", "InconsistentNaming",
		Justification = "Name required for correct property generation")]
	public partial double UIScale { get; set; } = uiOptions.UIScale;

	[ObservableProperty] public partial bool VSync { get; set; } = uiOptions.VSync;

	public bool CanApply
	{
		get;
		private set
		{
			if (SetProperty(ref field, value))
				ApplyCommand.NotifyCanExecuteChanged();
		}
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);

		if (e.PropertyName is nameof(VSync) or nameof(Fullscreen) or nameof(ShowFps) or nameof(UIScale))
			CanApply = true;
	}

	protected override Task LoadAsync() => Task.CompletedTask;

	[RelayCommand(CanExecute = nameof(CanApply))]
	private void Apply()
	{
		uiOptions.VSync = VSync;
		uiOptions.Fullscreen = Fullscreen;
		uiOptions.ShowFps = ShowFps;
		uiOptions.UIScale = UIScale;
		CanApply = false;
	}
}
