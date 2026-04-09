using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace GameTemplate.Main;

public partial class AudioManager : Node
{
	public enum Bus
	{
		// ReSharper disable InconsistentNaming
		Master = 0,
		Music = 1,
		SFX = 2,

		UI = 3
		// ReSharper restore InconsistentNaming
	}

	public enum Sound
	{
		// ReSharper disable once InconsistentNaming
		UISelect
	}

	private const int InitialAudioPlayerCount = 20;

	private readonly HashSet<AudioStreamPlayer> _activeAudioPlayers = [];
	private readonly Queue<AudioStreamPlayer> _audioPlayerQueue = new();

	// ReSharper disable once CollectionNeverUpdated.Local
	private readonly Dictionary<Bus, StringName> _busStringNames = new();

	private readonly Dictionary<Sound, AudioStream> _soundToStream = new()
	{
		{ Sound.UISelect, ResourceLoader.Load<AudioStream>("res://Audio/select.wav") }
	};

	private int _availableAudioPlayers;
	public static AudioManager? Instance { get; private set; }

	public bool DebugWriteAudioPlayback { get; set; }

	public static int GetBusLinearEnergyPercentage(Bus bus) => Mathf.RoundToInt(100 * GetBusLinearEnergy(bus));

	public static float GetBusLinearEnergy(Bus bus) => Mathf.DbToLinear(AudioServer.GetBusVolumeDb((int)bus));

	public static void UpdateBusDbLevelFromLinear(Bus bus, int linearEnergyPercentage)
	{
		AudioServer.SetBusVolumeDb((int)bus, Mathf.LinearToDb(linearEnergyPercentage / 100f));
	}

	public static void UpdateBusDbLevelFromLinear(Bus bus, float linearEnergy)
	{
		AudioServer.SetBusVolumeDb((int)bus, Mathf.LinearToDb(linearEnergy));
	}

	private void AddAudioPlayers(int count)
	{
		for (var i = 0; i < count; i++)
		{
			var audioPlayer = new AudioStreamPlayer();
			AddChild(audioPlayer);
			_audioPlayerQueue.Enqueue(audioPlayer);
			_availableAudioPlayers++;

			audioPlayer.Finished += () =>
			{
				_activeAudioPlayers.Remove(audioPlayer);
				_audioPlayerQueue.Enqueue(audioPlayer);
				_availableAudioPlayers++;
			};
		}
	}

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Instance = this;
		AddAudioPlayers(InitialAudioPlayerCount);
	}

	public void ResumeAllAudio()
	{
		PauseOrResumeAudioPlayersBus(false, [Bus.Master, Bus.Music, Bus.UI, Bus.SFX]);
	}

	public void PauseOrResumeAudioPlayersBus(bool pause, HashSet<Bus> busses)
	{
		foreach (var audioPlayer in _activeAudioPlayers)
			if (Enum.TryParse(audioPlayer.Bus.ToString(), out Bus bus)
				&& busses.Contains(bus))
				audioPlayer.StreamPaused = pause;
	}

	public void Play(object sender, Sound sound, Bus bus = Bus.Master, float volumeDbOffset = 0, float pitchScale = 1)
	{
		if (DebugWriteAudioPlayback)
			Debug.WriteLine($"AudioManager playing sound \"{sound}\", {bus} bus\n" +
							$"Sender: {sender} - {Time.GetTicksMsec()}ms\n");

		if (_availableAudioPlayers == 0) AddAudioPlayers(1);

		var audioPlayer = _audioPlayerQueue.Dequeue();
		_activeAudioPlayers.Add(audioPlayer);
		_availableAudioPlayers--;

		if (!_busStringNames.TryGetValue(bus, out var busName)) busName = bus.ToString();

		audioPlayer.Bus = busName;
		audioPlayer.Stream = _soundToStream[sound];
		audioPlayer.VolumeDb = volumeDbOffset;
		audioPlayer.PitchScale = pitchScale;
		audioPlayer.Play();
	}
}
