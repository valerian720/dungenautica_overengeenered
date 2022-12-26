using Godot;

namespace SibGameJam2021.Core.Managers
{
    public class UIManager : CanvasLayer
    {
        private const int AmmoBubbleSize = 4;
        private const int HeartSize = 8;
        private const int HeartValue = 10;

        private Label _ammoBoostLabel;
        private TextureRect _ammoTexture;
        private Label _bounceBoostLabel;
        private Label _damageBoostLabel;
        private Label _goldBoostLabel;
        private Label _goldLabel;
        private Label _totalGoldLabel;
        private Label _levelLabel;
        private TextureRect _healthEmptyTexture;
        private TextureRect _healthFullTexture;
        private Control _hud;
        private Label _lifesLabel;
        private Control _pauseMenu;
        private Label _speedBoostLabel;

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

            _ammoTexture = GetNode<TextureRect>("HUD/LowerLeft/AmmoBar");
            _healthEmptyTexture = GetNode<TextureRect>("HUD/LowerLeft/HealthEmpty");
            _healthFullTexture = GetNode<TextureRect>("HUD/LowerLeft/HealthEmpty/HealthFull");

            _goldLabel = GetNode<Label>("HUD/UpperLeft/GoldBar/GoldLabel");
            _totalGoldLabel = GetNode<Label>("HUD/UpperLeft/GoldBar/TotalGoldLabel");
            
            _lifesLabel = GetNode<Label>("HUD/UpperLeft/LifeBar/LifesLabel");

            _levelLabel = GetNode<Label>("HUD/UpperLeft/LevelBar/LevelLabel");

             _damageBoostLabel = GetNode<Label>("HUD/UpperLeft/BoostsBar/DamageBoostLabel");
            _goldBoostLabel = GetNode<Label>("HUD/UpperLeft/BoostsBar/GoldBoostLabel");
            _speedBoostLabel = GetNode<Label>("HUD/UpperLeft/BoostsBar/SpeedBoostLabel");
            _ammoBoostLabel = GetNode<Label>("HUD/UpperLeft/BoostsBar/AmmoBoostLabel");
            _bounceBoostLabel = GetNode<Label>("HUD/UpperLeft/BoostsBar/BounceBoostLabel");

            GameManager.Instance.UIManager = this;
        }

        public void ToggleHUD(bool visible)
        {
            _hud.Visible = visible;
            GameManager.Instance.UIManager.UpdateLevelCount(GameManager.Instance.SceneManager.LevelCount);
        }

        public void UpdateAmmoBoost(float value)
        {
            if (Mathf.IsEqualApprox(value, 0))
            {
                _ammoBoostLabel.Visible = false;
            }
            else
            {
                _ammoBoostLabel.Visible = true;
                _ammoBoostLabel.Text = $"Ammo: +{value * 100}%";
            }
        }

        public void UpdateAmmoCount(int count)
        {
            var rectSize = new Vector2(count * AmmoBubbleSize, AmmoBubbleSize);

            _ammoTexture.RectSize = rectSize;

            var rect = new Rect2(0, 0, rectSize);

            ((AtlasTexture)_ammoTexture.Texture).Region = rect;
        }

        public void UpdateBounceBoost(int value)
        {
            if (value == 0)
            {
                _bounceBoostLabel.Visible = false;
            }
            else
            {
                _bounceBoostLabel.Visible = true;
                _bounceBoostLabel.Text = $"Bounces: {value}";
            }
        }

        public void UpdateDamageBoost(float value)
        {
            if (Mathf.IsEqualApprox(value, 0))
            {
                _damageBoostLabel.Visible = false;
            }
            else
            {
                _damageBoostLabel.Visible = true;
                _damageBoostLabel.Text = $"Damage: +{value * 100}%";
            }
        }

        public void UpdateGoldBoost(float value)
        {
            if (Mathf.IsEqualApprox(value, 0))
            {
                _goldBoostLabel.Visible = false;
            }
            else
            {
                _goldBoostLabel.Visible = true;
                _goldBoostLabel.Text = $"Gold: +{value * 100}%";
            }
        }

        public void UpdateGoldCount(int count)
        {
            _goldLabel.Text = count.ToString();
        }

        public void UpdateTotalGoldCount(int count)
        {
            _totalGoldLabel.Text = count.ToString();
        }
        

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            var emptyHeartsCount = Mathf.Round(maxHealth / HeartValue);
            var halfHeartsCount = Mathf.Round(currentHealth / HeartValue * 2);

            ((AtlasTexture)_healthEmptyTexture.Texture).Region = new Rect2(0, 0, emptyHeartsCount * HeartSize, HeartSize);
            ((AtlasTexture)_healthFullTexture.Texture).Region = new Rect2(0, 0, halfHeartsCount * HeartSize / 2, HeartSize);
        }

        public void UpdateLifesCount(int count)
        {
            _lifesLabel.Text = count.ToString();
        }
        
        public void UpdateLevelCount(int count)
        {
            _levelLabel.Text = count.ToString();
        }

        public void UpdateSpeedBoost(float value)
        {
            if (Mathf.IsEqualApprox(value, 0))
            {
                _speedBoostLabel.Visible = false;
            }
            else
            {
                _speedBoostLabel.Visible = true;
                _speedBoostLabel.Text = $"Speed: +{value * 100}%";
            }
        }

        private void TogglePause()
        {
            var tree = GetTree();
            tree.Paused = !tree.Paused;
            _pauseMenu.Visible = tree.Paused;
        }
    }
}