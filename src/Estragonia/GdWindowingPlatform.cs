using System;
using Avalonia.Platform;

namespace JLeb.Estragonia;

internal sealed class GodotWindowingPlatform : IWindowingPlatform
{
    public IWindowImpl CreateWindow()
    {
        throw CreateNotImplementedException();
    }

    public IWindowImpl CreateEmbeddableWindow()
    {
        throw CreateNotImplementedException();
    }

    public ITopLevelImpl CreateEmbeddableTopLevel()
    {
        throw CreateNotImplementedException();
    }

    public ITrayIconImpl? CreateTrayIcon()
    {
        return null;
    }

    private static NotImplementedException CreateNotImplementedException()
    {
        return new NotImplementedException("Sub windows aren't implemented yet");
    }
}
