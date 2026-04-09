using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Platform;
using GameTemplate.UI.Controls;
using Godot;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace GameTemplate.UI;

public static class Utilities
{
	public enum TransitionType
	{
		// ReSharper disable once InconsistentNaming
		Fade
	}

	public static NavigationMethod NavigationMethodBasedOnMouseOrKey
		=> AvaloniaLoader.LastPressedInputWasMouseClick ? NavigationMethod.Unspecified : NavigationMethod.Directional;

	public static Bitmap LoadImageFromResource(Uri resourceUri) => new(AssetLoader.Open(resourceUri));

	/// <summary>
	///     Note: rounds to milliseconds.
	/// </summary>
	public static TimeSpan TimeSpanFromSeconds(float seconds) => new(0, 0, 0, 0, Mathf.RoundToInt(seconds * 1000));

	public static PageTransitionWithDuration CreatePageTransition(TransitionType transitionType, float durationSeconds)
	{
		var duration = TimeSpanFromSeconds(durationSeconds);

		IPageTransition transition;
		switch (transitionType)
		{
			default:
			case TransitionType.Fade:
				transition = new CompositePageTransition
				{
					PageTransitions =
					[
						new SequentialFade(duration),
						new TransitionDisableFromControl(duration)
					]
				};
				break;
		}

		return new PageTransitionWithDuration(transition, durationSeconds);
	}

	public class PageTransitionWithDuration(IPageTransition pageTransition, float durationSeconds) : IPageTransition
	{
		private IPageTransition PageTransition { get; } = pageTransition;
		private float Duration { get; } = durationSeconds;

		public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken) =>
			PageTransition.Start(from, to, forward, cancellationToken);

		public async Task StartToEnd()
		{
			await Task.Delay(TimeSpanFromSeconds(Duration));
		}
	}
}
