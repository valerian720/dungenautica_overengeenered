using Godot;
using SibGameJam2021.Core.Managers;

public class Enemy : KinematicBody2D
{
    public SpawnManager SpawnManager { get; set; }

    public override void _PhysicsProcess(float delta)
    {
        var player = GameManager.Instance.Player;

        Position += (player.Position - Position) / 50;
    }

    private void _on_Area2D_body_entered(Node body)
    {
        QueueFree();

        SpawnManager.EnemiesAlive--;
    }
}