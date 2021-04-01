using Godot;
using System;

public class WeaponBase
{
    public virtual string GunName { get; private set; } = "XYN";
    public virtual string GunDescription { get; private set; } = "an abomination";
    public virtual GunType GunType { get; private set; } = new baloonGunType();
    public virtual int Recoil { get; private set; } = 100;
    public virtual int Damage { get; private set; } = 30;
    public virtual int MagSize { get; private set; } = 30;
    public virtual int ammoPerShot { get; private set; } = 1;
    public virtual double RateOfFire { get; private set; } = .5;

    public int ammoCountdown { get { return this.ammoCountdown; } private set { this.ammoCountdown = value > 0 ? (value<MagSize?value:MagSize) : 0; } } // ограничение количества активных патронов (от 0 до размера магазина)

    public void shoot(Vector2 direction, Vector2 position)
    {
        // за сам эффект стрельбы отвечает тип оружи€
        GunType.Shoot(direction, position, Damage, RateOfFire);
    }

    public void reload()
    {
        // сразу идет перезар€дка на фулл без наказани€ в виде убывани€ запаса патронов
        ammoCountdown = MagSize;
    }
}



