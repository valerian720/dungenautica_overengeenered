using Godot;
using System;

public class Shotgun : WeaponBase
{
    public override string GunName => "just a normal shorgun";
    public override GunType GunType => new tripleShotGunType();
    public override int ammoPerShot => 3;
}
