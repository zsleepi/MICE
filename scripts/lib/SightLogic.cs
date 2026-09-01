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
        public static List<Vector2I> GetVisibleTiles(Vector2I viewPos, int sightRange, Grid grid, int darkSight)
        {
            List<Vector2I> visibles = [viewPos];
            Rect2I area = new Rect2I(viewPos.X - sightRange, viewPos.Y - sightRange, sightRange * 2, sightRange * 2);

            for (int x = area.Position.X; x < area.Position.X + area.Size.X; x++)
            {
                for (int y = area.Position.Y; y < area.Position.Y + area.Size.Y; y++)
                {
                    Vector2I targetPos = new Vector2I(x, y);
                    if (IsWithinDistance(viewPos, targetPos, sightRange))
                    {
                        Vector2I difference = new Vector2I(targetPos.X-viewPos.X, targetPos.Y-viewPos.Y);
                        int tileSteps = Math.Max(Math.Abs(difference.X), Math.Abs(difference.Y));
                        for (int i = 1; i <= tileSteps; i++)
                        {
                            Vector2I tileAlongLine = new Vector2I(viewPos.X + (int)Math.Round((difference.X) * ((decimal)(i) / tileSteps)), viewPos.Y + (int)Math.Round((difference.Y)*((decimal)i /tileSteps)));
                            if (!visibles.Contains(tileAlongLine) && grid.IsLit(tileAlongLine) || !visibles.Contains(tileAlongLine) && IsWithinDistance(viewPos, tileAlongLine, darkSight)) { visibles.Add(tileAlongLine); }
                            if (grid.IsViewObstruction(tileAlongLine)) { break; }
                        }
                    }
                }
            }
            return visibles;
        }

        // yeesh this one might be a challenge
        public static List<Vector2I> GetVisibleTilesPolygons()
        {

            return null;
        }

        public static List<Vector2I> GetVisibleTilesRaycast(Vector2I _viewPos, int _sightRange, Grid _grid, int _darkSightRange)
        {
            List<Vector2I> _visibles = new List<Vector2I>();
            int raycasts = 8 + (int)Math.Ceiling(_sightRange * 2 * Math.PI);
            float _degreeIncrement = 360 / raycasts;
            for (float degree = 0; degree < 360; degree += _degreeIncrement)
            {
                Vector2 _stepIncrement = new Vector2((float)Math.Sin(degree* 0.01745329), (float)Math.Cos(degree* 0.01745329));
                float _normalize = Math.Max(Math.Abs(_stepIncrement.X), Math.Abs(_stepIncrement.Y));
                _stepIncrement = new Vector2(_stepIncrement.X / _normalize, _stepIncrement.Y / _normalize);
                Vector2 _rayVector = _viewPos;
                while (IsWithinDistance(_viewPos, new Vector2I((int)Math.Round(_rayVector.X), (int)Math.Round(_rayVector.Y)), _sightRange))
                {
                    Vector2I _rayTile = new Vector2I((int)Math.Round(_rayVector.X), (int)Math.Round(_rayVector.Y));
                    if (_grid.IsLit(_rayTile) || IsWithinDistance(_viewPos, _rayTile, _darkSightRange)) { _visibles.Add(_rayTile); }
                    if (_grid.IsViewObstruction(_rayTile)) { break; }
                    _rayVector += _stepIncrement;
                }
            }
            return _visibles;
        }

        public static List<Vector2I> GetVisibleTilesOptimized(Vector2I _viewPos, int _sightRange, Grid _grid, int _darkSightRange)
        {
            List<Vector2I> _visibles = [_viewPos];
            HashSet<Vector2I> _viableTiles = [_viewPos];

            for (int iteration = 1; iteration <= _sightRange; iteration++)
            {
                List<Vector2I> _squarePerimeter = GetSquarePerimeterCha(_viewPos, iteration);
                foreach (Vector2I cell in _squarePerimeter)
                {
                    if (!IsWithinDistance(_viewPos, cell, _sightRange)) { continue; }
                    Vector2I _dif = _viewPos - cell;
                    Vector2 _stepIncrement = new Vector2((float)_dif.X / iteration, (float)_dif.Y / iteration);
                    Vector2 _rayVector = new Vector2(cell.X, cell.Y);
                    for (int i = 0; i <= iteration; i++)
                    {
                        _rayVector += _stepIncrement;
                        Vector2I _intersectedTile = new Vector2I((int)Math.Round(_rayVector.X), (int)Math.Round(_rayVector.Y));
                        if (_grid.IsViewObstruction(_intersectedTile)) { break; }
                        if (_viableTiles.Contains(_intersectedTile))
                        {
                            _viableTiles.Add(cell);
                            if (IsWithinDistance(_viewPos, cell, _darkSightRange) || _grid.IsLit(cell)) { _visibles.Add(cell); }
                            break;
                        }
                    }
                }
            }
            return _visibles;
        }

        public static List<Vector2I> GetSquarePerimeterCha(Vector2I center, int radius)
        {
            if (radius == 0) return new List<Vector2I> { center };

            var perimeter = new List<Vector2I>(radius * 8);

            int minX = center.X - radius;
            int maxX = center.X + radius;
            int minY = center.Y - radius;
            int maxY = center.Y + radius;

            // top edge (with corners)
            for (int x = minX; x <= maxX; x++)
                perimeter.Add(new Vector2I(x, minY));

            // bottom edge (with corners)
            for (int x = minX; x <= maxX; x++)
                perimeter.Add(new Vector2I(x, maxY));

            // left edge (no corners)
            for (int y = minY + 1; y <= maxY - 1; y++)
                perimeter.Add(new Vector2I(minX, y));

            // right edge (no corners)
            for (int y = minY + 1; y <= maxY - 1; y++)
                perimeter.Add(new Vector2I(maxX, y));

            return perimeter;
        }

        public static List<Vector2I> GetSquarePerimeterBilly(int _radius, Vector2I _origin)
        {
            if (_radius < 1) { return new List<Vector2I>([_origin]); }
            Vector2I _pos = new Vector2I(_origin.X - _radius, _origin.Y - _radius);
            Vector2I _size = new Vector2I(1 + _radius * 2, 1 + _radius * 2);
            Rect2I _square = new Rect2I( _pos, _size);
            List<Vector2I> _squareEdges = [_square.Position, new Vector2I(_square.Position.X, _square.End.Y), _square.End, new Vector2I(_square.End.X, _square.Position.Y)];
            List<Vector2I> _perimeterList = [.._squareEdges];
            for (int side = 0; side < 4; side++)
            {
                
            }

            return _perimeterList;
        }

        public static bool IsWithinDistance(Vector2I origin, Vector2I target, int range)
        {
            if (GetDistance(origin, target) < range) { return true; }
            else return false;
        }

        public static double GetDistance(Vector2I point1, Vector2I point2)
        {
            double distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            return distance;
        }
    }
}
