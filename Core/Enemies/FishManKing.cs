using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Enemies
{
    public class FishManKing : FishManGuard
    {
        [Export]
        public override float MaxHealth { get; set; } = 1000;

        // TODO special behavour

        private AudioStream fish_attack = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/king_attack.wav");
        private AudioStream fish_death = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/king_death.wav");

        public FishManKing() : base() { }

        protected override void Attack()
        {
            audioPlayer.Stream = fish_attack;
            audioPlayer.Playing = true;
            base.Attack();
        }

        protected override void Die()
        {
            GameManager.Instance.SoundManager.PlayDeathSound(fish_death, Position);
            DropCrown();
            base.Die();
        }

        private void DropCrown()
        {
            var loot = LootManager.CrownScene.Instance();

            Node2D tmp = (Node2D)loot;
            tmp.Position = Position;
            GameManager.Instance.CurrentLevel.AddChild(tmp);
        }
    }
}