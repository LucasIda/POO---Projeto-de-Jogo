using Godot;
using System;
using System.Collections.Generic;

public partial class MusicManager : Node
{
	private List<string> mp3Files = new List<string>
	{
		"res://Songs/Better Days.mp3",
		"res://Songs/Dreaming.mp3",
		"res://Songs/Kingdom in Blue.mp3",
		"res://Songs/Moon.mp3"
	};

	private AudioStreamPlayer audioPlayer;
	private bool isMusicPlaying = false;
	private int currentTrackIndex = 0;
	private int lastPlayedIndex = -1; // Evita repetir a mesma música

	[Export] public float MusicVolumeDB { get; set; } = -24.0f;

	public override void _Ready()
	{
		if (audioPlayer == null)
		{
			audioPlayer = new AudioStreamPlayer();
			audioPlayer.VolumeDb = MusicVolumeDB;
			AddChild(audioPlayer);
			audioPlayer.Finished += OnAudioFinished;
		}

		if (!isMusicPlaying)
		{
			PlayRandomFirstTrack(); 
		}
	}

	// Toca uma música ALEATÓRIA como primeira
	private void PlayRandomFirstTrack()
	{
		Random rand = new Random();
		currentTrackIndex = rand.Next(mp3Files.Count);
		lastPlayedIndex = currentTrackIndex;

		LoadAndPlayTrack(currentTrackIndex);
		isMusicPlaying = true;

		GD.Print($"Iniciando com música aleatória: {System.IO.Path.GetFileName(mp3Files[currentTrackIndex])}");
	}

	// Quando uma música acaba → próxima em sequência
	private void OnAudioFinished()
	{
		GD.Print("Música acabou, tocando próxima...");
		PlayNextTrack();
	}

	// Toca a próxima música (sequencial, sem repetir a última)
	private void PlayNextTrack()
	{
		int nextIndex;
		do
		{
			currentTrackIndex = (currentTrackIndex + 1) % mp3Files.Count;
			nextIndex = currentTrackIndex;
		}
		while (mp3Files.Count > 1 && nextIndex == lastPlayedIndex); // Evita repetir

		lastPlayedIndex = nextIndex;
		LoadAndPlayTrack(nextIndex);
	}

	// Carrega e toca faixa
	private void LoadAndPlayTrack(int index)
	{
		string trackPath = mp3Files[index];
		
		if (ResourceLoader.Exists(trackPath))
		{
			var stream = GD.Load<AudioStream>(trackPath);
			audioPlayer.Stream = stream;
			audioPlayer.Play();
			
			GD.Print($"Tocando: {System.IO.Path.GetFileName(trackPath)} (Volume: {MusicVolumeDB:F1} dB)");
		}
		else
		{
			GD.PrintErr($"Arquivo não encontrado: {trackPath}");
			PlayNextTrack(); // Pula se faltar
		}
	}

	// Ajusta volume
	public void SetVolumeDb(float volumeDb)
	{
		MusicVolumeDB = volumeDb;
		if (audioPlayer != null)
		{
			audioPlayer.VolumeDb = volumeDb;
			GD.Print($"Volume ajustado para: {volumeDb:F1} dB");
		}
	}

	public void StopMusic()
	{
		if (audioPlayer != null)
		{
			audioPlayer.Stop();
			isMusicPlaying = false;
		}
	}

	public void TogglePause()
	{
		if (audioPlayer != null)
		{
			audioPlayer.StreamPaused = !audioPlayer.StreamPaused;
			GD.Print(audioPlayer.StreamPaused ? "Música pausada" : "Música retomada");
		}
	}

	public void NextTrack()
	{
		if (audioPlayer != null && audioPlayer.Playing)
		{
			audioPlayer.Stop();
		}
		PlayNextTrack();
	}

	public float GetVolumeDb() => MusicVolumeDB;
}
