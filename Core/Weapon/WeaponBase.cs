using Godot;

public class WeaponBase : Node2D
{
    private int _ammoCountdown;
    private double CanShootAt = 0;
    public virtual double AftershotDelay { get; private set; } = 3;

    public int AmmoCountdown
    {
        get { return _ammoCountdown; }

        private set { _ammoCountdown = value > 0 ? (value < MagSize ? value : MagSize) : 0; }
    }

    public virtual int AmmoPerShot { get; private set; } = 1;
    public virtual int Damage { get; private set; } = 30;
    public virtual string GunDescription { get; private set; } = "an abomination";
    public virtual string GunName { get; private set; } = "XYN";
    public virtual GunType GunType { get; private set; } = new BaloonGunType();
    public virtual int MagSize { get; private set; } = 30;
    public virtual double RateOfFire { get; private set; } = .5;
    public virtual int Recoil { get; private set; } = 100;

    // отлавливание событий
    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("ui_fire"))
        {
            GD.Print(GetChild(0).GetParent());
            // (GetParent() as Node2D)
            Shoot(RotationDegrees, GlobalPosition);  // s своей позиции и направления вращения слота для оружия (0 индекс) //
            GD.Print(RotationDegrees, GlobalPosition);
        }
    }

    public void Reload()
    {
        // сразу идет перезарядка на фулл без наказания в виде убывания запаса патронов
        AmmoCountdown = MagSize;
    }

    // ограничение количества активных патронов (от 0 до размера магазина)
    public void Shoot(float rotationDegrees, Vector2 position)
    {
        // за сам эффект стрельбы отвечает тип оружия
        GunType.Shoot(FindParent("World") as Node2D, rotationDegrees, position, Damage, RateOfFire);
        // aftershot delay (?)
    }
}