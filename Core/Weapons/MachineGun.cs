using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Weapons
{
    public class MachineGun : WeaponBase
    {
        public MachineGun() : base()
        {
        }

        [Export]
        public override float Damage { get; protected set; } = 15f;

        [Export]
        public override int MagSize { get; protected set; } = 50;

        [Export]
        public override float RateOfFire { get; protected set; } = 20f;

        [Export]
        public override float Recoil { get; protected set; } = 30f;

        [Export]
        public override float ReloadDuration { get; protected set; } = 5f;

        [Export]
        public override int SoundType { get; protected set; } = 1;
        

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;

            GameManager.Instance.Player.ApplyImpulse((GlobalPosition - GetGlobalMousePosition()).Normalized() * Recoil);
        }

        protected override void SpecialAttack()
        {
            var tmpDamage = Damage;
            var tmpBulletSpeed = BulletSpeed;
            

            Damage = Damage * AmmoCount;
            BulletSpeed = BulletSpeed * 3;
            SpawnProjectiles();
            AdditionalLogic();
            AdditionalLogic();
            Damage = tmpDamage;
            BulletSpeed = tmpBulletSpeed;

        }

        protected override void SpawnProjectiles()
        {
            var bullet = InstanceBullet();

            GetParent().GetParent().GetParent().AddChild(bullet);
        }
    }
}