using Avalonia;
using Estragonia;
using Godot;

namespace HelloWorld;

public partial class AvaloniaLoader : Node
{
    public override void _Ready()
    {
        AppBuilder
            .Configure<App>()
            .UseGodot()
            .SetupWithoutStarting();
    }
}
