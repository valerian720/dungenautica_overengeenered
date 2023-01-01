using Godot;

namespace SibGameJam2021.Core.UI
{
    public class HealthBar : Node2D
    {
        private static readonly Texture _greeenBar;
        private static readonly Texture _orangeBar;
        private static readonly Texture _yellowBar;

        private TextureProgress _textureProgress;

        static HealthBar()
        {
            _greeenBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/GreenBar.png");
            _orangeBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/OrangeBar.png");
            _yellowBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/YellowBar.png");
        }

        public override void _Ready()
        {
            _textureProgress = GetNode<TextureProgress>("TextureProgress");
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            GD.Print($"trying update health bar: current: {currentHealth} max: {maxHealth}");
            _textureProgress.Value = currentHealth / maxHealth * 100; // progress bar consumes value 0-100

            _textureProgress.TextureProgress_ = currentHealth < maxHealth * 0.7f ? currentHealth < maxHealth * 0.35f ? _orangeBar : _yellowBar : _greeenBar;
        }
    }
}