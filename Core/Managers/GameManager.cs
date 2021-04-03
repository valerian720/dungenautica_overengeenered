using Godot;

namespace SibGameJam2021.Core.Managers
{
    public class GameManager
    {
        private GameManager()
        {
            Player = (Player)GD.Load<PackedScene>("res://Assets/Prefabs/Player.tscn").Instance();
        }

        public static GameManager Instance { get; } = new GameManager();

        public Player Player { get; }

        public SceneManager SceneManager { get; } = new SceneManager();
        public UIManager UIManager { get; set; }
    }
}