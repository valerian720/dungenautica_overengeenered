using System.Collections.Generic;
using System.Linq;
using Godot;
using SibGameJam2021.Core.Enemies;
using SibGameJam2021.Core.Managers;
using SibGameJam2021.Core.UI;
using SibGameJam2021.Core.Utility;
using SibGameJam2021.Core.Weapons;

namespace SibGameJam2021.Core
{
    public class Player : Entity
    {
        public float _ammoBoost = 0;
        public int _bounceBoost = 0;
        public float _damageBoost = 0;
        public float _goldBoost = 0;
        public float _speedBoost = 0;
        private const float DashDelay = 1;
        private const float DashForce = 400;

        private const float MaxHealthDefault = 100f;
        private static readonly Dictionary<string, PackedScene> _weaponScenes = PrefabHelper.LoadPrefabsDictionary("res://Assets/Prefabs/Weapons");
        private bool _canDash = true;
        private int _coins = 0;
        private int _totalCoins = 0;
        private WeaponBase _currentWeapon = null;
        private Timer _dashTimer = new Timer();
        private Node2D _gunSlot;
        private Area2D _hitbox;
        private int _lifes = 3;
        private float _maxHealth = MaxHealthDefault;
        private ReloadBar _reloadBar;
        private Vector2 _velocity = Vector2.Zero;
        private List<WeaponBase> _weapons = _weaponScenes.Select(kv => (WeaponBase)kv.Value.Instance()).ToList();

        private AudioStreamPlayer2D audioPlayer = null;
        private AudioStream player_hurt = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/player_hurt.wav");

        public Player() : base()
        {
            _dashTimer.OneShot = true;
            _dashTimer.Connect("timeout", this, nameof(OnDashTimeout));
        }

        public float AmmoBoost
        {
            get { return _ammoBoost; }

            set
            {
                _ammoBoost = value;
                GameManager.Instance.UIManager.UpdateAmmoBoost(_ammoBoost);
            }
        }

        public int BounceBoost
        {
            get { return _bounceBoost; }

            set
            {
                _bounceBoost = value;
                GameManager.Instance.UIManager.UpdateBounceBoost(_bounceBoost);
            }
        }

        public int Coins
        {
            get { return _coins; }

            set
            {
                _coins = value;
                GameManager.Instance.UIManager.UpdateGoldCount(_coins);
            }
        }

        public int TotalCoins
        {
            get { return _totalCoins; }

            set
            {
                _totalCoins = value;
                GameManager.Instance.UIManager.UpdateTotalGoldCount(_totalCoins);
            }
        }

        public override float CurrentHealth
        {
            get { return _currentHealth; }

            protected set
            {
                base.CurrentHealth = value > _maxHealth ? _maxHealth : (value < 0 ? 0 : value);
                GameManager.Instance.UIManager.UpdateHealth(_currentHealth, _maxHealth);
            }
        }

        public float DamageBoost
        {
            get { return _damageBoost; }

            set
            {
                _damageBoost = value;
                GameManager.Instance.UIManager.UpdateDamageBoost(_damageBoost);
            }
        }

        public float GoldBoost
        {
            get { return _goldBoost; }

            set
            {
                _goldBoost = value;
                GameManager.Instance.UIManager.UpdateGoldBoost(_goldBoost);
            }
        }

        public int Lifes
        {
            get { return _lifes; }

            set
            {
                _lifes = value;
                GameManager.Instance.UIManager.UpdateLifesCount(_lifes);
            }
        }

        public override float MaxHealth
        {
            get { return _maxHealth; }

            set
            {
                _maxHealth = value;
                GameManager.Instance.UIManager.UpdateHealth(_currentHealth, _maxHealth);
            }
        }

        public float SpeedBoost
        {
            get { return _speedBoost; }

            set
            {
                _speedBoost = value;
                GameManager.Instance.UIManager.UpdateSpeedBoost(_speedBoost);
            }
        }

        public int BulletsLeft
        {
            get { return _currentWeapon.AmmoCount; }
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("slot1"))
            {
                EquipWeapon(0);
            }
            if (inputEvent.IsActionPressed("slot2"))
            {
                EquipWeapon(1);
            }
            if (inputEvent.IsActionPressed("slot3"))
            {
                EquipWeapon(2);
            }
            if (inputEvent.IsActionPressed("ui_fire"))
            {
                _currentWeapon.StartShooting();
            }
            if (inputEvent.IsActionPressed("reload"))
            {
                RunReload();
            }
            if (inputEvent.IsActionPressed("dash"))
            {
                Dash();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            // получение вектора движения игрока
            Vector2 inputVector = Vector2.Zero;
            inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
            inputVector = inputVector.Normalized();

            if (!inputVector.IsEqualApprox(Vector2.Zero))
            {
                // передача текущей скорости игрока в дерево анимации
                _animationTree.Set("parameters/Idle/blend_position", inputVector);
                _animationTree.Set("parameters/Run/blend_position", inputVector);

                // переключение дерева анимации на бег
                _animationState.Travel("Run");
                audioPlayer.Stream = player_hurt;

                // применение трения к игроку
                _velocity = _velocity.MoveToward(inputVector * MaxSpeed * Mathf.Round(1f + SpeedBoost), Acceleration * delta);
            }
            else
            {
                // переключение дерева анимации на idle
                _animationState.Travel("Idle");

                // применение трения к игроку
                _velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);
            }

