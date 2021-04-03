using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Weapons
{
    public class Shotgun : WeaponBase
    {
        public Shotgun() : base()
        {
        }

        [Export]
        public override int AmmoPerShot { get; protected set; } = 5;

        [Export]
        public override string GunName { get; protected set; } = "just a normal shotgun";

        [Export]
        public override float Recoil { get; protected set; } = 100f;

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;

            GameManager.Instance.Player.ApplyImpulse((GlobalPosition - GetGlobalMousePosition()).Normalized() * Recoil);
        }

        protected override void SpawnBullets()
        {
            var deltaAngle = 90 / AmmoPerShot;

            for (int i = 0; i < AmmoPerShot; i++)
            {
                var bullet = (Bullet)Bullet.Instance();

                bullet.GlobalPosition = _muzzlePoint.GlobalPosition;
                bullet.Direction = (GetGlobalMousePosition() - GlobalPosition).Normalized().Rotated(Mathf.Deg2Rad(-45 + i * deltaAngle));
                bullet.Speed = BulletSpeed;
                bullet.Damage = Damage;

                GetParent().GetParent().GetParent().AddChild(bullet);
            }
        }
    }
}