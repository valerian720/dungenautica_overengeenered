using Godot;
using System;

public class WeaponBase : Node
{
    private static int BULLET_SPEED = 500;
    private PackedScene bullet = null;

    public override void _Ready()
    {
        bullet = GD.Load("res://Assets/Prefab/Bullet.tscn") as PackedScene;
    }

    public void fire(Vector2 direction, Vector2 position)
    {
        Node bulletInstance = bullet.Instance();

    }
}
