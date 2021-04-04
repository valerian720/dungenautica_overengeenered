using Godot;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.UI;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core.Enemies
{
    public class Enemy : Entity
    {
        private Timer _attackDurationTimer = new Timer();
        private CollisionShape2D _attackShape;
        private HealthBar _healthbar;

        // AI
        [Export]
        private float ActivationRadius = 300;

        private MovementOnNavigation2D movementOnNav2D;

        [Export]
        private float SightActivationRadius = 350;

        [Export]
        private float StopAtRadius = 5;

        public Enemy() : base()
        {
            _attackDurationTimer.OneShot = true;
            _attackDurationTimer.Connect("timeout", this, nameof(OnAttackDurationEnded));
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

        public AudioStreamPlayer2D audioPlayer = null;

        public override void _Ready()
        {
            base._Ready();

            _attackShape = GetNode<CollisionShape2D>("AttackBox/CollisionShape2D");
            _attackShape.SetDeferred("disabled", true);

            _healthbar = GetNode<HealthBar>("HealthBar");

            movementOnNav2D = new MovementOnNavigation2D(GameManager.Instance.CurrentLevel.Navigation2D);
            AddChild(movementOnNav2D);

            AddChild(audioPlayer);
            audioPlayer.Playing = true;
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            SetAnimationHurt();
            _healthbar.UpdateHealth(CurrentHealth, MaxHealth);
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
            if ((distanceSquared < ActivationRadius * ActivationRadius) && (distanceSquared > StopAtRadius * StopAtRadius))
            {
                // базовое перемещение в сторону игрока если он находится в некотром радиусе от моба
                //Position += (player.Position - Position) / 50;
                //this.MoveAndCollide(); TODO
                FollowPath(player.Position);
                SetAnimationRun();
            }
            else
            {
                SetAnimationIdle();
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

            GetDamage(bullet.Pop());
        }

        virtual public void Attack()
        {
            SetAnimationAttack();

            _attackShape.SetDeferred("disabled", false);
            _attackDurationTimer.Start(0.5f);

            // here
        }

        private void FollowPath(Vector2 destiny)
        {
            // https://youtu.be/0fPOt0Jw52s
            Vector2 nextPoint = movementOnNav2D.GetPointTowardsDestiny(GlobalPosition, destiny);
            var velocity = (nextPoint - GlobalPosition).Normalized() * MaxSpeed;

            MoveAndSlide(velocity);
        }
        //

        private void OnAttackDurationEnded()
        {
            _attackShape.SetDeferred("disabled", true);
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