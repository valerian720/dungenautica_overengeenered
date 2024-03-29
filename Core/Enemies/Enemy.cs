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
        private Vector2 _forcedVelocity = Vector2.Zero;

        public Enemy() : base()
        {
            //_attackDurationTimer.OneShot = true;
            _attackDurationTimer.Connect("timeout", this, nameof(OnAttackDurationEnded));
            //
            this.MaxHealth += GameManager.Instance.SceneManager.LevelCount; // auto scale health
            this.Damage += GameManager.Instance.SceneManager.LevelCount; // auto scale damage
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

        public void ApplyImpulse(Vector2 velocity)
        {
            _forcedVelocity = velocity;
        }

        virtual public void UpdatePosition(float delta)
        {
            // TODO redo
            // add ability to see. not just sense player (move if raycast to player = true)
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
                movementOnNav2D.DebugPathClearPoints();
            }

            if (distanceSquared < SightActivationRadius * SightActivationRadius)
            {
                // обновление анимаций моба если игрок входит в определенный радиус
                UpdateAnimationTreeState((player.GlobalPosition - GlobalPosition).Normalized());
            }

            MoveAndSlide(_forcedVelocity);
            _forcedVelocity = _forcedVelocity / 2;
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
            movementOnNav2D.DebugPathClearPoints();
            movementOnNav2D.PopResources();

            QueueFree();
            SpawnManager.EnemiesAlive--;

            DropLoot();
        }

        private void DropLoot()
        {
            Node loot;

            // TODO make nice

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
                Node2D tmp = (Node2D)loot;
                tmp.Position = Position;
                GameManager.Instance.CurrentLevel.AddChild(tmp);
            }


            if (GameManager.Instance.Player.HasEquiped(0) && GameManager.Instance.Player.BulletsLeft <= 0) // 100% chance if killed with last bullet in minigun (or super attack of minigun)
            {
                loot = LootManager.CoinScene.Instance();

                if (GameManager.Instance.Player.GlobalPosition.DistanceTo(GlobalPosition) > 220) // if killed enemy was too far then get heal
                {
                    loot = LootManager.HealScene.Instance();
                    GD.Print($"distance = {GameManager.Instance.Player.GlobalPosition.DistanceTo(GlobalPosition)}");
                }

                Node2D tmp = (Node2D)loot;
                tmp.Position = Position;
                GameManager.Instance.CurrentLevel.AddChild(tmp);
            }

            //GD.Print($"distance = {GameManager.Instance.Player.GlobalPosition.DistanceTo(GlobalPosition)}");

            if (GameManager.Instance.Player.HasEquiped(1) && GameManager.Instance.Player.GlobalPosition.DistanceTo(GlobalPosition) < 60) // 100% chance if killed with pistol while beeing close 
            {
                loot = LootManager.HealScene.Instance();

                Node2D tmp = (Node2D)loot;
                tmp.Position = Position;
                GameManager.Instance.CurrentLevel.AddChild(tmp);
            }


            if (GameManager.Instance.Player.HasEquiped(0) && GameManager.Instance.Player.GlobalPosition.DistanceTo(GlobalPosition) > 220) // 100% chance if killed with pistol while beeing close 
            {
                loot = LootManager.HealScene.Instance();

                Node2D tmp = (Node2D)loot;
                tmp.Position = Position;
                GameManager.Instance.CurrentLevel.AddChild(tmp);
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