using System.Collections.Generic;
using Godot;
using SibGameJam2021.Core.Spawn;

namespace SibGameJam2021.Core.Managers
{
    public class SpawnManager : YSort
    {
        private List<PackedScene> _enemiesScenes = new List<PackedScene>();
        private Node2D _playerSpawn;
        private int _remainingEnemies;
        private List<SpawnPoint> _spawnpoints = new List<SpawnPoint>();
        private Timer _timer = new Timer();

        public SpawnManager()
        {
            _timer.ProcessMode = Timer.TimerProcessMode.Physics;
            _timer.Connect("timeout", this, nameof(OnTimer));

            _remainingEnemies = TotalEnemyCount;
        }

        [Export]
        private int TotalEnemyCount { get; set; } = 10;

        [Export]
        private float WaveDelay { get; set; } = 5;

        [Export]
        private int WaveSize { get; set; } = 3;

        public override void _PhysicsProcess(float delta)
        {
        }

        public override void _Ready()
        {
            _playerSpawn = GetNode<Node2D>("PlayerSpawn");
            GameManager.Instance.Player.GlobalPosition = _playerSpawn.GlobalPosition;

            foreach (var child in GetChildren())
            {
                if (child is SpawnPoint)
                {
                    var point = child as SpawnPoint;
                    _spawnpoints.Add(point);
                }
            }

            var dir = new Directory();
            var path = "res://Assets/Prefabs/Enemies";

            if (dir.Open(path) == Error.Ok)
            {
                dir.ListDirBegin();
                var filename = dir.GetNext();

                while (!string.IsNullOrEmpty(filename))
                {
                    if (dir.CurrentIsDir())
                    {
                        filename = dir.GetNext();
                        continue;
                    }

                    //GD.Print(filename);

                    var enemyScene = GD.Load<PackedScene>($"{path}/{filename}");
                    _enemiesScenes.Add(enemyScene);

                    filename = dir.GetNext();
                }
            }

            AddChild(_timer);
            _timer.Start(WaveDelay);
        }

        private void OnTimer()
        {
            var enemiesLeft = WaveSize;

            int spawnIndex = 0;

            while (enemiesLeft > 0)
            {
                var enemy = (Enemy)_enemiesScenes[(int)GD.Randi() % _enemiesScenes.Count].Instance();

                _spawnpoints[spawnIndex].SpawnEnemy(enemy);

                spawnIndex = (spawnIndex + 1) % _spawnpoints.Count;

                enemiesLeft--;
                _remainingEnemies--;

                if (_remainingEnemies <= 0)
                {
                    _timer.Stop();

                    GD.Print("Level Cleared");

                    break;
                }
            }
        }
    }
}