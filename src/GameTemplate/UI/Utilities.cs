using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Platform;
using Godot;
using Template.UI.Controls;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Template.UI;

public static class Utilities
{
    public enum TransitionType
    {
        Fade
    }

    public static NavigationMethod NavigationMethodBasedOnMouseOrKey
        => AvaloniaLoader.LastPressedInputWasMouseClick ? NavigationMethod.Unspecified : NavigationMethod.Directional;

    public static Bitmap LoadImageFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }

    /// <summary>
    ///     Note: rounds to milliseconds.
    /// </summary>
    public static TimeSpan TimeSpanFromSeconds(float seconds)
    {
        return new TimeSpan(0, 0, 0, 0, Mathf.RoundToInt(seconds * 1000));
    }

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
                    PageTransitions = new List<IPageTransition>
                    {
                        new SequentialFade(duration),
                        new TransitionDisableFromControl(duration)
                    }
                };
                break;
        }

        return new PageTransitionWithDuration(transition, durationSeconds);
    }

    public class PageTransitionWithDuration : IPageTransition
    {
        public PageTransitionWithDuration(IPageTransition pageTransition, float durationSeconds)
        {
            PageTransition = pageTransition;
            Duration = durationSeconds;
        }

        public IPageTransition PageTransition { get; set; }
        public float Duration { get; set; }

        public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
        {
            return PageTransition.Start(from, to, forward, cancellationToken);
        }

        public async Task StartToEnd()
        {
            await Task.Delay(TimeSpanFromSeconds(Duration));
        }
    }
}
