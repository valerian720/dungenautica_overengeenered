using Godot;

namespace SibGameJam2021.Core.Weapons
{
    public class Bullet : KinematicBody2D
    {
        [Export]
        public float LifeTime = 10f;

        private AnimatedSprite _animatedSprite;
        private Timer _timer = new Timer();
        public float Damage { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            var info = MoveAndCollide(Direction * Speed * delta);

            if (info != null)
            {
                Pop();
            }
        }

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, "queue_free");

            AddChild(_timer);
            _timer.Connect("timeout", this, nameof(OnTimer));
            _timer.Start(LifeTime);
        }

        public float Pop()
        {
            _animatedSprite.Play();
            return Damage;
        }

        private void OnTimer()
        {
            _timer.Stop();
            Pop();
        }
    }
}