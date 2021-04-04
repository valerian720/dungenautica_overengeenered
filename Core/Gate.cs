using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Gate : StaticBody2D
    {
        private AnimatedSprite _animatedSprite;
        private Area2D _area2D;
        private CollisionShape2D _collisionShape;

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, nameof(DisableCollision));

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
                _area2D.Disconnect("body_entered", this, nameof(OnBodyEntered));

                GameManager.Instance.SceneManager.LoadRandomLevel();
            }
        }

        public void Open()
        {
            _animatedSprite.Play();
        }

        private void DisableCollision()
        {
            _collisionShape.SetDeferred("disabled", true);
        }
    }
}