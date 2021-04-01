namespace SibGameJam2021.Core
{
    using Godot;
    using System;

    public class Player : KinematicBody2D
    {

        const int MAX_SPEED = 80;
        const int ACCELERATION = 600;
        const int FRICTION = 700;

        Vector2 velocity = Vector2.Zero;

        AnimationTree animationTree = null;
        AnimationPlayer animationPlayer = null;
        AnimationNodeStateMachinePlayback animationState = null;

        public override void _Ready()
        {
            animationTree = GetNode("AnimationTree") as AnimationTree; // da
            animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer; // da
            animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback; // da

            animationTree.Active = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            Vector2 inputVector = Vector2.Zero;
            inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
            inputVector = inputVector.Normalized();

            if (inputVector != Vector2.Zero)
            {
                animationTree.Set("parameters/Idle/blend_position", inputVector);
                animationTree.Set("parameters/Run/blend_position", inputVector);

                animationState.Travel("Run");

                velocity = velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * delta);
            }
            else
            {
                animationState.Travel("Idle");

                velocity = velocity.MoveToward(Vector2.Zero, FRICTION * delta);
            }

            //MoveAndCollide(velocity * delta); // сильная потеря скорости при движении вдоль коллайдера
            velocity = MoveAndSlide(velocity); // скольжение вдоль коллайдера
        }


    }
}