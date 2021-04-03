using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Gate : StaticBody2D
    {
        private Area2D _area2D;
        private CollisionShape2D _collisionShape;

        public override void _Ready()
        {
            _area2D = GetNode<Area2D>("Area2D");
            _area2D.Connect("body_entered", this, nameof(OnBodyEntered));
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        public void OnBodyEntered(Node body)
        {
            var player = body as Player;

            if (player == null)
            {
                return;
            }

            if (_collisionShape.Disabled)
            {
                GameManager.Instance.SceneManager.LoadRandomLevel();
                _area2D.Disconnect("body_entered", this, nameof(OnBodyEntered));
            }
        }

        public void Open()
        {
            _collisionShape.SetDeferred("disabled", true);
        }
    }
}