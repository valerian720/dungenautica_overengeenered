using System;
using Godot;

namespace SibGameJam2021.Core.Enemies
{
    internal class MovementOnNavigation2D : Node2D
    {
        private static float calculatePerSecond = 2f;
        private Timer _calculatePathTimer = new Timer();
        private bool _canCalucate = true;
        private Navigation2D nav2D = null;
        private Vector2[] path = new Vector2[0];

        public MovementOnNavigation2D(Navigation2D _nav2D)
        {
            nav2D = _nav2D;

            _calculatePathTimer.OneShot = true;
            _calculatePathTimer.Connect("timeout", this, nameof(OnCalculatePathTimer));
        }

        public float CalculateDelay => 1f / calculatePerSecond;

        public override void _Ready()
        {
            AddChild(_calculatePathTimer);
        }

        public Vector2 GetPointTowardsDestiny(Vector2 source, Vector2 destiny)
        {
            Vector2 ret = source; // по умолчанию остается на месте
            // возвращает координаты ближайщей точки по направлению движения

            // TODO не вызывать кучу раз (на каждом кадре), а вызвать, затем пройти некоторый путь по path, затем пересчитать path
            // или мб добавить просчет пути по таймеру каждую секунду (?)
            if (_canCalucate)
            {
                try
                {
                    path = nav2D.GetSimplePath(source, destiny);
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
    }
}