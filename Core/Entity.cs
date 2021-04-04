using Godot;

namespace SibGameJam2021.Core
{
    public abstract class Entity : KinematicBody2D
    {
        protected AnimationPlayer _animationPlayer = null;
        protected AnimationNodeStateMachinePlayback _animationState = null;
        protected AnimationTree _animationTree = null;
        protected float _currentHealth;

        public Entity()
        {
            _currentHealth = MAX_HEALTH;
        }

        public virtual float CurrentHealth
        {
            get { return _currentHealth; }

            protected set { _currentHealth = value > 0 ? (value < MAX_HEALTH ? value : MAX_HEALTH) : 0; }
        }

        protected virtual float ACCELERATION { get; } = 600;

        protected virtual float FRICTION { get; } = 700;

        protected virtual float MAX_HEALTH { get; } = 100;

        protected virtual float MAX_SPEED { get; } = 80;

        public override void _Ready()
        {
            _animationTree = GetNode<AnimationTree>("Animations/AnimationTree");
            _animationPlayer = GetNode<AnimationPlayer>("Animations/AnimationPlayer");
            _animationState = _animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

            _animationTree.Active = true;
        }

        public virtual void GetDamage(float damage)
        {
            CurrentHealth -= damage;

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        protected abstract void Die();
    }
}