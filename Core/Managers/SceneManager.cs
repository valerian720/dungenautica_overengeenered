using Godot;

namespace SibGameJam2021.Core.Managers
{
	public class SceneManager
	{
		private Node _currentSceneInstance;
		private MainMenu _mainMenu;
		private SceneTree _tree;
		private UIManager _uiManager;

		public SceneManager()
		{
			_tree = (SceneTree)Engine.GetMainLoop();
			_mainMenu = _tree.Root.GetNode<MainMenu>("/root/MainMenu"); // start scene
			_uiManager = (UIManager)GD.Load<PackedScene>("res://Scenes/UIManager.tscn").Instance();
		}

		public void LoadDemoLevel()
		{
			LoadLevel("Empty", true);
		}

		public void LoadLevel(string levelName, bool firstLoad = false)
		{
			var level = GD.Load<PackedScene>($"res://Scenes/Levels/{levelName}.tscn").Instance();
			LoadLevel(level, firstLoad);
		}

        public void LoadMainMenu()
        {
            _currentSceneInstance.RemoveChild(GameManager.Instance.Player);
            _currentSceneInstance.QueueFree();
            _tree.Root.RemoveChild(_uiManager);
            _tree.Root.AddChild(_mainMenu);
            _uiManager.ToggleHUD(false);
        }

		private void LoadLevel(Node level, bool firstLoad = false)
		{
			if (firstLoad)
			{
				_tree.Root.RemoveChild(_mainMenu);
				_tree.Root.AddChild(_uiManager);
			}
			else
			{
				_currentSceneInstance.QueueFree();
			}

            _currentSceneInstance = level;
            _currentSceneInstance.AddChild(GameManager.Instance.Player);
            _tree.Root.AddChild(_currentSceneInstance);
            _uiManager.ToggleHUD(true);
        }
    }
}
