using System;
using Godot;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.UI;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core.Enemies
{
    public class Enemy : Entity
    {
        public AudioStreamPlayer2D audioPlayer = null;
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
        private float StopAtRadius = 20;

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
            UpdatePosition(delta);
        }

        public override void _Ready()
        {
            base._Ready();

            _attackShape = GetNode<CollisionShape2D>("AttackBox/CollisionShape2D");
            _attackShape.SetDeferred("disabled", true);

            _healthbar = GetNode<HealthBar>("HealthBar");

            AddChild(_attackDurationTimer);

            movementOnNav2D = new MovementOnNavigation2D(GameManager.Instance.CurrentLevel.Navigation2D);
            AddChild(movementOnNav2D);

            audioPlayer = new AudioStreamPlayer2D();
            AddChild(audioPlayer);
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            SetAnimationHurt();
            _healthbar.UpdateHealth(CurrentHealth, MaxHealth);
        }

        virtual public void UpdatePosition(float delta)
        {
            var player = GameManager.Instance.Player;

            float distanceSquared = player.GlobalPosition.DistanceSquaredTo(GlobalPosition);

            if (distanceSquared < ActivationRadius * ActivationRadius)
            {
                if (distanceSquared > StopAtRadius * StopAtRadius)
                {
                    var nextPoint = movementOnNav2D.GetPointTowardsDestiny(GlobalPosition, player.GlobalPosition);
                    var velocity = (nextPoint - GlobalPosition).Normalized() * MaxSpeed;

                    MoveAndSlide(velocity);

                    SetAnimationRun();
                }
                else
                {
                    Attack();
                }
            }
            else
            {
                SetAnimationIdle();
            }

            if (distanceSquared < SightActivationRadius * SightActivationRadius)
            {
                // обновление анимаций моба если игрок входит в определенный радиус
                UpdateAnimationTreeState((player.GlobalPosition - GlobalPosition).Normalized());
            }
        }

        virtual protected void Attack()
        {
            SetAnimationAttack();

            _attackShape.SetDeferred("disabled", false);
            _attackDurationTimer.Start(0.5f);

            // here
        }

        protected override void Die()
        {
            QueueFree();
            SpawnManager.EnemiesAlive--;

            DropLoot();
        }

        private void DropLoot()
        {
            Node loot;

            if (new Random().Next(10) == 0 || GameManager.Instance.Player.HasEquiped(2)) // 10% chance if not shotgun and 100 if
            {
                if (new Random().Next(4) == 0) // 25% chance
                {
                    loot = LootManager.CoinScene.Instance();
                }
                else
                {
                    loot = LootManager.HealScene.Instance();
                }
                // TODO make sure pos is rigth
                Node2D tmp = (Node2D)loot;
                tmp.Position = Position;
                //loot.Position();
                GameManager.Instance.CurrentLevel.AddChild(tmp);
                //CallDeferred("add_child", loot);
            }

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