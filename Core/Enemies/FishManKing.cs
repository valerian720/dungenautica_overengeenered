using Godot;

namespace SibGameJam2021.Core.Enemies
{
    public class FishManKing : FishManGuard
    {
        // TODO special behavour

        private AudioStream fish_attack = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/king_attack.wav");
        private AudioStream fish_death = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/king_death.wav");

        protected override void Attack()
        {
            audioPlayer.Stream = fish_attack;
            audioPlayer.Playing = true;
            base.Attack();
        }
        protected override void Die()
        {
            audioPlayer.Stream = fish_death;
            audioPlayer.Playing = true;
            base.Die();
        }
    }
}