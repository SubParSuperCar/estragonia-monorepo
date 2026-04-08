using System.Diagnostics.CodeAnalysis;
using Avalonia.Skia;
using SkiaSharp;

namespace Estragonia;

/// <summary>A render target that uses an underlying Skia surface.</summary>
internal sealed class GodotSkiaRenderTarget : ISkiaGpuRenderTarget
{
    private readonly GRContext _grContext;
    private readonly double _renderScaling;

    private readonly IGodotSkiaSurface _surface;
    private readonly ISurfaceSynchronizer _synchronizer;

    public GodotSkiaRenderTarget(IGodotSkiaSurface surface, GRContext grContext, ISurfaceSynchronizer synchronizer)
    {
        _renderScaling = surface.RenderScaling;
        _surface = surface;
        _grContext = grContext;
        _synchronizer = synchronizer;
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Doesn't affect correctness")]
    public bool IsCorrupted
        => _surface.IsDisposed || _grContext.IsAbandoned || _renderScaling != _surface.RenderScaling;

    public ISkiaGpuRenderSession BeginRenderingSession()
    {
        return new GodotSkiaGpuRenderSession(_surface, _grContext, _synchronizer);
    }

    public void Dispose()
    {
    }
}
