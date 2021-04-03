using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SibGameJam2021.Core.Managers
{
    public class SceneManager
    {
        private static readonly Dictionary<string, PackedScene> _levels = new Dictionary<string, PackedScene>();
        private Level _currentLevel = null;
        private MainMenu _mainMenu;
        private SceneTree _tree;
        private UIManager _uiManager;

        static SceneManager()
        {
            var dir = new Directory();
            var path = "res://Scenes/Levels";

            if (dir.Open(path) == Error.Ok)
            {
                dir.ListDirBegin();
                var filename = dir.GetNext();

                while (!string.IsNullOrEmpty(filename))
                {
                    if (dir.CurrentIsDir())
                    {
                        filename = dir.GetNext();
                        continue;
                    }

                    //GD.Print(filename);

                    var level = GD.Load<PackedScene>($"{path}/{filename}");
                    _levels.Add(System.IO.Path.GetFileNameWithoutExtension(filename), level);

                    filename = dir.GetNext();
                }
            }
        }

        public SceneManager()
        {
            _tree = (SceneTree)Engine.GetMainLoop();
            _mainMenu = _tree.Root.GetNode<MainMenu>("/root/MainMenu"); // start scene
            _uiManager = (UIManager)GD.Load<PackedScene>("res://Scenes/UIManager.tscn").Instance();
        }

        public void LoadDemoLevel()
        {
            LoadLevel("Empty");
        }

        public void LoadLevel(string levelName)
        {
            var level = _levels[levelName].Instance();
            LoadLevel(level);
        }

        public void LoadMainMenu()
        {
            _currentLevel.RemovePlayer();
            _currentLevel.QueueFree();
            _currentLevel = null;

            _tree.Root.RemoveChild(_uiManager);
            _tree.Root.AddChild(_mainMenu);
            _uiManager.ToggleHUD(false);
        }

        public void LoadRandomLevel()
        {
            LoadLevel(_levels.ElementAt(new Random().Next(0, _levels.Count())).Value.Instance());
        }

        private void LoadLevel(Node level)
        {
            if (_currentLevel == null)
            {
                _tree.Root.RemoveChild(_mainMenu);
                _tree.Root.AddChild(_uiManager);
            }
            else
            {
                _currentLevel.RemovePlayer();
                _currentLevel.QueueFree();
            }

            _currentLevel = (Level)level;
            _tree.Root.CallDeferred("add_child", _currentLevel);
            _currentLevel.CallDeferred(nameof(_currentLevel.SpawnPlayer));

            _uiManager.ToggleHUD(true);
        }
    }
}