using Godot;
using System;

public class Enemy : KinematicBody2D
{
    private Vector2 motion = new Vector2();
    public override void _Ready()
    {
        
    }


    public override void _PhysicsProcess(float delta)
    {
        Node2D player = GetParent().GetNode("Player") as Node2D;

        Position += (player.Position - Position) / 50;
        LookAt(player.Position);

        MoveAndSlide(motion);
    }

    private void _on_Area2D_body_entered(Area2D body)
    {
        if (body.Name.IndexOf("Bullet") > 0)
        {
            QueueFree();
        }
        GD.Print("enemy");
        GD.Print(body.Name);
    }
}
