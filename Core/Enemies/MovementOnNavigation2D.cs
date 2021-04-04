using Godot;
using System;

namespace SibGameJam2021.Core.Enemies
{
    class MovementOnNavigation2D
    {
        private Navigation2D nav2D = null;
        private Vector2[] path = new Vector2[0];
        public MovementOnNavigation2D(Navigation2D _nav2D)
        {
            nav2D = _nav2D;
        }
        public Vector2 GetPointTowardsDestiny(Vector2 source, Vector2 destiny)
        {
            Vector2 ret = source; // по умолчанию остается на месте
            // возвращает координаты ближайщей точки по направлению движения

            // TODO не вызывать кучу раз (на каждом кадре), а вызвать, затем пройти некоторый путь по path, затем пересчитать path
            try
            {
                path = nav2D.GetSimplePath(source, destiny);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }

            if (path.Length>0) // блинблять
            {
                ret = path[0];
            }

            return ret;
        }
    }
}
