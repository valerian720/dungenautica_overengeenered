using System;
using Godot;
using SibGameJam2021.Core.Managers;

namespace SibGameJam2021.Core.Enemies
{
    internal class MovementOnNavigation2D : Node2D
    {
        private static float calculatePerSecond = 2f;
        private Timer _calculatePathTimer = new Timer();
        private bool _canCalucate = true;
        private Navigation2D nav2D = null;
        private Vector2[] path = new Vector2[0];

        private Line2D _pathVisualized = new Line2D();


        public MovementOnNavigation2D(Navigation2D _nav2D)
        {
            nav2D = _nav2D;

            _calculatePathTimer.OneShot = true;
            _calculatePathTimer.Connect("timeout", this, nameof(OnCalculatePathTimer));

            _pathVisualized.GlobalPosition = Vector2.Zero;
            _pathVisualized.Width = 1;
        }

        public float CalculateDelay => 1f / calculatePerSecond;

        public override void _Ready()
        {
            AddChild(_calculatePathTimer);
            GameManager.Instance.CurrentLevel.AddChild(_pathVisualized);
        }

        public Vector2 GetPointTowardsDestiny(Vector2 source, Vector2 destiny)
        {
            Vector2 ret = source; // по умолчанию остается на месте
            // возвращает координаты ближайщей точки по направлению движения

            if (_canCalucate)
            {
                try
                {
                    path = nav2D.GetSimplePath(source, destiny, true);
                    //DebugPath();

                    _canCalucate = false;
                    _calculatePathTimer.Start(CalculateDelay);
                }
                catch (Exception e)
                {
                    GD.PrintErr(e);
                }
            }

            if (path.Length > 1) // блинблять
            {
                ret = path[1];
            }

            return ret;
        }

        private void OnCalculatePathTimer()
        {
            _canCalucate = true;
        }


        public void DebugPath()
        {
            GD.Print(path[1]);

            _pathVisualized.Points = path;

        }

        public void DebugPathClearPoints()
        {
            _pathVisualized.ClearPoints();
        }

        public void PopResources()
        {
            RemoveChild(_calculatePathTimer);
            GameManager.Instance.CurrentLevel.RemoveChild(_pathVisualized);
        }
    }
}