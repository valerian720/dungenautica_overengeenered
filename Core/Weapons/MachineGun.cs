using Godot;
using SibGameJam2021.Core.Managers;
using System;

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
        public override float ReloadDuration { get; protected set; } = 2f;

        [Export]
        public override int SoundType { get; protected set; } = 1;

        [Export]
        public int DispersionDegree { get; protected set; } = 10;

        private static Random _bulletRandom = new Random();
        

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;

            GameManager.Instance.Player.ApplyImpulse((GlobalPosition - GetGlobalMousePosition()).Normalized() * Recoil);
        }

        protected override void SpecialAttack()
        {
            var tmpDamage = Damage;
            var tmpBulletSpeed = BulletSpeed;
            var tmpDispersionDegree = DispersionDegree;
            

            Damage = Damage * AmmoCount;
            BulletSpeed = BulletSpeed * 3;
            DispersionDegree = 0;

            SpawnProjectiles();
            AdditionalLogic();
            AdditionalLogic();

            Damage = tmpDamage;
            BulletSpeed = tmpBulletSpeed;
            DispersionDegree = tmpDispersionDegree;

        }

        protected override void SpawnProjectiles()
        {
            var bullet = InstanceBullet();
            bullet.Direction = (GetGlobalMousePosition() - GlobalPosition).Normalized().Rotated(Mathf.Deg2Rad(_bulletRandom.Next(-DispersionDegree, DispersionDegree)));


            GetParent().GetParent().GetParent().AddChild(bullet);
        }
    }
}