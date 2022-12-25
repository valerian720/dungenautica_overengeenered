using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot
{
    public class Heal : LootBase
    {
        [Export]
        protected virtual int Value { get; set; } = 25;

        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimatedSprite>("AnimatedSprite").Play();
        }

        protected override void CustomLogic()
        {
            _player.IncreaseHealth(Value);
        }
    }
}