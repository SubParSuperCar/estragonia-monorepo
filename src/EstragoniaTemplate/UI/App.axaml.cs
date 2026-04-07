using Avalonia;
using Avalonia.Markup.Xaml;

namespace EstragoniaTemplate.UI;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
