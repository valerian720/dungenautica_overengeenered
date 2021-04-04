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
            Vector2 ret = Vector2.Zero; // по умолчанию остается на месте
            // возвращает координаты ближайщей точки по направлению движения

            // TODO не вызывать кучу раз (на каждом кадре), а вызвать, затем пройти некоторый путь по path, затем пересчитать path
            // или мб добавить просчет пути по таймеру каждую секунду (?)
            try
            {
                path = nav2D.GetSimplePath(source, destiny);
                 /*
                GD.Print(path[0]);
                GD.Print(path.Length);

                GD.Print(source);
                GD.Print(destiny);
                GD.Print("========");
                 */
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }

            if (path.Length>1) // блинблять
            {
                ret = path[1];
            }

            return ret;
        }
    }
}
