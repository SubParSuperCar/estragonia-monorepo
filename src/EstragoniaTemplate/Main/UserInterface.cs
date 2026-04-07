using System;
using EstragoniaTemplate.UI;
using EstragoniaTemplate.UI.ViewModels;
using EstragoniaTemplate.UI.Views;
using Godot;
using JLeb.Estragonia;

namespace EstragoniaTemplate.Main;

public partial class UserInterface : AvaloniaControl, IFocussable
{
    private bool _inputEnabled = true;

    private KeyRepeater? _keyRepeater;

    [Export(
        hintString:
        "Determines whether or not this control will block or allow mouse clicks through it's transparent areas (see https://github.com/MrJul/Estragonia/issues/14).")]
    public bool AllowTransparentClickThrough { get; set; } = true;

    public bool InputEnabled
    {
        get => _inputEnabled;
        set
        {
            _inputEnabled = value;
            if (!value) _keyRepeater?.ClearRepeatingAndBlockedInput();
        }
    }

    public ViewModel? CurrentViewModel => MainViewModel?.CurrentViewModel;

    public MainViewModel? MainViewModel { get; private set; }

    public new void GrabFocus()
    {
        FocusMode = FocusModeEnum.All;
        base.GrabFocus();
        CurrentViewModel?.OnUserInterfaceFocusReturned();
    }

    public new void ReleaseFocus()
    {
        FocusMode = FocusModeEnum.None;
        base.ReleaseFocus();
        CurrentViewModel?.OnUserInterfaceFocusLost();
        _keyRepeater?.ClearRepeatingAndBlockedInput();
    }

    public event EventHandler<InputEvent>? InputEventReceived;

    public override bool _HasPoint(Vector2 point)
    {
        if (!AllowTransparentClickThrough)
            return true;

        return base._HasPoint(point);
    }

    private void OnUIScaleChanged(object? sender, double scale)
    {
        RenderScaling = scale;
    }

    public override void _Ready()
    {
        RenderScaling = AvaloniaLoader.Instance.UIScaling;
        AvaloniaLoader.Instance.UIScaleChanged += OnUIScaleChanged;
        base._Ready();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete) AvaloniaLoader.Instance.UIScaleChanged -= OnUIScaleChanged;
    }

    public void Initialize(MainViewModel mainViewModel, KeyRepeater keyRepeater, ViewModel? initialViewModel = null)
    {
        MainViewModel = mainViewModel;
        _keyRepeater = keyRepeater;

        if (initialViewModel != null) mainViewModel.NavigateTo(initialViewModel);

        Control = new MainView
        {
            DataContext = mainViewModel
        };
    }

    public override void _GuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (!InputEnabled || (_keyRepeater != null && _keyRepeater.Input(@event)))
                return;

            InputEventReceived?.Invoke(this, @event);

            if (@event is InputEventKey key && key.PhysicalKeycode == Key.Space)
            {
                key.Keycode = Key.Enter;
                key.PhysicalKeycode = Key.Enter;
            }

            base._GuiInput(@event);
        }
    }

    public void ForceGuiInput(InputEvent @event)
    {
        using (@event)
        {
            InputEventReceived?.Invoke(this, @event);

            base._GuiInput(@event);
        }
    }

    public override void _Process(double delta)
    {
        if (HasFocus()) _keyRepeater?.Process((float)delta, this);

        base._Process(delta);
    }
}
