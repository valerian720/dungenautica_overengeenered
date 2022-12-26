using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot
{
    public class Coin : LootBase
    {
        [Export]
        protected virtual int Value { get; set; } = 1;

        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimatedSprite>("AnimatedSprite").Play();
        }

        protected override void CustomLogic()
        {
            var additionalValue = Value * (int)Mathf.Round(1f + GameManager.Instance.Player.GoldBoost);
            _player.Coins += additionalValue;
            _player.TotalCoins += additionalValue;
        }
    }
}