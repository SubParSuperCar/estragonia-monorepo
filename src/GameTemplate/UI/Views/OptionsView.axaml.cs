using Avalonia;
using Avalonia.Controls;

namespace GameTemplate.UI.Views;

// ReSharper disable once UnusedType.Global
public partial class OptionsView : View
{
	private const int FocusInflation = 100;

	public OptionsView()
	{
		InitializeComponent();

		ScrollViewer.BringIntoViewOnFocusChange = false;
		ScrollViewer.GotFocus += (_, e) =>
		{
			var control = e.Source as Control;
			var inflatedSize = control?.DesiredSize.Inflate(new Thickness(FocusInflation)) ?? default;
			control?.BringIntoView(new Rect(-new Point(FocusInflation, FocusInflation), inflatedSize));
		};
	}
}
