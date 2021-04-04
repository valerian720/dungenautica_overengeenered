using Godot;

namespace SibGameJam2021.Core.Managers
{
    public class UIManager : CanvasLayer
    {
        private const int AmmoBubbleSize = 4;
        private const int HeartSize = 8;

        private TextureRect _ammoTexture;
        private TextureRect _healthTexture;
        private Control _hud;
        private Control _pauseMenu;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                TogglePause();
            }
        }

        public void _on_MainMenuButton_pressed()
        {
            TogglePause();
            GameManager.Instance.SceneManager.LoadMainMenu();
        }

        public void _on_PauseButton_pressed()
        {
            TogglePause();
        }

        public void _on_ResumeButton_pressed()
        {
            var tree = GetTree();
            tree.Paused = false;
            _pauseMenu.Visible = false;
        }

        public void _on_SaveButton_pressed()
        {
        }

        public override void _Ready()
        {
            _hud = GetNode<Control>("HUD");
            _pauseMenu = GetNode<Control>("PauseMenu");
            _ammoTexture = GetNode<TextureRect>("HUD/VBoxContainer/AmmoBar");
            _healthTexture = GetNode<TextureRect>("HUD/VBoxContainer/HealthEmpty/HealthFull");

            GameManager.Instance.UIManager = this;
        }

        public void ToggleHUD(bool visible)
        {
            _hud.Visible = visible;
        }

        public void UpdateAmmoCount(int count)
        {
            ((AtlasTexture)_ammoTexture.Texture).Region = new Rect2(0, 0, count * AmmoBubbleSize, AmmoBubbleSize);
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            var halfHeartValue = maxHealth / 20;
            var halfHeartsCount = Mathf.Round(currentHealth / halfHeartValue);

            ((AtlasTexture)_healthTexture.Texture).Region = new Rect2(0, 0, halfHeartsCount * HeartSize / 2, HeartSize);
        }

        private void TogglePause()
        {
            var tree = GetTree();
            tree.Paused = !tree.Paused;
            _pauseMenu.Visible = tree.Paused;
        }
    }
}