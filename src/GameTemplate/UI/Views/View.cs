using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using GameTemplate.Main;
using GameTemplate.UI.ViewModels;
using static GameTemplate.UI.Utilities;

namespace GameTemplate.UI.Views;

public class FocussedControl(Control control)
{
	public Control Control { get; } = control;

	public virtual bool TryFocus() => Control.Focus(NavigationMethodBasedOnMouseOrKey);
}

public class FocussedItemsControl : FocussedControl
{
	private readonly int _containerIndex;

	public FocussedItemsControl(Control item, ItemsControl itemsControl) : base(itemsControl)
	{
		if (item.DataContext == null) return;
		var container = itemsControl.ContainerFromItem(item.DataContext);
		if (container != null) _containerIndex = itemsControl.IndexFromContainer(container);
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
	private readonly List<FocussedControl> _lastFocussedControls = [];

	private bool _firstLoad = true;
	private bool _focusLastOnLoaded;
	private NavigationMethod _previousNavigationMethod = NavigationMethod.Unspecified;

	private protected bool trackFocussedControls = true;
	private static int TrackedLastFocussedControlsCount => 5;

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		_previousNavigationMethod = NavigationMethodBasedOnMouseOrKey;

		base.OnGotFocus(e);

		if (!IsLoaded || !trackFocussedControls || e.Source is not Control { IsFocused: true } control) return;
		var itemsControl = control.FindAncestorOfType<ItemsControl>();
		if (itemsControl != null)
			_lastFocussedControls.Insert(0, new FocussedItemsControl(control, itemsControl));
		else
			_lastFocussedControls.Insert(0, new FocussedControl(control));

		if (_lastFocussedControls.Count > TrackedLastFocussedControlsCount)
			_lastFocussedControls.RemoveAt(TrackedLastFocussedControlsCount);
	}

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnLoaded(RoutedEventArgs e)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		base.OnLoaded(e);

		if (_firstLoad)
		{
			FocusNamedControls();
			_firstLoad = false;
		}

		if (!_focusLastOnLoaded) return;
		FocusLast();
		_focusLastOnLoaded = false;
	}

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnInitialized()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		base.OnInitialized();

		if (DataContext == null)
			return;

		var viewModel = (ViewModel)DataContext;
		viewModel.NavigatorFocusReturned += (_, _) => FocusLast();

		viewModel.UserInterfaceFocusReturned += (_, _) =>
		{
			FocusLast();
			trackFocussedControls = true;
		};
		viewModel.UserInterfaceFocusLost += (_, _) => { trackFocussedControls = false; };
	}

	[Obsolete("Obsolete")]
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

		if (_lastFocussedControls.Any(focussableControl => focussableControl.TryFocus()))
		{
		}
	}

	private void FocusNamedControls()
	{
		Control? focusableControl;
		var count = 0;

		do
		{
			focusableControl = this.FindControl<Control>($"initialFocus{count}");

			if (focusableControl is { Focusable: true, IsEffectivelyEnabled: true })
				break;

			count++;
		} while (focusableControl != null);

		focusableControl?.Focus(NavigationMethodBasedOnMouseOrKey);
	}

	[Obsolete("Obsolete")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	protected override void OnKeyDown(KeyEventArgs e)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		base.OnKeyDown(e);

		if (e.Handled || e.KeyModifiers != KeyModifiers.None || e.Source is not InputElement inputElement)
			return;

		var nextFocus = e.Key switch
		{
			Key.Up => KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Up),
			Key.Down => KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Down),
			Key.Left => KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Left),
			Key.Right => KeyboardNavigationHandler.GetNext(inputElement, NavigationDirection.Right),
			_ => null
		};

		if (nextFocus is { Focusable: true })
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
				topLevel?.FocusManager?.ClearFocus();
				inputElement.Focus(NavigationMethodBasedOnMouseOrKey);
			}
		}

		_previousNavigationMethod = NavigationMethodBasedOnMouseOrKey;
	}
}
