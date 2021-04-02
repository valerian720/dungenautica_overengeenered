using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Level : Node2D
    {
        private Gate _gate;
        private SpawnManager _spawnManager;

        public override void _Ready()
        {
            _gate = GetNode<Gate>("Gate");
            _spawnManager = GetNode<SpawnManager>("SpawnManager");

            _spawnManager.Connect("LevelCleared", this, nameof(OnLevelCleared));
        }

        private void OnLevelCleared()
        {
            _gate.Open();
        }
    }
}