using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMenu.UI;

public sealed partial class UiOptions : ObservableObject
{
	[ObservableProperty] public partial bool Fullscreen { get; set; }

	[ObservableProperty] public partial bool ShowFps { get; set; } = true;

	[ObservableProperty]
	[field: SuppressMessage("ReSharper", "InconsistentNaming",
		Justification = "Name required for correct property generation")]
	public partial double UIScale { get; set; } = 1.0;

	[ObservableProperty] public partial bool VSync { get; set; } = true;
}
