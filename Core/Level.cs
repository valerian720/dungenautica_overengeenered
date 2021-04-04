using System;
using Godot;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.World;

namespace SibGameJam2021.Core
{
    public class Level : Node2D
    {
        private static AudioStream battleMusic1 = ResourceLoader.Load<AudioStream>("res://Assets/Music/fight_music.wav");
        private static AudioStream battleMusic2 = ResourceLoader.Load<AudioStream>("res://Assets/Music/fight_music_2.wav");
        private static AudioStream openGateSound = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/gate_open.wav");

        private Gate _gate;
        private SpawnManager _spawnManager;

        private AudioStreamPlayer2D audioPlayer = null;
        public Navigation2D Navigation2D { get; private set; } = null;

        public override void _Ready()
        {
            _gate = GetNode<Gate>("YSort/Gate");
            _spawnManager = GetNode<SpawnManager>("YSort/SpawnManager");
            Navigation2D = GetNode<Navigation2D>("Navigation2D");

            _spawnManager.Connect(nameof(SpawnManager.LevelCleared), this, nameof(OnLevelCleared));

            audioPlayer = new AudioStreamPlayer2D();
            audioPlayer.Stream = new Random().Next(2) == 0 ? battleMusic1 : battleMusic2;
            audioPlayer.Playing = true;

            AddChild(audioPlayer);
        }

        public void RemovePlayer()
        {
            _spawnManager.RemovePlayer();
        }

        public void SpawnPlayer()
        {
            _spawnManager.SpawnPlayer();
        }

        private void OnLevelCleared()
        {
            _gate.Open();

            audioPlayer.Stream = openGateSound;
            audioPlayer.Playing = true;
        }
    }
}