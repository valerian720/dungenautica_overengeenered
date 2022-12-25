using Godot;
using System;

namespace SibGameJam2021.Core.Managers
{
    public class SoundManager: Node2D
    {
        private AudioStreamPlayer2D[] deathEnemySoundsBuffer = null;
        private DateTime[] lastPlayedSoundInfo = null;
        private int bufferSize = 16;

        private AudioStreamPlayer _musicAudioPlayer = new AudioStreamPlayer();
        private AudioStreamPlayer _sfxAudioPlayer = new AudioStreamPlayer();

        private AudioStreamPlayer _pewAudioPlayer = new AudioStreamPlayer();
        private AudioStreamPlayer _reloadAudioPlayer = new AudioStreamPlayer();

        // cached sound files

        private static AudioStream _backgroundMusic = ResourceLoader.Load<AudioStream>("res://Assets/Music/background_music.wav");

        private static AudioStream _battleMusic1 = ResourceLoader.Load<AudioStream>("res://Assets/Music/fight_music.wav");
        private static AudioStream _battleMusic2 = ResourceLoader.Load<AudioStream>("res://Assets/Music/fight_music_2.wav");

        private static AudioStream _openGateSound = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/gate_open.wav");
        //
        private static AudioStream _pew1 = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/pew_1.wav");
        private static AudioStream _pew2 = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/pew_2.wav");
        private static AudioStream _pew3 = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/pew_3.wav");
        //
        private static AudioStream _reload1 = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/reload_1.wav");
        private static AudioStream _reload2 = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/reload_2.wav");


        public SoundManager()
        {
            _musicAudioPlayer.Set("loop", true);
            _reloadAudioPlayer.VolumeDb = -6;

            AddChild(_musicAudioPlayer);
            AddChild(_sfxAudioPlayer);
            AddChild(_pewAudioPlayer);
            AddChild(_reloadAudioPlayer);

            //
            deathEnemySoundsBuffer = new AudioStreamPlayer2D[bufferSize];
            lastPlayedSoundInfo = new DateTime[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                deathEnemySoundsBuffer[i] = new AudioStreamPlayer2D();
                deathEnemySoundsBuffer[i].VolumeDb = -6;

                AddChild(deathEnemySoundsBuffer[i]);

                lastPlayedSoundInfo[i] = new DateTime();
            }
        }

        public void PlayRandomMusic()
        {
            var nextMusic = new Random().Next(2) == 0 ? _battleMusic1 : _battleMusic2;

            if (_musicAudioPlayer.Stream != nextMusic)
            {
                _musicAudioPlayer.Stream = nextMusic;
                _musicAudioPlayer.Playing = true;
            }

        }

        public void PlayPew(int pewType)
        {
            var nextSound = _pew1;
            switch (pewType)
            {
                case 0:
                    nextSound = _pew1;
                    break;
                case 1:
                    nextSound = _pew2;
                    break;
                default:
                    nextSound = _pew3;
                    break;
            }

            _pewAudioPlayer.Stream = nextSound;
            _pewAudioPlayer.Playing = true;

        }

        public void PlayReload()
        {
            var nextSound = new Random().Next(2) == 0 ? _reload1 : _reload2;

            if (_reloadAudioPlayer.Stream != nextSound)
            {
                _reloadAudioPlayer.Stream = nextSound;
                _reloadAudioPlayer.Playing = true;
            }

        }

        public void PlayBGMusic()
        {
            StopSounds();
            _musicAudioPlayer.Stream = _backgroundMusic;
            _musicAudioPlayer.Playing = true;
        }

        public void PlayGateSound()
        {
            _sfxAudioPlayer.Stream = _openGateSound;
            _sfxAudioPlayer.Playing = true;
        }

        public void PlayDeathSound(AudioStream sound, Vector2 pos)
        {
            DateTime tmpTimestamp = lastPlayedSoundInfo[bufferSize-1];
            AudioStreamPlayer2D tmpAudioPlayer = deathEnemySoundsBuffer[bufferSize - 1];

            for (int i = 0; i < bufferSize; i++)
            {
                if (lastPlayedSoundInfo[i] > tmpTimestamp)
                {
                    tmpTimestamp = lastPlayedSoundInfo[i];
                    tmpAudioPlayer = deathEnemySoundsBuffer[i];
                }
            }

            tmpAudioPlayer.Stream = sound;
            tmpAudioPlayer.Position = pos;

            tmpAudioPlayer.Playing = true;
        }

        public void StopSounds()
        {
            _musicAudioPlayer.Stream = _battleMusic1;
            _musicAudioPlayer.Playing = false;
            _sfxAudioPlayer.Playing = false;

            _pewAudioPlayer.Playing = false;
            _reloadAudioPlayer.Playing = false;


            for (int i = 0; i < bufferSize; i++)
            {
                deathEnemySoundsBuffer[i].Playing = false;

                lastPlayedSoundInfo[i] = new DateTime();
            }
        }
    }
}
