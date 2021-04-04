using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot.Boosts
{
    public class LifeBoost : BoostBase
    {
        public override void ApplyBoost()
        {
            GameManager.Instance.Player.Lifes++;
        }
    }
}