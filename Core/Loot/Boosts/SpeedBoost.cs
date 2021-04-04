using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Loot.Boosts
{
    public class SpeedBoost : BoostBase
    {
        public override void ApplyBoost()
        {
            GameManager.Instance.Player.SpeedBoost += 0.1f;
        }
    }
}