using Avalonia.Input;

namespace GameTemplate.UI.Views;

/// <summary>
///     View that does not track the last focused control. Use this when it is contained within <br />
///     a NavigatorViewModel with a View that also inherits from View and not UserControl.
/// </summary>
public abstract class NestedView : View
{
	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		TrackFocussedControls = false;
		base.OnGotFocus(e);
	}

	protected override void FocusLast()
	{
	}
}
