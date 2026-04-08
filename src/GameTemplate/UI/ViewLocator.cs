using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GameTemplate.UI.ViewModels;

namespace GameTemplate.UI;

/// <summary>
///     Caches views for unique ViewModel instances. <br />
///     That means that returning to an existing ViewModel by closing a ViewModel <br />
///     in a NavigatorViewModel will keep using the same view (allowing for retained focus).
/// </summary>
public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, object> _viewModelTypeToInstance = new();
    private readonly Dictionary<Type, Control> _viewModelTypeToView = new();

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var type = param.GetType();
        if (!_viewModelTypeToInstance.TryAdd(type, param) && _viewModelTypeToInstance[type] == param)
            return _viewModelTypeToView[type];

        var view = CreateView(param);
        _viewModelTypeToView[type] = view;
        _viewModelTypeToInstance[type] = param;

        return view;
    }

    public bool Match(object? data)
    {
        return data is ViewModel;
    }

    private Control CreateView(object? param)
    {
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null) return (Control)Activator.CreateInstance(type)!;

        return new TextBlock { Text = "Not Found: " + name };
    }
}
