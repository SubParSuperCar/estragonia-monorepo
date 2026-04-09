using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;

namespace GameTemplate.UI.Controls;

public class TransitionDisableFromControl(TimeSpan duration) : IPageTransition
{
	private TimeSpan Duration { get; } = duration;

	public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		if (from is not InputElement fromElement || to is not InputElement) return;

		fromElement.IsHitTestVisible = false;

		if (cancellationToken.IsCancellationRequested)
		{
			OnTransitionEnd();
			return;
		}

		await Task.Delay(Duration, CancellationToken.None);

		OnTransitionEnd();
		return;

		void OnTransitionEnd()
		{
			fromElement.IsHitTestVisible = true;
		}
	}
}
