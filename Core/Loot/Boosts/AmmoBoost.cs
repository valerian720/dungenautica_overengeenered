using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot.Boosts
{
    public class AmmoBoost : BoostBase
    {
        public override void ApplyBoost()
        {
            GameManager.Instance.Player.AmmoBoost += 0.1f;
        }
    }
}