using Godot;
using SibGameJam2021.Core.Enemies;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Player : Entity
    {
        private Vector2 _velocity = Vector2.Zero;
        private Node2D gunSlot = null;

        public Player() : base()
        {
        }

        public  void ApplyImpulse(Vector2 velocity)
        {
            _velocity += velocity;
        }

        public override void _PhysicsProcess(float delta)
        {
            // получение вектора движения игрока
            Vector2 inputVector = Vector2.Zero;
            inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
            inputVector = inputVector.Normalized();

            if (!inputVector.IsEqualApprox(Vector2.Zero))
            {
                // передача текущей скорости игрока в дерево анимации
                _animationTree.Set("parameters/Idle/blend_position", inputVector);
                _animationTree.Set("parameters/Run/blend_position", inputVector);

                // переключение дерева анимации на бег
                _animationState.Travel("Run");

                // применение трения к игроку
                _velocity = _velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * delta);
            }
            else
            {
                // переключение дерева анимации на idle
                _animationState.Travel("Idle");

                // применение трения к игроку
                _velocity = _velocity.MoveToward(Vector2.Zero, FRICTION * delta);
            }

            _velocity = MoveAndSlide(_velocity); // скольжение вдоль коллайдера

            gunSlot.LookAt(GetGlobalMousePosition());
        }

        public override void _Ready()
        {
            base._Ready();

            gunSlot = GetNode<Node2D>("GunSlot"); // подгрузка ссылки на слот для оружия
        }

        protected override void Die()
        {
            GameManager.Instance.SceneManager.LoadMainMenu();
        }

        private void _on_Hitbox_body_entered(Node body)
        {
            if (body.Name.IndexOf("Enemy") == 0)
            {
                GetDamage((body as Enemy).Damage);
                _animationState.Travel("Hurt");
            }
        }
    }
}