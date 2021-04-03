using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Gate : StaticBody2D
    {
        private CollisionShape2D _collisionShape;

        public override void _Ready()
        {
            GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnBodyEntered));
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        public void OnBodyEntered(Node body)
        {
            if (_collisionShape.Disabled)
            {
                GameManager.Instance.SceneManager.LoadRandomLevel();
            }
        }

        public void Open()
        {
            _collisionShape.SetDeferred("disabled", true);
        }
    }
}