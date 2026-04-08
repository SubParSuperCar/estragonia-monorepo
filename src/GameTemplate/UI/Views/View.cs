using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using GameTemplate.Main;
using GameTemplate.UI.ViewModels;
using static GameTemplate.UI.Utilities;

namespace GameTemplate.UI.Views;

public class FocussedControl
{
	public FocussedControl(Control control)
	{
		Control = control;
	}

	public Control Control { get; }

	public virtual bool TryFocus() => Control.Focus(NavigationMethodBasedOnMouseOrKey);
}

public class FocussedItemsControl : FocussedControl
{
	private readonly int _containerIndex;

	public FocussedItemsControl(Control item, ItemsControl itemsControl) : base(itemsControl)
	{
		var container = itemsControl.ContainerFromItem(item.DataContext);
		_containerIndex = itemsControl.IndexFromContainer(container);
	}

	public override bool TryFocus()
	{
		var itemsControl = (ItemsControl)Control;
		itemsControl.UpdateLayout();

		// First, check the saved index. If this is unsuccessful, check the previous index (item directly above)
		for (var i = 0; i < 2; i++)
		{
			var container = itemsControl.ContainerFromIndex(_containerIndex - i);
			var button = container?.FindLogicalDescendantOfType<Button>();
			if (button != null)
				return button.Focus(NavigationMethodBasedOnMouseOrKey);
		}

		return false;
	}
}

public abstract class View : UserControl
{
	/// <summary>
	///     Index 0 indicates the most recently focussed control.
	/// </summary>
	private readonly List<FocussedControl> _lastFocussedControls = new();

	private bool _firstLoad = true;
	private bool _focusLastOnLoaded;
	private NavigationMethod _previousNavigationMethod = NavigationMethod.Unspecified;

	protected bool TrackFocussedControls = true;
	protected virtual int TrackedLastFocussedControlsCount { get; } = 5;

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		_previousNavigationMethod = NavigationMethodBasedOnMouseOrKey;

		base.OnGotFocus(e);

		if (IsLoaded && TrackFocussedControls && e.Source is Control control && control.IsFocused)
		{
			var itemsControl = control.FindAncestorOfType<ItemsControl>();
			if (itemsControl != null)
				_lastFocussedControls.Insert(0, new FocussedItemsControl(control, itemsControl));
			else
				_lastFocussedControls.Insert(0, new FocussedControl(control));

			if (_lastFocussedControls.Count > TrackedLastFocussedControlsCount)
				_lastFocussedControls.RemoveAt(TrackedLastFocussedControlsCount);
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		if (_firstLoad)
		{
			FocusNamedControls();
			_firstLoad = false;
		}

		if (_focusLastOnLoaded)
		{
			FocusLast();
			_focusLastOnLoaded = false;
		}
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (DataContext == null)
			return;

		var viewModel = (ViewModel)DataContext;
		viewModel.NavigatorFocusReturned += (s, e) => FocusLast();

		viewModel.UserInterfaceFocusReturned += (s, e) =>
		{
			FocusLast();
			TrackFocussedControls = true;
		};
		viewModel.UserInterfaceFocusLost += (s, e) => { TrackFocussedControls = false; };
	}

	protected virtual void FocusLast()
	{
		if (!IsLoaded)
		{
			_focusLastOnLoaded = true;
			return;
		}

		if (_lastFocussedControls.Count == 0)
			return;

		var topLevel = TopLevel.GetTopLevel(_lastFocussedControls[0].Control);
		topLevel?.FocusManager?.ClearFocus();

		foreach (var focussableControl in _lastFocussedControls)
			if (focussableControl.TryFocus())
				return;
	}

	public void FocusNamedControls()
	{
		Control? focusableControl = null;
		var count = 0;

		do
		{
			focusableControl = this.FindControl<Control>($"initialFocus{count}");

			if (focusableControl != null && focusableControl.Focusable && focusableControl.IsEffectivelyEnabled)
				break;

			count++;
		} while (focusableControl != null);

		focusableControl?.Focus(NavigationMethodBasedOnMouseOrKey);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);

		if (e.Handled || e.KeyModifiers != KeyModifiers.None || e.Source is not InputElement inputElement)
			return;

		IInputElement? nextFocus = null;
		switch (e.Key)
		{
			case Key.Up:
				nextFocus = KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Up);
				break;
			case Key.Down:
				nextFocus = KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Down);
				break;
			case Key.Left:
				nextFocus = KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Left);
				break;
			case Key.Right:
				nextFocus = KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Right);
				break;
		}

		if (nextFocus != null && nextFocus.Focusable)
		{
			nextFocus.Focus(NavigationMethodBasedOnMouseOrKey);
			AudioManager.Instance?.Play(this, AudioManager.Sound.UISelect, AudioManager.Bus.UI);
			e.Handled = true;
		}
		else
		{
			var topLevel = TopLevel.GetTopLevel(inputElement);

			if (_previousNavigationMethod == NavigationMethod.Unspecified)
			{
				topLevel.FocusManager.ClearFocus();
				inputElement.Focus(NavigationMethodBasedOnMouseOrKey);
			}
		}

		_previousNavigationMethod = NavigationMethodBasedOnMouseOrKey;
	}
}
