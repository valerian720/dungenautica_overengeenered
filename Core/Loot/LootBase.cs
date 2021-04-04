using Godot;

namespace SibGameJam2021.Core.Loot
{
    public abstract class LootBase : Node2D
    {
        protected const float Speed = 100;
        protected const float Tolerance = 3;

        protected Player _player;

        public bool Enabled { get; set; } = true;

        public override void _PhysicsProcess(float delta)
        {
            var direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            GlobalPosition += direction * Speed * delta;

            if (Mathf.Abs(GlobalPosition.x - _player.GlobalPosition.x) < Tolerance || Mathf.Abs(GlobalPosition.y - _player.GlobalPosition.y) < Tolerance)
            {
                GlobalPosition = _player.GlobalPosition;

                QueueFree();

                CustomLogic();
            }
        }

        public override void _Ready()
        {
            SetPhysicsProcess(false);

            if (Enabled)
            {
                GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnBodyEntered));
            }
        }

        protected abstract void CustomLogic();

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