﻿using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot.Boosts
{
    public class GoldBoost : BoostBase
    {
        protected override void ApplyBoost()
        {
            GameManager.Instance.Player.GoldBoost += 0.1f;
        }
    }
}