﻿using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Enemies
{
    public class Fish : Enemy
    {
        [Export]
        public override float MaxHealth { get; set; } = 100;

        // TODO special behavour
        private AudioStream fish_attack = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/fish_attack.wav");
        private AudioStream fish_death = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/fish_death.wav");

        public Fish() : base() { }

        protected override void Attack()
        {
            audioPlayer.Stream = fish_attack;
            audioPlayer.Playing = true;
            base.Attack();
        }

        protected override void Die()
        {
            GameManager.Instance.SoundManager.PlayDeathSound(fish_death, Position);

            base.Die();
        }
    }
}