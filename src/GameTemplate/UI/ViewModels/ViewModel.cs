using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameTemplate.UI.ViewModels;

public abstract partial class ViewModel : ObservableObject
{
	private bool _forcedClose;

	/// <summary>
	///     Invoked when this view becomes the top of the stack after another view is popped.
	/// </summary>
	public event EventHandler? NavigatorFocusReturned;

	public event EventHandler? UserInterfaceFocusReturned;
	public event EventHandler? UserInterfaceFocusLost;

	/// <summary>
	///     If argument is true: was closed forcefully (by the navigator).
	/// </summary>
	public event Action<bool>? Closed;

	[RelayCommand]
	protected virtual void Close()
	{
		Closed?.Invoke(_forcedClose);
	}

	public void ForcedClose()
	{
		_forcedClose = true;
		Close();
	}

	// ReSharper disable once VirtualMemberNeverOverridden.Global
	public virtual void FirstNavigationByNavigator()
	{
	}

	public virtual void OnNavigatorFocusReturned()
	{
		NavigatorFocusReturned?.Invoke(this, EventArgs.Empty);
	}

	public virtual void OnUserInterfaceFocusReturned()
	{
		UserInterfaceFocusReturned?.Invoke(this, EventArgs.Empty);
	}

	public virtual void OnUserInterfaceFocusLost()
	{
		UserInterfaceFocusLost?.Invoke(this, EventArgs.Empty);
	}
}
