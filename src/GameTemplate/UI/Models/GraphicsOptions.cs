using System;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace GameTemplate.UI.Models;

public partial class GraphicsOptions : ObservableObject
{
	public GraphicsOptions()
	{
	}

	public GraphicsOptions(GraphicsOptions options)
	{
		SetFromOptions(options);
	}

	[ObservableProperty] public partial float UiScale { get; set; } = 1;

	[ObservableProperty] public partial bool VSync { get; set; }

	[ObservableProperty]
	public partial DisplayServer.WindowMode WindowMode { get; set; } = DisplayServer.WindowMode.Fullscreen;

	public static int MaxFpsLimit => 300;
	public static int MinFpsLimit => 60;

	public int FpsLimit
	{
		get;
		set => SetProperty(ref field, Mathf.Clamp(value, MinFpsLimit, MaxFpsLimit));
	} = 60;

	// ReSharper disable once EventNeverSubscribedTo.Global
	public event EventHandler? Applied;

	public void SetFromOptions(GraphicsOptions options)
	{
		WindowMode = options.WindowMode;
		VSync = options.VSync;
		FpsLimit = options.FpsLimit;
		UiScale = options.UiScale;
	}

	public void Apply()
	{
		Applied?.Invoke(this, EventArgs.Empty);

		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
		DisplayServer.WindowSetVsyncMode(VSync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);

		var currentMode = DisplayServer.WindowGetMode();
		if (WindowMode != DisplayServer.WindowMode.Windowed || currentMode != DisplayServer.WindowMode.Maximized)
			DisplayServer.WindowSetMode(WindowMode);

		AvaloniaLoader.Instance.UiScalingOption = UiScale;

		Engine.MaxFps = 0;
		if (!VSync) Engine.MaxFps = FpsLimit;
	}

	public override bool Equals(object? obj) =>
		obj is GraphicsOptions options &&
		WindowMode == options.WindowMode &&
		VSync == options.VSync &&
		FpsLimit == options.FpsLimit &&
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		UiScale == options.UiScale;

	[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
	public override int GetHashCode() => HashCode.Combine(WindowMode, VSync, FpsLimit, UiScale);
}
