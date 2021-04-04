namespace SibGameJam2021.Core.Loot.Boosts
{
    public abstract class BoostBase : LootBase
    {
        public abstract void ApplyBoost();

        protected override void CustomLogic()
        {
            ApplyBoost();

            QueueFree();
        }
    }
}