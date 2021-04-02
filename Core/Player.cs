using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core
{
    public class Player : KinematicBody2D
    {
        private const int ACCELERATION = 600;
        private const int FRICTION = 700;
        private const int MAX_SPEED = 80;

        private const int MAX_HEALTH = 100;
        private const int DMG_PER_HIT = 10;

        public int CurrentHealth
        {
            get { return _сurrentHealth; }

            private set { _сurrentHealth = value > 0 ? (value < MAX_HEALTH ? value : MAX_HEALTH) : 0; }
        }
        private int _сurrentHealth;

        public Player()
        {
            CurrentHealth = MAX_HEALTH;
        }

        private AnimationPlayer animationPlayer = null;
        private AnimationNodeStateMachinePlayback animationState = null;
        private AnimationTree animationTree = null;
        private Node2D gunSlot = null;
        private Vector2 velocity = Vector2.Zero;

        public override void _PhysicsProcess(float delta)
        {
            // получение вектора движения игрока
            Vector2 inputVector = Vector2.Zero;
            inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
            inputVector = inputVector.Normalized();

            if (inputVector != Vector2.Zero)
            {
                // передача текущей скорости игрока в дерево анимации
                animationTree.Set("parameters/Idle/blend_position", inputVector);
                animationTree.Set("parameters/Run/blend_position", inputVector);

                // переключение дерева анимации на бег
                animationState.Travel("Run");

                // применение трения к игроку
                velocity = velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * delta);
            }
            else
            {
                // переключение дерева анимации на idle
                animationState.Travel("Idle");

                // применение трения к игроку
                velocity = velocity.MoveToward(Vector2.Zero, FRICTION * delta);
            }

            //MoveAndCollide(velocity * delta); // сильная потеря скорости при движении вдоль коллайдера
            velocity = MoveAndSlide(velocity); // скольжение вдоль коллайдера

            //
            gunSlot.LookAt(GetGlobalMousePosition());
        }

        public override void _Ready()
        {
            animationTree = GetNode("AnimationTree") as AnimationTree; // da
            animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer; // da
            animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback; // da

            gunSlot = GetNode("GunSlot") as Node2D; // подгрузка ссылки на слот для оружия

            animationTree.Active = true;
        }

        private void DealDmg()
        {
            CurrentHealth -= DMG_PER_HIT;

            if (CurrentHealth == 0)
            {
                kill();
            }
        }
        private void _on_Hitbox_body_entered(Area2D body)
        {
            if (body.Name.IndexOf("Enemy") == 0)
            {
                kill();
            }

            GD.Print(body.Name);
        }

        private void kill()
        {
            GameManager.Instance.SceneManager.LoadMainMenu(); // todo нормальная смерть
            GD.Print("dead");
        }
    }
}