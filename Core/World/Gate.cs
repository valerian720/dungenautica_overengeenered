using System;
using System.Linq;
using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.World
{
    public class Gate : StaticBody2D
    {
        private AnimatedSprite _animatedSprite;
        private Area2D _area2D;
        private CollisionShape2D _collisionShape;
        private CollisionShape2D _enterCollisionShape;

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, nameof(OnOpened));

            _area2D = GetNode<Area2D>("Area2D");
            _area2D.Connect("body_entered", this, nameof(OnBodyEntered));

            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
            _enterCollisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

            _collisionShape.SetDeferred("disabled", false);
            _enterCollisionShape.SetDeferred("disabled", true);
        }

        public void OnBodyEntered(Node body)
        {
            var player = body as Player;

            if (player == null)
            {
                return;
            }

            if (_collisionShape.Disabled)
            {
                _area2D.Disconnect("body_entered", this, nameof(OnBodyEntered));

                GameManager.Instance.SceneManager.LoadRandomLevel();
            }
        }

        public void Open()
        {
            _animatedSprite.Play();
        }

        private void OnOpened()
        {
            var boosts = LootManager.Boosts;

            var count = boosts.Count() + 1;

            var index = new Random().Next(count);

            Node loot;

            if (index == count - 1)
            {
                loot = LootManager.CrownScene.Instance();
            }
            else
            {
                loot = boosts.ElementAt(index).Value.Instance();
            }

            CallDeferred("add_child", loot);

            _collisionShape.SetDeferred("disabled", true);
            _enterCollisionShape.SetDeferred("disabled", false);
        }
    }
}