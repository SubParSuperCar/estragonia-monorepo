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
	private double _pendingUiScaling = 1;
	private double _resizeGracePeriod = 0.1;
	private float _resolutionTargetHeight = 540;

	private float _resolutionTargetWidth = 960;

	private double _uiScaling;

	private double _uiScalingOption = 1;
	public static AvaloniaLoader Instance { get; private set; } = null!;

	public static bool LastPressedInputWasMouseClick { get; private set; } = true;
	private static bool MouseMovedSinceLastButtonPress { get; set; }

	public double UiScalingOption
	{
		get => _uiScalingOption;
		set
		{
			_uiScalingOption = value;
			_pendingUiScaling = ComputeUiScale(GetWindow());
			_elapsedSinceLastResize = _resizeGracePeriod;
		}
	}

	public double UiScaling
	{
		get => _uiScaling;
		private set
		{
			_uiScaling = value;
			UiScaleChanged?.Invoke(this, _uiScaling);
		}
	}

	public event EventHandler<double>? UiScaleChanged;

	private double ComputeUiScale(Window window)
	{
		var xRatio = window.Size.X / _resolutionTargetWidth;
		var yRatio = window.Size.Y / _resolutionTargetHeight;

		return Mathf.Min(xRatio, yRatio) * UiScalingOption;
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
			_pendingUiScaling = ComputeUiScale(window);
			_elapsedSinceLastResize = 0;
		};

		UiScaling = ComputeUiScale(window);
		_pendingUiScaling = UiScaling;

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

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (UiScaling != _pendingUiScaling && _elapsedSinceLastResize > _resizeGracePeriod)
			UiScaling = _pendingUiScaling;
	}

	public override void _Input(InputEvent @event)
	{
		using (@event)
		{
			switch (@event)
			{
				case InputEventMouseMotion mouseMotion when mouseMotion.Velocity.Length() < 50:
					return;
				case InputEventMouseMotion:
					MouseMovedSinceLastButtonPress = true;
					DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
					break;
				case InputEventMouseButton { Pressed: true }:
					LastPressedInputWasMouseClick = true;
					MouseMovedSinceLastButtonPress = true;
					DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
					break;
				case InputEventKey { Pressed: true }:
				case InputEventJoypadButton { Pressed: true }:
				{
					if (LastPressedInputWasMouseClick || MouseMovedSinceLastButtonPress)
						_elapsedMouseUntouchedSinceButtonPress = 0;

					LastPressedInputWasMouseClick = false;
					MouseMovedSinceLastButtonPress = false;
					break;
				}
			}
		}
	}
}
