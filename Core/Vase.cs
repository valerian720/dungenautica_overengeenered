using Godot;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core
{
    public class Vase : StaticBody2D
    {
        private static readonly PackedScene CoinScene = ResourceLoader.Load<PackedScene>("res://Assets/Prefabs/Coin.tscn");

        private AnimatedSprite _animatedSprite;
        private CollisionShape2D _collisionShape1;
        private CollisionShape2D _collisionShape2;

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _collisionShape1 = GetNode<CollisionShape2D>("CollisionShape2D");
            var area2D = GetNode<Area2D>("Area2D");
            area2D.Connect("body_entered", this, nameof(OnBodyEntered));
            _collisionShape2 = area2D.GetNode<CollisionShape2D>("CollisionShape2D");
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

            var coin = CoinScene.Instance();
            CallDeferred("add_child", coin);

            bullet.QueueFree();
        }
    }
}