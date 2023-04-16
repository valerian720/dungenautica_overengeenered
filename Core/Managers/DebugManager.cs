using Godot;
using System;
using System.Collections.Generic;

namespace SibGameJam2021.Core.Managers
{
    public class DebugManager : CanvasLayer
    {
        private Control _debugOverlay;
        private TextEdit _consoleOutput;
        private int _minArgumentToConsoleCount = 2;

        private Dictionary<string, Func<string, string>> _handlers = new Dictionary<string, Func<string, string>>();

        public override void _Input(InputEvent @event)
        {
            if (Input.IsKeyPressed((int)KeyList.F1) && @event.IsPressed() && !@event.IsEcho())
            {
                _debugOverlay.Visible = !_debugOverlay.Visible;
                var ui = GameManager.Instance.UIManager;
                if (ui != null)
                {
                    //ui.DebugTogglePause();
                }
            }
        }

        private void ScrollToCurrent()
        {
            _consoleOutput.ScrollVertical = _consoleOutput.GetLineCount();
        }

        public void DebugPrint(string text)
        {
            _consoleOutput.Text += $"> {text}\n";
            ScrollToCurrent();

        }

        public override void _Ready()
        {
            SetProcessInput(OS.IsDebugBuild());

            _debugOverlay = GetNode<Control>("DebugOverlay");
            _consoleOutput = GetNode<TextEdit>("DebugOverlay/ConsoleOutput");
            
            GetNode<LineEdit>("DebugOverlay/ConsoleInput").Connect("text_entered", this, nameof(OnConsoleInput));
            PrepareFunctionBinding();

            DebugPrint("Debug console is ready");
            DebugPrint("everything in lower case");
            DebugPrint("print help to list all commands");
        }

        private void PrepareFunctionBinding()
        {
            _handlers.Add("lvl", DebugChangeLevel);
            _handlers.Add("gold", DebugSetGold);
            _handlers.Add("health", DebugSetHealth);

            _handlers.Add("help", DebugHelp);
            _handlers.Add("list-levels", DebugListLevels);
        }

        public void OnConsoleInput(string text)
        {
            GD.Print(text);

            string[] splittedText = text.Split(" ");

            if (splittedText.Length >= _minArgumentToConsoleCount)
            {
                DebugPrint(_handlers[splittedText[0]](splittedText[1]));
            }
            else
            {
                DebugPrint(_handlers[splittedText[0]](""));
            }
        }

        //

        private string DebugChangeLevel(string levelName)
        {
            
            GameManager.Instance.SceneManager.LoadLevel(levelName);
            return "processing level change";
        }

        private string DebugSetGold(string goldCount)
        {
            if (int.TryParse(goldCount, out var gold))
            {
                GameManager.Instance.Player.Coins = gold;
                GameManager.Instance.Player.TotalCoins = gold;
            }
            return "processing gold change";
        }

        private string DebugSetHealth(string healthCount)
        {
            if (int.TryParse(healthCount, out var health))
            {
                GameManager.Instance.Player.ForceSetHealth(health);
            }
            return "processing health change";
        }


        private string DebugHelp(string param)
        {
            DebugPrint("\ncommands:");
            foreach (string key in _handlers.Keys)
            {
                DebugPrint(key);
            }
            return "type help to display it again";
        }


        private string DebugListLevels(string param)
        {
            DebugPrint("\nall levels:");
            foreach (string levelName in GameManager.Instance.SceneManager.ListLevels())
            {
                DebugPrint($"lvl {levelName}");
            }
            return "type list-levels to dosplay it again";
        }

        
    }
}