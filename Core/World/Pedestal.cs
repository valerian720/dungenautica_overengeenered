using System;
using System.Linq;
using Godot;
using SibGameJam2021.Core.Loot.Boosts;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.World
{
    public class Pedestal : StaticBody2D
    {
        private BoostBase _boost;
        private Node2D _boostSpawn;
        private CollisionShape2D _pickCollider;

        [Export]
        private int _price = 10;

        [Export]
        private int _perLevelOverprice = 10;

        private HBoxContainer _priceInfo;

        public override void _Ready()
        {
            _boost = LootManager.Boosts.ElementAt(new Random().Next(LootManager.Boosts.Count())).Value.Instance() as BoostBase;
            _boost.Enabled = false;

            _boostSpawn = GetNode<Node2D>("BoostSpawn");
            _boostSpawn.AddChild(_boost);

            _pickCollider = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

            _price += GameManager.Instance.SceneManager.LevelCount / SceneManager.ShopLevelInterval * _perLevelOverprice; // auto level prices
            _priceInfo = GetNode<HBoxContainer>("PriceInfo");
            _priceInfo.GetNode<Label>("Label").Text = _price.ToString();

            GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnBodyEntered));
        }

        private void OnBodyEntered(Node body)
        {
            var player = body as Player;

            if (player == null)
            {
                return;
            }

            if (player.Coins >= _price)
            {
                player.Coins -= _price;

                _boost.ApplyBoost();
                _boost.CallDeferred("queue_free");

                _priceInfo.Visible = false;

                _pickCollider.SetDeferred("disabled", true);
            }
        }
    }
}