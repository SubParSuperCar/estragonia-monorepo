using CommunityToolkit.Mvvm.ComponentModel;
using Godot;
using Template.Main;
using static Template.Main.AudioManager;

namespace Template.UI.Models;

public class AudioOptions : ObservableObject
{
    private int _interfaceLevel = 100;
    private int _masterLevel = 100;

    private int _musicLevel = 100;

    private int _soundEffectsLevel = 100;

    public int MasterLevel
    {
        get => _masterLevel;
        set => _masterLevel = Mathf.Clamp(0, value, 100);
    }

    public int MusicLevel
    {
        get => _musicLevel;
        set => _musicLevel = Mathf.Clamp(0, value, 100);
    }

    public int SoundEffectsLevel
    {
        get => _soundEffectsLevel;
        set => _soundEffectsLevel = Mathf.Clamp(0, value, 100);
    }

    public int InterfaceLevel
    {
        get => _interfaceLevel;
        set => _interfaceLevel = Mathf.Clamp(0, value, 100);
    }

    public void Apply()
    {
        UpdateBusDbLevelFromLinear(AudioManager.Bus.Master, MasterLevel);
        UpdateBusDbLevelFromLinear(AudioManager.Bus.Music, MusicLevel);
        UpdateBusDbLevelFromLinear(AudioManager.Bus.SFX, SoundEffectsLevel);
        UpdateBusDbLevelFromLinear(AudioManager.Bus.UI, InterfaceLevel);
    }
}
