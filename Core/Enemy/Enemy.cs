using Godot;
using SibGameJam2021.Core.Managers;

public class Enemy : KinematicBody2D
{
    public SpawnManager SpawnManager { get; set; }


    public override void _PhysicsProcess(float delta)
    {
        var player = GameManager.Instance.Player;

        Position += (player.Position - Position) / 50;

        direction = (player.Position - Position).Normalized();
        UpdateAnimationTreeState();
    }

    private void _on_Area2D_body_entered(Node body)
    {
        QueueFree();

        SpawnManager.EnemiesAlive--;
    }

    // я хз будет ли это норм работать на всех противников или только на рыбу
    private AnimationNodeStateMachinePlayback animationState = null;
    private AnimationTree animationTree = null;

    private Vector2 direction;

    public override void _Ready()
    {
         animationTree = GetNode("Animations/AnimationTree") as AnimationTree; // da
         animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback; // da

        animationTree.Active = true;
    }

    private void UpdateAnimationTreeState()
    {
        animationTree.Set("parameters/Attack/blend_position", direction);
        animationTree.Set("parameters/Hurt/blend_position", direction);
        animationTree.Set("parameters/Idle/blend_position", direction);
        animationTree.Set("parameters/Run/blend_position", direction);
    }

    private void SetAnimationIdle() {
        animationState.Travel("Idle");
    }

    private void SetAnimationRun() {
        animationState.Travel("Run");
    }

    private void SetAnimationAttack() {
        animationState.Travel("Attack");
    }

    private void SetAnimationHurt() {
        animationState.Travel("Hurt");
    }
}