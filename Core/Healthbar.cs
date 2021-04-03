using Godot;

namespace SibGameJam2021.Core
{
    public class Healthbar : Node2D
    {
        private static readonly Texture _greeenBar;
        private static readonly Texture _redBar;
        private static readonly Texture _yellowBar;

        private TextureProgress _textureProgress;

        static Healthbar()
        {
            _greeenBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/barHorizontal_green.png");
            _redBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/barHorizontal_red.png");
            _yellowBar = ResourceLoader.Load<Texture>("res://Assets/Textures/UI/barHorizontal_yellow.png");
        }

        public override void _Ready()
        {
            _textureProgress = GetNode<TextureProgress>("TextureProgress");
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            _textureProgress.Value = currentHealth;

            _textureProgress.TextureProgress_ = currentHealth < maxHealth * 0.7f ? currentHealth < maxHealth * 0.35f ? _redBar : _yellowBar : _greeenBar;
        }
    }
}