using System;
using Godot;

public class BaloonGunType : GunType
{
    public override int BulletSpeed => 100;
    public override string GunTypeName => "baloon gun type";

    public override void Shoot(Node2D root, float rotationDegrees, Vector2 position, int Damage, double RateOfFire)
    {
        throw new NotImplementedException("no shoot logic"); //todo
    }
}

public abstract class GunType
{
    public PackedScene Bullet { get; } = (PackedScene)ResourceLoader.Load("res://Assets/Prefabs/Bullet.tscn");
    public virtual int BulletSpeed { get; private set; } = 500;
    public virtual string GunTypeName { get; private set; } = "base gun type (not implemented)";

    public abstract void Shoot(Node2D root, float rotationDegrees, Vector2 position, int Damage, double RateOfFire);

    // загрузка объекта пули
}

public class LaserGunType : GunType
{
    public override int BulletSpeed => 600;
    public override string GunTypeName => "laser gun type";

    public override void Shoot(Node2D root, float rotationDegrees, Vector2 position, int Damage, double RateOfFire)
    {
        throw new NotImplementedException("no shoot logic"); //todo
    }
}

public class TripleShotGunType : GunType
{
    public override string GunTypeName => "triple shot gun type";

    public override void Shoot(Node2D root, float rotationDegrees, Vector2 position, int Damage, double RateOfFire)
    {
        var buletInstance = Bullet.Instance() as RigidBody2D;

        buletInstance.Position = position;
        buletInstance.RotationDegrees = rotationDegrees;
        buletInstance.ApplyImpulse(new Vector2(), new Vector2(BulletSpeed, 0).Rotated(rotationDegrees));
        /*

         buletInstance.Position = position.Rotated(Mathf.Deg2Rad(rotationDegrees));
        //buletInstance.RotationDegrees = rotationDegrees;
        buletInstance.ApplyImpulse(new Vector2(), new Vector2(bulletSpeed, 0).Rotated(rotationDegrees));

         */

        root.AddChild(buletInstance);
    }
}