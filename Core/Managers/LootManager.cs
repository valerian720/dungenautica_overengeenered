using System.Collections.Generic;
using Godot;
using SibGameJam2021.Core.Utility;

namespace SibGameJam2021.Core.Managers
{
    public class LootManager
    {
        public static Dictionary<string, PackedScene> Boosts { get; } = PrefabHelper.LoadPrefabsDictionary("res://Assets/Prefabs/Boosts");
        public static PackedScene CoinScene { get; } = ResourceLoader.Load<PackedScene>("res://Assets/Prefabs/Coin.tscn");
        public static PackedScene CrownScene { get; } = ResourceLoader.Load<PackedScene>("res://Assets/Prefabs/Crown.tscn");
        public static PackedScene HealScene { get; } = ResourceLoader.Load<PackedScene>("res://Assets/Prefabs/Heal.tscn");
    }
}