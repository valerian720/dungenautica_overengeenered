using Godot;

namespace SibGameJam2021.Core.Loot
{
    public class Coin : Node2D
    {
        private const float Speed = 100;
        private const float Tolerance = 3;

        private Player _player;

        [Export]
        protected virtual int Value { get; set; } = 1;

        public override void _PhysicsProcess(float delta)
        {
            var direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            GlobalPosition += direction * Speed * delta;

            if (Mathf.Abs(GlobalPosition.x - _player.GlobalPosition.x) < Tolerance || Mathf.Abs(GlobalPosition.y - _player.GlobalPosition.y) < Tolerance)
            {
                GlobalPosition = _player.GlobalPosition;

                QueueFree();

                _player.Coins += Value;
            }
        }

        public override void _Ready()
        {
            GetNode<AnimatedSprite>("AnimatedSprite").Play();
            GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnBodyEntered));
            SetPhysicsProcess(false);
        }

        private void OnBodyEntered(Node body)
        {
            var player = body as Player;

            if (player == null)
            {
                return;
            }

            _player = player;

            SetPhysicsProcess(true);
        }
    }
}