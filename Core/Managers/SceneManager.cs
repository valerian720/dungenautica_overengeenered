using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SibGameJam2021.Core.Enemies;
using SibGameJam2021.Core.UI;
using SibGameJam2021.Core.Utility;

namespace SibGameJam2021.Core.Managers
{
    public class SceneManager : Node2D
    {
        public const int ShopLevelInterval = 4;
        private static readonly Dictionary<string, PackedScene> _levels = PrefabHelper.LoadPrefabsDictionary("res://Scenes/Levels", new string[] { "Empty", "shop" });

        private static PackedScene _shopLevel = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/shop.tscn");

        private MainMenu _mainMenu;
        private SceneTree _tree;
        private UIManager _uiManager;
        private SoundManager _soundManager;

        public SceneManager()
        {
            _tree = (SceneTree)Engine.GetMainLoop();
            _mainMenu = _tree.Root.GetNode<MainMenu>("/root/MainMenu"); // start scene
            _uiManager = (UIManager)GD.Load<PackedScene>("res://Scenes/UIManager.tscn").Instance();

            _soundManager = new SoundManager();
            _tree.Root.AddChild(_soundManager);
        }

        [Signal]
        public delegate void OnLevelChange();

        public Level CurrentLevel { get; private set; } = null;

        public SoundManager SoundManager { get => _soundManager; }

        public int LevelCount { get; set; } = 0;

        public void LoadDemoLevel()
        {
            LoadLevel("Empty");
        }

        public void LoadLevel(string levelName)
        {
            Node level;

            if (levelName == "shop")
            {
                level = _shopLevel.Instance();
            }
            else
            {
                level = _levels[levelName].Instance();
            }

            LoadLevel(level);
        }

        public void LoadMainMenu()
        {
            GameManager.Instance.Player.Reset();

            CurrentLevel.RemovePlayer();
            CurrentLevel.QueueFree();
            CurrentLevel = null;

            _tree.Root.RemoveChild(_uiManager);
            _tree.Root.AddChild(_mainMenu);
            _uiManager.ToggleHUD(false);
            _soundManager.PlayBGMusic();
        }

        public void LoadRandomLevel()
        {
            LevelCount++;

            if (LevelCount % ShopLevelInterval == 0)
            {
                LoadLevel(_shopLevel.Instance());

            }
            else
            {
                LoadLevel(_levels.ElementAt(new Random().Next(0, _levels.Count())).Value.Instance());
            }
        }

        public void StartNewGame()
        {
            SoundManager.CasheRandomMusic();
            LoadRandomLevel();
        }

        private void LoadLevel(Node level)
        {
            EmitSignal(nameof(OnLevelChange));

            if (CurrentLevel == null)
            {
                _tree.Root.RemoveChild(_mainMenu);
                _tree.Root.AddChild(_uiManager);
            }
            else
            {
                CurrentLevel.RemovePlayer();
                CurrentLevel.QueueFree();
            }

            CurrentLevel = (Level)level;
            _tree.Root.CallDeferred("add_child", CurrentLevel);
            CurrentLevel.CallDeferred(nameof(CurrentLevel.SpawnPlayer));

            _uiManager.ToggleHUD(true);
        }


    }
}