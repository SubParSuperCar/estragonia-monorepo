using CommunityToolkit.Mvvm.ComponentModel;
using Godot;
using static GameTemplate.Main.AudioManager;

namespace GameTemplate.UI.Models;

public class AudioOptions : ObservableObject
{
	public int MasterLevel
	{
		get;
		init => field = Mathf.Clamp(0, value, 100);
	} = 100;

	public int MusicLevel
	{
		get;
		init => field = Mathf.Clamp(0, value, 100);
	} = 100;

	public int SoundEffectsLevel
	{
		get;
		init => field = Mathf.Clamp(0, value, 100);
	} = 100;

	public int InterfaceLevel
	{
		get;
		init => field = Mathf.Clamp(0, value, 100);
	} = 100;

	public void Apply()
	{
		UpdateBusDbLevelFromLinear(Bus.Master, MasterLevel);
		UpdateBusDbLevelFromLinear(Bus.Music, MusicLevel);
		UpdateBusDbLevelFromLinear(Bus.SFX, SoundEffectsLevel);
		UpdateBusDbLevelFromLinear(Bus.UI, InterfaceLevel);
	}
}
