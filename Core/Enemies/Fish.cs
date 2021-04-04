using Godot;

namespace SibGameJam2021.Core.Enemies
{
    public class Fish : Enemy
    {
        // TODO special behavour
        private AudioStream fish_attack = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/fish_attack.wav");

        public override void Attack()
        {
            audioPlayer.Stream = fish_attack;
            base.Attack();
        }
    }
}