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
		private const float DashDelay = 1;
		private const float DashForce = 400;

		private static readonly Dictionary<string, PackedScene> _weaponScenes = PrefabHelper.LoadPrefabsDictionary("res://Assets/Prefabs/Weapons");

		private bool _canDash = true;

		private WeaponBase _currentWeapon = null;
		private Timer _dashTimer = new Timer();
		private Node2D _gunSlot;
		private ReloadBar _reloadBar;
		private Vector2 _velocity = Vector2.Zero;
		private List<WeaponBase> _weapons = _weaponScenes.Select(kv => (WeaponBase)kv.Value.Instance()).ToList();

		public Player() : base()
		{
			_dashTimer.OneShot = true;
			_dashTimer.Connect("timeout", this, nameof(OnDashTimeout));
		}

		public int Coins { get; set; } = 0;

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
				_currentWeapon.StartReloading();
				_reloadBar.StartReloading(_currentWeapon.ReloadDuration);
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

				// применение трения к игроку
				_velocity = _velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * delta);
			}
			else
			{
				// переключение дерева анимации на idle
				_animationState.Travel("Idle");

				// применение трения к игроку
				_velocity = _velocity.MoveToward(Vector2.Zero, FRICTION * delta);
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

			EquipWeapon();

			AddChild(_dashTimer);

			GameManager.Instance.SceneManager.Connect(nameof(SceneManager.OnLevelChange), this, nameof(OnLevelChange));
		}

		public void ApplyImpulse(Vector2 velocity)
		{
			_velocity += velocity;
		}

		protected override void Die()
		{
			GameManager.Instance.SceneManager.LoadMainMenu();
		}

		private void _on_Hitbox_body_entered(Node body)
		{
			if (body.Name.IndexOf("Enemy") == 0)
			{
				GetDamage((body as Enemy).Damage);
				_animationState.Travel("Hurt");
			}
		}

		private void Dash()
		{
			if (!_canDash)
			{
				return;
			}

			var dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

			ApplyImpulse(dir * DashForce);

			_canDash = false;

			_dashTimer.Start(DashDelay);
		}

		private void EquipWeapon(int index = 0)
		{
			if (_currentWeapon != null)
			{
				_gunSlot.RemoveChild(_currentWeapon);
				_reloadBar.InterruptReloading();
			}

			_currentWeapon = _weapons.ElementAt(index);

			_gunSlot.AddChild(_currentWeapon);
			_currentWeapon.Position = Vector2.Zero;
		}

		private void OnDashTimeout()
		{
			_canDash = true;
		}

		private void OnLevelChange()
		{
			_reloadBar.InterruptReloading();
		}

		private void OnReloadBarFinished()
		{
			_currentWeapon.FinishReloading();
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
