using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace GameMenu.UI;

public sealed class ViewLocator : IDataTemplate
{
	private readonly Dictionary<Type, ViewFactory> _viewFactoryByModelType = new()
	{
		[typeof(MainMenuViewModel)] = new ViewFactory(() => new MainMenuView(), true),
		[typeof(DifficultyViewModel)] = new ViewFactory(() => new DifficultyView()),
		[typeof(GameLoadingViewModel)] = new ViewFactory(() => new GameLoadingView()),
		[typeof(GameViewModel)] = new ViewFactory(() => new GameView()),
		[typeof(OptionsViewModel)] = new ViewFactory(() => new OptionsView())
	};

	public bool Match(object? data) => data is ViewModel;

	public Control? Build(object? param)
	{
		if (param?.GetType() is not { } viewModelType)
			return null;

		return _viewFactoryByModelType.TryGetValue(viewModelType, out var viewFactory)
			? viewFactory.GetOrCreateView()
			: CreateViewNotFound(viewModelType);
	}

	private static TextBlock CreateViewNotFound(Type viewModelType) =>
		new()
		{
			Text = $"No view registered for viewmodel type\n{viewModelType}",
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(8.0),
			Foreground = Brushes.Red
		};

	private sealed class ViewFactory(Func<View> createView, bool cached = false)
	{
		private View? _cachedView;

		public View GetOrCreateView() =>
			cached
				? _cachedView ??= createView()
				: createView();
	}
}
