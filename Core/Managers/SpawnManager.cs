using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SibGameJam2021.Core.Enemies;
using SibGameJam2021.Core.Spawn;
using SibGameJam2021.Core.Utility;

namespace SibGameJam2021.Core.Managers
{
	public class SpawnManager : YSort
	{
		private static readonly List<PackedScene> _enemiesScenes = PrefabHelper.LoadPrefabsList("res://Assets/Prefabs/Enemies");

		private int _enemiesAlive = 0;

		private int _enemiesToSpawn;

		private Node2D _playerSpawn;

		private List<SpawnPoint> _spawnpoints = new List<SpawnPoint>();

		private Timer _timer = new Timer();

		public SpawnManager()
		{
			_timer.ProcessMode = Timer.TimerProcessMode.Physics;
			_timer.Connect("timeout", this, nameof(OnTimer));
		}

		[Signal]
		public delegate void LevelCleared();

		public int EnemiesAlive
		{
			get { return _enemiesAlive; }

			set
			{
				_enemiesAlive = value;

				if (_enemiesAlive <= 0)
				{
					GD.Print("Level Cleared");

					EmitSignal(nameof(LevelCleared));
				}
			}
		}

		[Export]
		private Dictionary<PackedScene, int> Enemies { get; set; } = _enemiesScenes.ToDictionary(x => x, y => 0);

		[Export]
		private float WaveDelay { get; set; } = 5;

        public override void _Ready()
        {
            _playerSpawn = GetNode<Node2D>("PlayerSpawn");

			foreach (var child in GetChildren())
			{
				if (child is SpawnPoint)
				{
					var point = child as SpawnPoint;
					_spawnpoints.Add(point);
				}
			}

			_enemiesToSpawn = Enemies.Sum(x => x.Value);

			AddChild(_timer);
			_timer.Start(WaveDelay);
		}

        public void RemovePlayer()
        {
            CallDeferred("remove_child", GameManager.Instance.Player);
        }

        public void SpawnPlayer()
        {
            CallDeferred("add_child", GameManager.Instance.Player);
            GameManager.Instance.Player.GlobalPosition = _playerSpawn.GlobalPosition;
        }

        private void OnTimer()
        {
            if (_enemiesToSpawn <= 0)
            {
                EnemiesAlive = 0;
                _timer.Stop();
                return;
            }

            foreach (var point in _spawnpoints)
            {
                var enemies = Enemies.Where(x => x.Value > 0);

				if (enemies.Count() < 1)
				{
					_timer.Stop();

					break;
				}

				var pair = enemies.ElementAt(new Random().Next(0, enemies.Count()));

				Enemies[pair.Key]--;

				var enemy = (Enemy)pair.Key.Instance();
				enemy.SpawnManager = this;

				point.SpawnEnemy(enemy);

				_enemiesToSpawn--;
				EnemiesAlive++;

				if (_enemiesToSpawn <= 0)
				{
					_timer.Stop();

					break;
				}
			}
		}
	}
}
