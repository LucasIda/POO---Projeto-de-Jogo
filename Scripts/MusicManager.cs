using Godot;
using System;
using System.Collections.Generic;

public partial class MusicManager : Node
{
    // Lista de arquivos MP3
    private List<string> mp3Files = new List<string>
    {
        "res://Songs/Better Days.mp3",
        "res://Songs/Dreaming.mp3",
        "res://Songs/Kingdom in Blue.mp3",
        "res://Songs/Moon.mp3"
    };

    // Referência ao AudioStreamPlayer
    private AudioStreamPlayer audioPlayer;

    // Variável para garantir que a música não seja reiniciada
    private bool isMusicPlaying = false;

    public override void _Ready()
    {
        // Se o MusicManager ainda não tiver um AudioStreamPlayer, cria um.
        if (audioPlayer == null)
        {
            audioPlayer = new AudioStreamPlayer();
            AddChild(audioPlayer); // Adiciona o AudioStreamPlayer como filho
        }
        // Toca um arquivo MP3 aleatório se a música ainda não estiver tocando
        if (!isMusicPlaying)
        {
            PlayRandomAudio();
        }
    }

    // Método para tocar um áudio aleatório
    private void PlayRandomAudio()
    {
        // Gerar um número aleatório para escolher o arquivo
        Random random = new Random();
        int index = random.Next(mp3Files.Count);

        // Carregar o arquivo de áudio escolhido
        var stream = GD.Load<AudioStream>(mp3Files[index]);

        // Atribuir o stream ao AudioStreamPlayer
        audioPlayer.Stream = stream;

        // Configurar o loop de áudio
        audioPlayer.Play();

        // Marcar que a música está tocando
        isMusicPlaying = true;
    }

    // Método para parar a música (usado quando o jogador clica em "Sair")
    public void StopMusic()
    {
        audioPlayer.Stop();
        isMusicPlaying = false;
    }
}
