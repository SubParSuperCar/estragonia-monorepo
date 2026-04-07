using Avalonia;
using Avalonia.Controls;

namespace EstragoniaTemplate.UI.Views;

public partial class OptionsView : View
{
    private const int focusInflation = 100;

    public OptionsView()
    {
        InitializeComponent();

        scrollViewer.BringIntoViewOnFocusChange = false;
        scrollViewer.GotFocus += (s, e) =>
        {
            var control = e.Source as Control;
            var inflatedSize = control?.DesiredSize.Inflate(new Thickness(focusInflation)) ?? default;
            control?.BringIntoView(new Rect(-new Point(focusInflation, focusInflation), inflatedSize));
        };
    }
}
