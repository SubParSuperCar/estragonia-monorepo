using System;
using Avalonia;
using Estragonia;
using Godot;

namespace GameTemplate.UI;

public partial class AvaloniaLoader : Node
{
	private const double MouseHidingDelay = 3;
	private double _elapsedMouseUntouchedSinceButtonPress;
	private double _elapsedSinceLastResize;
	private double _pendingUIScaling = 1;
	private double _resizeGracePeriod = 0.1;
	private float _resolutionTargetHeight = 540;

	private float _resolutionTargetWidth = 960;

	private double _uiScaling;

	private double _uiScalingOption = 1;
	public static AvaloniaLoader Instance { get; set; } = null!;

	public static bool LastPressedInputWasMouseClick { get; private set; } = true;
	public static bool MouseMovedSinceLastButtonPress { get; private set; }

	public double UIScalingOption
	{
		get => _uiScalingOption;
		set
		{
			_uiScalingOption = value;
			_pendingUIScaling = ComputeUIScale(GetWindow());
			_elapsedSinceLastResize = _resizeGracePeriod;
		}
	}

	public double UIScaling
	{
		get => _uiScaling;
		private set
		{
			_uiScaling = value;
			UIScaleChanged?.Invoke(this, _uiScaling);
		}
	}

	public event EventHandler<double>? UIScaleChanged;

	private double ComputeUIScale(Window window)
	{
		var xRatio = window.Size.X / _resolutionTargetWidth;
		var yRatio = window.Size.Y / _resolutionTargetHeight;

		return Mathf.Min(xRatio, yRatio) * UIScalingOption;
	}

	public override void _Ready()
	{
		AppBuilder
			.Configure<App>()
			.UseGodot()
			.SetupWithoutStarting();

		var window = GetWindow();
		window.SizeChanged += () =>
		{
			_pendingUIScaling = ComputeUIScale(window);
			_elapsedSinceLastResize = 0;
		};

		UIScaling = ComputeUIScale(window);
		_pendingUIScaling = UIScaling;

		ProcessMode = ProcessModeEnum.Always;
		Instance = this;
	}

	public override void _Process(double delta)
	{
		_elapsedSinceLastResize += delta;
		if (!MouseMovedSinceLastButtonPress)
		{
			_elapsedMouseUntouchedSinceButtonPress += delta;

			if (_elapsedMouseUntouchedSinceButtonPress > MouseHidingDelay)
			{
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);

				// So that the mouse no longer causes hover style on UI
				var ie = new InputEventMouseButton
				{
					ButtonIndex = MouseButton.Left,
					Position = -Vector2I.One
				};
				Input.ParseInputEvent(ie);
			}
		}

		if (UIScaling != _pendingUIScaling && _elapsedSinceLastResize > _resizeGracePeriod)
			UIScaling = _pendingUIScaling;
	}

	public override void _Input(InputEvent @event)
	{
		using (@event)
		{
			if (@event is InputEventMouseMotion mouseMotion)
			{
				if (mouseMotion.Velocity.Length() < 50)
					return;

				MouseMovedSinceLastButtonPress = true;
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
			}
			else if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
			{
				LastPressedInputWasMouseClick = true;
				MouseMovedSinceLastButtonPress = true;
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
			}
			else if ((@event is InputEventKey key && key.Pressed) ||
			         (@event is InputEventJoypadButton joypadButton && joypadButton.Pressed))
			{
				if (LastPressedInputWasMouseClick || MouseMovedSinceLastButtonPress)
					_elapsedMouseUntouchedSinceButtonPress = 0;

				LastPressedInputWasMouseClick = false;
				MouseMovedSinceLastButtonPress = false;
			}
		}
	}
}
