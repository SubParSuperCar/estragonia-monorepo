using System;
using Avalonia;
using Avalonia.Controls;
using EstragoniaTemplate.Main;

namespace EstragoniaTemplate.UI.Controls;

public class AudioButton : Button
{
    public static readonly StyledProperty<string> ClickSoundProperty =
        AvaloniaProperty.Register<AudioButton, string>(nameof(ClickSound), string.Empty);

    public string ClickSound
    {
        get => GetValue(ClickSoundProperty);
        set => SetValue(ClickSoundProperty, value);
    }

    protected override void OnClick()
    {
        if (!string.IsNullOrEmpty(ClickSound) && Enum.TryParse<AudioManager.Sound>(ClickSound, out var sound))
            AudioManager.Instance?.Play(this, sound, AudioManager.Bus.UI);

        base.OnClick();
    }
}
