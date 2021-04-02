using Godot;
using System;

public class WeaponBase : Node2D
{
    public virtual string GunName { get; private set; } = "XYN";
    public virtual string GunDescription { get; private set; } = "an abomination";
    public virtual GunType GunType { get; private set; } = new baloonGunType();
    public virtual int Recoil { get; private set; } = 100;
    public virtual int Damage { get; private set; } = 30;
    public virtual int MagSize { get; private set; } = 30;
    public virtual int ammoPerShot { get; private set; } = 1;
    public virtual double RateOfFire { get; private set; } = .5;
    public virtual double aftershotDelay { get; private set; } = 3;
    public int ammoCountdown { get { return this.ammoCountdown; } private set { this.ammoCountdown = value > 0 ? (value<MagSize?value:MagSize) : 0; } } // ограничение количества активных патронов (от 0 до размера магазина)

    private TimeStamp canShootAt = new Timer(); // TODO
    


    public void shoot(float rotationDegrees, Vector2 position)
    {
        canShootAt.
        // за сам эффект стрельбы отвечает тип оружия
        GunType.Shoot(rotationDegrees, position, Damage, RateOfFire);
        // aftershot delay (?)
    }

    public void reload()
    {
        // сразу идет перезарядка на фулл без наказания в виде убывания запаса патронов
        ammoCountdown = MagSize;
    }

    // отлавливание событий
    public override void _PhysicsProcess(float delta)
    {
        if 
    }
}



