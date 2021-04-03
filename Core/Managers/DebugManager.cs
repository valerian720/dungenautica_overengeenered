using Godot;

namespace SibGameJam2021.Core.Managers
{
    public class DebugManager : CanvasLayer
    {
        private Control _debugOverlay;
        private int _firstElement = 0;
        private int _minArgumentToConsoleCount = 2;

        public override void _Input(InputEvent @event)
        {
            if (Input.IsKeyPressed((int)KeyList.F1) && @event.IsPressed() && !@event.IsEcho())
            {
                _debugOverlay.Visible = !_debugOverlay.Visible;
            }
        }

        public override void _Ready()
        {
            SetProcessInput(OS.IsDebugBuild());

            _debugOverlay = GetNode<Control>("DebugOverlay");
            GetNode<LineEdit>("DebugOverlay/ConsoleInput").Connect("text_entered", this, nameof(OnConsoleInput));
        }

        public void OnConsoleInput(string text)
        {
            GD.Print(text);

            string[] splittedText = text.Split(" ");

            if (splittedText.Length >= _minArgumentToConsoleCount)
            {
                switch (splittedText[_firstElement])
                {
                    case "lvl":
                        splittedText[_firstElement] = "";
                        string levelName = string.Join(" ", splittedText);
                        try
                        {
                            GameManager.Instance.SceneManager.LoadLevel(levelName.Substr(1, levelName.Length - 1));
                        }
                        catch
                        {
                            throw;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}