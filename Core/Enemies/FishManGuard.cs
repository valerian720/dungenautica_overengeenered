using Godot;

namespace SibGameJam2021.Core.Enemies
{
    public class FishManGuard : FishMan
    {
        // TODO special behavour

        private AudioStream fish_attack = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/guard_attack.wav");
        private AudioStream fish_death = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/guard_death.wav");

        protected override void Attack()
        {
            audioPlayer.Stream = fish_attack;
            base.Attack();
        }
        protected override void Die()
        {
            audioPlayer.Stream = fish_death;
            base.Die();
        }
    }
}