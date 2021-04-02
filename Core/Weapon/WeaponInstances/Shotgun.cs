public class Shotgun : WeaponBase
{
    public override int AmmoPerShot => 3;
    public override string GunName => "just a normal shorgun";
    public override GunType GunType => new TripleShotGunType();
}