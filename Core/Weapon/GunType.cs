using Godot;
using System;

public class GunType
{
    public string name = "base gun type";

    virtual public void Shoot(Vector2 direction, Vector2 position, int Damage, double RateOfFire) => throw new NotImplementedException("no shoot logic for base gun type");
}

public class baloonGunType : GunType
{
    public override void Shoot(Vector2 direction, Vector2 position, int Damage, double RateOfFire)
    {

    }
}
public class laserGunType : GunType
{
    public override void Shoot(Vector2 direction, Vector2 position, int Damage, double RateOfFire)
    {

    }
}
public class tripleShotGunType : GunType
{
    public override void Shoot(Vector2 direction, Vector2 position, int Damage, double RateOfFire)
    {

    }
}
