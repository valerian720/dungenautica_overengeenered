﻿using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot.Boosts
{
    public class DamageBoost : BoostBase
    {
        public override void ApplyBoost()
        {
            GameManager.Instance.Player.DamageBoost += 0.1f;
        }
    }
}