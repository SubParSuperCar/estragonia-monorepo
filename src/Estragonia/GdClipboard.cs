using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Godot;

namespace Estragonia;

/// <summary>An implementation of <see cref="IClipboard" /> that uses Godot clipboard methods.</summary>
internal sealed class GodotClipboard : IClipboard
{
    public Task<string?> GetTextAsync()
    {
        return Task.FromResult<string?>(DisplayServer.ClipboardGet());
    }

    public Task SetTextAsync(string? text)
    {
        DisplayServer.ClipboardSet(text);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        return SetTextAsync(string.Empty);
    }

    public Task SetDataObjectAsync(IDataObject data)
    {
        return Task.CompletedTask;
    }

    public Task SetDataAsync(IAsyncDataTransfer? dataTransfer)
    {
        throw new NotImplementedException();
    }

    public Task FlushAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetFormatsAsync()
    {
        return Task.FromResult(Array.Empty<string>());
    }

    public Task<object?> GetDataAsync(string format)
    {
        return Task.FromResult<object?>(null);
    }

    public Task<IAsyncDataTransfer?> TryGetDataAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IDataObject?> TryGetInProcessDataObjectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IAsyncDataTransfer?> TryGetInProcessDataAsync()
    {
        throw new NotImplementedException();
    }
}
