using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Weapons
{
    public class Bullet : KinematicBody2D
    {
        [Export]
        public float LifeTime = 10f;

        private AnimatedSprite _animatedSprite;
        private int _bouncesLeft;
        private Timer _timer = new Timer();
        public float Damage { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            var collision = MoveAndCollide(Direction * Speed * delta);

            if (collision != null)
            {
                if (_bouncesLeft > 0)
                {
                    Direction = Direction.Bounce(collision.Normal);
                    _bouncesLeft--;
                }
                else
                {
                    Pop();
                }
            }
        }

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, "queue_free");

            _bouncesLeft = GameManager.Instance.Player.BounceBoost;

            AddChild(_timer);
            _timer.Connect("timeout", this, nameof(OnTimer));
            _timer.Start(LifeTime);
        }

        public float Pop()
        {
            SetPhysicsProcess(false);
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