            _velocity = MoveAndSlide(_velocity); // скольжение вдоль коллайдера

            UpdateWeaponPosition();
        }

        public override void _Ready()
        {
            base._Ready();

            _reloadBar = GetNode<ReloadBar>("ReloadBar");
            _reloadBar.Connect(nameof(ReloadBar.ReloadFinished), this, nameof(OnReloadBarFinished));

            _gunSlot = GetNode<Node2D>("GunSlot"); // подгрузка ссылки на слот для оружия

            _hitbox = GetNode<Area2D>("Hitbox");
            _hitbox.Connect("area_entered", this, nameof(OnHitboxAreaEntered));

            UpdateHUD();

            EquipWeapon();

            AddChild(_dashTimer);

            GameManager.Instance.SceneManager.Connect(nameof(SceneManager.OnLevelChange), this, nameof(OnLevelChange));

            audioPlayer = new AudioStreamPlayer2D();
            AddChild(audioPlayer);
            audioPlayer.Playing = true;
        }

        public void ApplyImpulse(Vector2 velocity)
        {
            _velocity += velocity;
        }

        public void RunReload()
        {
            if(_currentWeapon.StartReloading())
            _reloadBar.StartReloading(_currentWeapon.ReloadDuration);
        }

        public void IncreaseHealth(float value)
        {
            CurrentHealth += value;
        }


        public void Reset()
        {
            MaxHealth = MaxHealthDefault;
            CurrentHealth = MaxHealth;
            Lifes = 3;
            Coins = 0;
            TotalCoins = 0;
            DamageBoost = 0;
            SpeedBoost = 0;
            GoldBoost = 0;
            AmmoBoost = 0;
            BounceBoost = 0;

            foreach (var weapon in _weapons)
            {
                weapon.FinishReloading();
            }

            _velocity = Vector2.Zero;

            EquipWeapon(0);

            GameManager.Instance.SceneManager.LevelCount = 0;
        }

        protected override void Die()
        {
            Lifes--;

            CurrentHealth = MaxHealth;

            GameManager.Instance.SoundManager.PlayPlayerHurtSound();

            if (Lifes <= 0)
            {
                GameManager.Instance.SceneManager.LoadMainMenu();
            }
        }

        private void Dash()
        {
            if (!_canDash)
            {
                return;
            }

            //var dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
            var dir = _velocity.Normalized(); // деш в сторону, куда движется игрок

            ApplyImpulse(dir * DashForce);

            _canDash = false;

            _dashTimer.Start(DashDelay);
        }

        private void EquipWeapon(int index = 0)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.InterruptReloading();
                _gunSlot.RemoveChild(_currentWeapon);
                _reloadBar.InterruptReloading();
            }

            _currentWeapon = _weapons.ElementAt(index);

            _gunSlot.AddChild(_currentWeapon);
            _currentWeapon.Position = Vector2.Zero;
            GameManager.Instance.UIManager.UpdateAmmoCount(_currentWeapon.AmmoCount);
        }

        private void OnDashTimeout()
        {
            _canDash = true;
        }

        private void OnHitboxAreaEntered(Area2D area)
        {
            if (area.Name.Equals("AttackBox"))
            {
                GetDamage((area.GetParent() as Enemy).Damage);
                _animationState.Travel("Hurt");
                audioPlayer.Stream = player_hurt;
                audioPlayer.Playing = true;
            }
        }

        private void OnLevelChange()
        {
            _reloadBar.InterruptReloading();
        }

        private void OnReloadBarFinished()
        {
            _currentWeapon.FinishReloading();
        }

        private void UpdateHUD()
        {
            var uiManager = GameManager.Instance.UIManager;

            uiManager.UpdateHealth(CurrentHealth, MaxHealth);
            uiManager.UpdateGoldCount(Coins);
            uiManager.UpdateTotalGoldCount(TotalCoins);
            uiManager.UpdateLifesCount(Lifes);
            uiManager.UpdateLevelCount(GameManager.Instance.SceneManager.LevelCount);
            uiManager.UpdateDamageBoost(DamageBoost);
            uiManager.UpdateSpeedBoost(SpeedBoost);
            uiManager.UpdateGoldBoost(GoldBoost);
            uiManager.UpdateAmmoBoost(AmmoBoost);
            uiManager.UpdateBounceBoost(BounceBoost);
        }

        public bool HasEquiped(int index)
        {
            return _currentWeapon == _weapons.ElementAt(index);
        }

        private void UpdateWeaponPosition()
        {
            var mousePos = GetGlobalMousePosition();

            if (mousePos.x > _gunSlot.GlobalPosition.x)
            {
                _currentWeapon.LookRight();
            }
            else
            {
                _currentWeapon.LookLeft();
            }

            _gunSlot.LookAt(mousePos);
        }
    }
}