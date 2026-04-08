using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;

namespace GameTemplate.UI.Controls;

public class TransitionDisableFromControl : IPageTransition
{
	public TransitionDisableFromControl() : this(TimeSpan.Zero)
	{
	}

	public TransitionDisableFromControl(TimeSpan duration)
	{
		Duration = duration;
	}

	public TimeSpan Duration { get; set; }

	public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		if (from is not InputElement fromElement || to is not InputElement toElement) return;

		void OnTransitionEnd()
		{
			fromElement.IsHitTestVisible = true;
		}

		fromElement.IsHitTestVisible = false;

		if (cancellationToken.IsCancellationRequested)
		{
			OnTransitionEnd();
			return;
		}

		await Task.Delay(Duration, CancellationToken.None);

		OnTransitionEnd();
	}
}
