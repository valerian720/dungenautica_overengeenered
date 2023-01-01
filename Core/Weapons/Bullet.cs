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

        private CollisionShape2D _collisionLayer = null;

        public override void _PhysicsProcess(float delta)
        {
            var tmpDir = Direction;
            var collision = MoveAndCollide(Direction * Speed * delta);

            if (collision != null)
            {
                //
                //if (collision.Collider is TileMap)
                //{
                //    var tilemap = (TileMap)collision.Collider;
                //    var tilePos = tilemap.WorldToMap(collision.Position);
                //    GD.Print(tilePos);
                //    var tileName = tilemap.TileSet.TileGetName(tilemap.GetCell((int)tilePos.x, (int)tilePos.y));
                //    GD.Print(tileName);

                //    //if (tileName == "HoleDeapthTiles") // TODO
                //    //{
                //    //    Direction = tmpDir;
                //    //    Position = Position + Direction * Speed * delta;
                //    //    return;

                //    //}

                //}
                //
                if (_bouncesLeft > 0)
                {
                    Direction = Direction.Bounce(collision.Normal);

                    if (_bouncesLeft == GameManager.Instance.Player.BounceBoost) // first bounce
                        Damage *= 2;
                    else
                    {
                        Speed /= 2;
                        Damage /= 2;
                    }

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

            _collisionLayer = (CollisionShape2D)GetChild(1);

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