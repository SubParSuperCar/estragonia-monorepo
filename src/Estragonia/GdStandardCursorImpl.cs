using Avalonia.Platform;
using GdCursorShape = Godot.Control.CursorShape;

namespace Estragonia;

/// <summary>A standard cursor, represented by a <see cref="GdCursorShape" /> enum value.</summary>
internal sealed class GodotStandardCursorImpl : ICursorImpl
{
	public GodotStandardCursorImpl(GdCursorShape cursorShape)
	{
		CursorShape = cursorShape;
	}

	public GdCursorShape CursorShape { get; }

	public void Dispose()
	{
	}

	public override string ToString() => CursorShape.ToString();
}
