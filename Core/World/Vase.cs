using System;
using Godot;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core.World
{
    public class Vase : StaticBody2D
    {
        [Export]
        public bool IsOptional = true;
        [Export]
        public int DisaplayPersantage = 30;

        private AnimatedSprite _animatedSprite;
        private CollisionShape2D _collisionShape1;
        private CollisionShape2D _collisionShape2;

        static private Random _random = new Random();

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _collisionShape1 = GetNode<CollisionShape2D>("CollisionShape2D");
            var area2D = GetNode<Area2D>("Area2D");
            area2D.Connect("body_entered", this, nameof(OnBodyEntered));
            _collisionShape2 = area2D.GetNode<CollisionShape2D>("CollisionShape2D");

            if (IsOptional) {
                if (_random.Next(0, 100) >= DisaplayPersantage)
                    QueueFree(); // delete
            }
        }

        private void DropLoot()
        {
            Node loot;

            if (new Random().Next(10) == 0) // 10% chance
            {
                loot = LootManager.CrownScene.Instance();
            }
            else
            {
                if (new Random().Next(3) == 0 || ( GameManager.Instance.Player.CurrentHealth < GameManager.Instance.Player.MaxHealth/3 && new Random().Next(2) == 0)) // 25% chance or if hp is low + coin flip
                {
                    loot = LootManager.HealScene.Instance();
                }
                else
                {
                    loot = LootManager.CoinScene.Instance();
                }
            }

            CallDeferred("add_child", loot);
        }

        private void OnBodyEntered(Node body)
        {
            var bullet = body as Bullet;

            if (bullet == null)
            {
                return;
            }

            _animatedSprite.Play("broken");
            _collisionShape1.SetDeferred("disabled", true);
            _collisionShape2.SetDeferred("disabled", true);

            DropLoot();

            bullet.Pop();
        }
    }
}