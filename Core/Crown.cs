using Godot;

namespace SibGameJam2021.Core
{
    public class Crown : Coin
    {
        [Export]
        protected override int Value { get; set; } = 10;
    }
}