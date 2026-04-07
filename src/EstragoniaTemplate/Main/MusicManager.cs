using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using static Template.Main.AudioManager;

namespace Template.Main;

public partial class MusicManager : Node
{
    public enum Music
    {
        MainMenu
    }

    private readonly Dictionary<Music, AudioStream> _musicToStream = new()
    {
        { Music.MainMenu, ResourceLoader.Load<AudioStream>("res://Audio/Music/LobbyTime.mp3") }
    };

    private AudioStreamPlayer _musicPlayer = new()
    {
        Bus = Bus.Music.ToString()
    };

    private Tween? _musicTween;
    private Music? _nextMusic;
    public static MusicManager? Instance { get; private set; }

    public bool DebugWriteMusicPlayback { get; set; } = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
        AddChild(_musicPlayer);
    }

    /// <summary>
    ///     If _nextMusic is not null, then that music track will be played after the fade out.
    /// </summary>
    public void FadeOutMusic(float durationSeconds)
    {
        var originalMusicLevel = GetBusLinearEnergy(Bus.Music);
        var tween = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        tween.TweenMethod(
            Callable.From((float linearEnergy) => { UpdateBusDbLevelFromLinear(Bus.Music, linearEnergy); }),
            originalMusicLevel, 0, durationSeconds);

        _musicTween = tween;

        tween.Finished += async () =>
        {
            _musicTween = null;
            _musicPlayer.Stop();

            await Task.Delay(50);

            UpdateBusDbLevelFromLinear(Bus.Music, originalMusicLevel);

            if (_nextMusic != null)
            {
                _musicPlayer.Stream = _musicToStream[_nextMusic.Value];
                _musicPlayer.Play();
                _nextMusic = null;
            }
        };
    }

    public void StopMusic()
    {
        _musicPlayer.Stop();
    }

    public void PlayMusic(object sender, Music music)
    {
        if (DebugWriteMusicPlayback)
            Debug.WriteLine($"AudioManager playing music \"{music}\"\n" +
                            $"Sender: {sender} - {Time.GetTicksMsec()}ms\n");

        if (!_musicPlayer.Playing)
        {
            _musicPlayer.Stream = _musicToStream[music];
            _musicPlayer.Play();
            return;
        }

        _nextMusic = music;

        if (_musicTween != null)
            return;

        FadeOutMusic(4);
    }
}
