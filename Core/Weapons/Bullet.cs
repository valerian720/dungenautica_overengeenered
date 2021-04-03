using Godot;

namespace SibGameJam2021.Core.Weapons
{
    public class Bullet : KinematicBody2D
    {
        [Export]
        public float LifeTime = 10f;

        private Timer _timer = new Timer();

        public float Damage { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            var info = MoveAndCollide(Direction * Speed * delta);

            if (info != null)
            {
                QueueFree();
            }
        }

        public override void _Ready()
        {
            AddChild(_timer);
            _timer.Connect("timeout", this, nameof(OnTimer));
            _timer.Start(LifeTime);
        }

        private void OnTimer()
        {
            _timer.Stop();
            QueueFree();
        }
    }
}