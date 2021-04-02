namespace SibGameJam2021.Core
{
    using Godot;
    using SibGameJam2021.Core.Managers;
    using System;
    using Object = Godot.Object;

    public class Player : KinematicBody2D
    {

        const int MAX_SPEED = 80;
        const int ACCELERATION = 600;
        const int FRICTION = 700;

        Vector2 velocity = Vector2.Zero;

        AnimationTree animationTree = null;
        AnimationPlayer animationPlayer = null;
        AnimationNodeStateMachinePlayback animationState = null;
        Node2D gunSlot = null;

        public override void _Ready()
        {
            animationTree = GetNode("AnimationTree") as AnimationTree; // da
            animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer; // da
            animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback; // da

            gunSlot = GetNode("GunSlot") as Node2D; // подгрузка ссылки на слот для оружия

            animationTree.Active = true;
        }

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

        private void kill()
        {
            GameManager.Instance.SceneManager.LoadMainMenu(); // todo нормальная смерть
            GD.Print("ded");
        }

        private void _on_Hitbox_body_entered(Area2D body)
        {
            if (body.Name.IndexOf("Enemy")==0)    
            {
                kill();
            }

            GD.Print(body.Name);

        }
    }
}