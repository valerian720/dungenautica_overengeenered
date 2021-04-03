using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Enemies
{
    public class Enemy : Entity
    {
        private Healthbar _healthbar;

        // AI
        [Export]
        private float ActivationRadius = 300;

        [Export]
        private float SightActivationRadius = 350;

        public Enemy() : base()
        {
        }

        [Export]
        public float Damage { get; set; } = 25;

        public SpawnManager SpawnManager { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            var player = GameManager.Instance.Player;

            UpdatePosition(player);

            UpdateAnimation(player);
        }

        public override void _Ready()
        {
            base._Ready();

            _healthbar = GetNode<Healthbar>("Healthbar");
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);

            _healthbar.UpdateHealth(CurrentHealth, MAX_HEALTH);
        }

        virtual public void UpdatePosition(Player player)
        {
            if (player.Position.DistanceSquaredTo(Position) < ActivationRadius * ActivationRadius)
            {
                // базовое перемещение в сторону игрока если он находится в некотром радиусе от моба
                Position += (player.Position - Position) / 50;
                SetAnimationRun();
            }
            else
            {
                SetAnimationIdle();
            }
        }
        virtual public void UpdateAnimation(Player player)
        {
            if (player.Position.DistanceSquaredTo(Position) < SightActivationRadius * SightActivationRadius)
            {
                // обновление анимаций моба если игрок входит в определенный радиус
                UpdateAnimationTreeState((player.Position - Position).Normalized());
            }
        }

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

            GetDamage(bullet.Damage);
            bullet.QueueFree();
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