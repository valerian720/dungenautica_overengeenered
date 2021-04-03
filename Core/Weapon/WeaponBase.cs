using Godot;
using SibGameJam2021.Core;

public class WeaponBase : Node2D
{
    private static readonly PackedScene Bullet;

    private int _ammoCount;

    private Node2D _muzzlePoint;

    static WeaponBase()
    {
        Bullet = GD.Load<PackedScene>("res://Assets/Prefabs/Bullet.tscn");
    }

    public virtual double AftershotDelay { get; private set; } = 3;

    public int AmmoCount
    {
        get { return _ammoCount; }

        private set { _ammoCount = value > 0 ? (value < MagSize ? value : MagSize) : 0; }
    }

    public virtual int AmmoPerShot { get; private set; } = 1;
    public virtual int BulletSpeed { get; private set; } = 200;
    public virtual int Damage { get; private set; } = 35;
    public virtual string GunDescription { get; private set; } = "an abomination";
    public virtual string GunName { get; private set; } = "XYN";
    public virtual int MagSize { get; private set; } = 30;
    public virtual double RateOfFire { get; private set; } = .5;
    public virtual int Recoil { get; private set; } = 100;

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("ui_fire"))
        {
            Shoot();
        }
    }

    public override void _Ready()
    {
        _muzzlePoint = GetNode<Node2D>("Muzzle");
    }

    public void Reload()
    {
        // сразу идет перезарядка на фулл без наказания в виде убывания запаса патронов
        AmmoCount = MagSize;
    }

    public void Shoot()
    {
        var bullet = (Bullet)Bullet.Instance();

        bullet.GlobalPosition = _muzzlePoint.GlobalPosition;
        bullet.Direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;

        GetParent().GetParent().AddChild(bullet);
    }
}