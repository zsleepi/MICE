using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public static class SightLogic
    {
        // try optimizing with polygon math? https://legends2k.github.io/2d-fov/design.html
        public static List<Vector2I> GetVisibleTiles(Vector2I viewPos, int sightRange, Grid grid)
        {
            List<Vector2I> visibles = [];
            Rect2I area = new Rect2I(viewPos.X - sightRange, viewPos.Y - sightRange, sightRange*2, sightRange * 2);

            for (int x = area.Position.X; x < area.Position.X + area.Size.X; x++)
            {
                for (int y = area.Position.Y; y < area.Position.Y + area.Size.Y; y++)
                {
                    Vector2I targetPos = new Vector2I(x, y);

                    if (IsWithinDistance(viewPos, targetPos, sightRange))
                    {
                        Vector2I difference = new Vector2I(x - viewPos.X, y - viewPos.Y);
                        int tileSteps = Math.Max(Math.Abs(difference.X), Math.Abs(difference.Y));
                        for (int i = 0; i < tileSteps; i++)
                        {
                            Vector2I tileAlongLine = new Vector2I(viewPos.X + difference.X * (i / tileSteps), viewPos.Y + difference.Y*(i/tileSteps));
                            visibles.Add(tileAlongLine);
                            if (grid.IsViewObstruction(tileAlongLine)) { break; }
                        }
                    }
                }
            }
            return visibles;
        }

        public static bool IsWithinDistance(Vector2I origin, Vector2I target, int range)
        {
            if (GetDistance(origin, target) <= range) { return true; }
            else return false;
        }

        public static double GetDistance(Vector2I point1, Vector2I point2)
        {
            double distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            return distance;
        }
    }
}
