using Godot;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.UI;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core.Enemies
{
    public class Enemy : Entity
    {
        private HealthBar _healthbar;
        private MovementOnNavigation2D movementOnNav2D;

        // AI
        [Export]
        private float ActivationRadius = 300;

        [Export]
        private float SightActivationRadius = 350;
        [Export]
        private float StopAtRadius = 50;

        public Enemy() : base()
        {
        }

        [Export]
        public float Damage { get; set; } = 25;

        public SpawnManager SpawnManager { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            var player = GameManager.Instance.Player;

            UpdatePosition(player, delta);

            UpdateAnimation(player);
        }

        public override void _Ready()
        {
            base._Ready();
            movementOnNav2D = new MovementOnNavigation2D(GameManager.Instance.CurrentLevel.Navigation2D);
            _healthbar = GetNode<HealthBar>("HealthBar");
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);

            _healthbar.UpdateHealth(CurrentHealth, MAX_HEALTH);
        }

        virtual public void UpdateAnimation(Player player)
        {
            if (player.Position.DistanceSquaredTo(Position) < SightActivationRadius * SightActivationRadius)
            {
                // обновление анимаций моба если игрок входит в определенный радиус
                UpdateAnimationTreeState((player.Position - Position).Normalized());
            }
        }

        virtual public void UpdatePosition(Player player, float delta)
        {
            float distanceSquared = player.Position.DistanceSquaredTo(Position);
            if ((distanceSquared < ActivationRadius * ActivationRadius) && (distanceSquared > StopAtRadius*StopAtRadius))
            {
                // базовое перемещение в сторону игрока если он находится в некотром радиусе от моба
                //Position += (player.Position - Position) / 50;
                //this.MoveAndCollide(); TODO
                FollowPath(this.MAX_SPEED*delta, player.Position);
                SetAnimationRun();
            }
            else
            {
                SetAnimationIdle();
            }
        }
        //
        private void FollowPath(float moveDistance, Vector2 destiny)
        {
            // https://youtu.be/0fPOt0Jw52s
            Vector2 nextPoint = movementOnNav2D.GetPointTowardsDestiny(Position, destiny);
            GD.Print(nextPoint);
            /* 
             * известно:
             * - текущая точка моба
             * - следующая точка моба
             * - дистанция которая будет пройдена за кадр
             * 
             * найти точку на векторе, исходящему из текущей точки в сторону следующей, уделенному от исходной точки на расстояние
             * 
             * решение MoveToward // бля
             */
            Position = Position.MoveToward(nextPoint, moveDistance);

            //float distanceToNextPoint = Position.DistanceTo(nextPoint);
            //if (moveDistance<= distanceToNextPoint && moveDistance>=0.0)
            //{
            //    Position = Position.LinearInterpolate(nextPoint, moveDistance*1000 / distanceToNextPoint);
            //}
        }
        //

        protected override void Die()
        {
            QueueFree();
            SpawnManager.EnemiesAlive--;
        }

        private void _on_Area2D_body_entered(Node body)
        {
            var bullet = body as Bullet;

            if (bullet == null)
            {
                return;
            }

            GetDamage(bullet.Pop());
        }

        private void SetAnimationAttack()
        {
            _animationState.Travel("Attack");
        }

        private void SetAnimationHurt()
        {
            _animationState.Travel("Hurt");
        }

        private void SetAnimationIdle()
        {
            _animationState.Travel("Idle");
        }

        private void SetAnimationRun()
        {
            _animationState.Travel("Run");
        }

        private void UpdateAnimationTreeState(Vector2 direction)
        {
            _animationTree.Set("parameters/Attack/blend_position", direction);
            _animationTree.Set("parameters/Hurt/blend_position", direction);
            _animationTree.Set("parameters/Idle/blend_position", direction);
            _animationTree.Set("parameters/Run/blend_position", direction);
        }
    }
}