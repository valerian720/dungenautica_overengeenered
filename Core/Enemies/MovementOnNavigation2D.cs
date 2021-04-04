using Godot;
using System;

namespace SibGameJam2021.Core.Enemies
{
    class MovementOnNavigation2D : Node2D
    {
        private Navigation2D nav2D = null;
        private Vector2[] path = new Vector2[0];

        private Timer _calculatePathTimer = new Timer();
        private bool _canCalucate = true;

        private static float calculatePerSecond = 2f;
        public float CalculateDelay => 1f / calculatePerSecond;
        public MovementOnNavigation2D(Navigation2D _nav2D)
        {
            nav2D = _nav2D;

            _calculatePathTimer.OneShot = true;
            _calculatePathTimer.Connect("timeout", this, nameof(OnCalculatePathTimer));
        }
        public override void _Ready()
        {
            AddChild(_calculatePathTimer);
        }
        private void OnCalculatePathTimer()
        {
            _canCalucate = true;
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

            if (path.Length>1) // блинблять
            {
                ret = path[1];
            }

            return ret;
        }
    }
}
