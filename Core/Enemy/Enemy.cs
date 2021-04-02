using Godot;
using SibGameJam2021.Core.Managers;

public class Enemy : KinematicBody2D
{
    public override void _PhysicsProcess(float delta)
    {
        var player = GameManager.Instance.Player;

        Position += (player.Position - Position) / 50;
    }

    public override void _Ready()
    {
    }

    private void _on_Area2D_body_entered(Area2D body)
    {
        if (body.Name.IndexOf("Bullet") > 0)
        {
            QueueFree();
        }
        //GD.Print("enemy");
        //GD.Print(body.Name);
    }
}