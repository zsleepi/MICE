using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MICE.scripts.lib
{
    internal static class GridUtils
    {
        public static readonly Vector2I[] Directions =
        {
            new Vector2I(0, 1), new Vector2I(1, 1), new Vector2I(0, -1),
            new Vector2I(-1, -1), new Vector2I(1, 0), new Vector2I(-1, 0),
            new Vector2I(-1, 1), new Vector2I(1, -1)
        };

        // attempting to write a pathfinding algo from scratch :P should output a directional vector that follows the path to the destination
        public static Queue<Vector2I> BreadthFirstSearch(Vector2I start, Vector2I destination, int maxIterations = 4000)
        {
            // randomize order that directions are checked in
            var _unorderedDirections = Directions.OrderBy(x => Guid.NewGuid()).ToArray();

            var _finalPath = new Queue<Vector2I>();
            if (start == destination)
            {
                _finalPath.Enqueue(Vector2I.Zero);
                return _finalPath;
            }

            var _searchFrontier = new Queue<Vector2I>();
            _searchFrontier.Enqueue(start);
            var cameFrom = new System.Collections.Generic.Dictionary<Vector2I, Vector2I>();
            var visited = new HashSet<Vector2I>() { start };

            // has destination been found in pathfinding search
            bool found = false;
            // still using maxIterations so the actor doesn't consider rly distant cells for pathfinding
            int iterations = 0;

            while (_searchFrontier.Count > 0 && iterations++ < maxIterations)
            {
                // get current cell by dequeuing from frontier
                Vector2I current = _searchFrontier.Dequeue();

                for (int d = 0; d < _unorderedDirections.Length; d++)
                {
                    // populating neighbors
                    Vector2I neighbor = current + _unorderedDirections[d];

                    if (neighbor == destination)
                    {
                        visited.Add(neighbor);
                        cameFrom[neighbor] = current;
                        found = true;
                        break;
                    }

                    // conditions to skip this neighbor
                    if (visited.Contains(neighbor)) { continue; }
                    if (!Grid.IsFree(neighbor)) { continue; }

                    // otherwise, add it and log where we're coming from
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;

                    _searchFrontier.Enqueue(neighbor);
                }

                if (found) { break; }
            }

            if (!found)
            {
                GD.PrintErr($"Pathfinding failed after {iterations} iterations...");
                _finalPath.Enqueue(Vector2I.Zero);
                return _finalPath;
            }

            // walking backward until we find parent...
            Vector2I step = destination;
            _finalPath.Enqueue(step - cameFrom[step]);
            while (cameFrom[step] != start)
            {
                step = cameFrom[step];
                _finalPath.Enqueue(step - cameFrom[step]);
            }
            var _pathBack = new Queue<Vector2I>(_finalPath.Reverse());
            return _pathBack;
        }

        public static List<Vector2I> GetSquarePerimeter(Vector2I center, int radius)
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

        // this is outdated.- implement error correction here too
        public static List<Vector2I> GetVisibleArea(Vector2I origin, int range)
        {
            Vector2I _viewPos = origin;
            int _sightRange = range;

            List<Vector2I> _visibles = [_viewPos];
            HashSet<Vector2I> _viableTiles = [_viewPos];

            for (int iteration = 1; iteration <= _sightRange; iteration++)
            {
                List<Vector2I> _squarePerimeter = GridUtils.GetSquarePerimeter(_viewPos, iteration);
                foreach (Vector2I cell in _squarePerimeter)
                {
                    if (!GridUtils.IsWithinDistance(_viewPos, cell, _sightRange)) { continue; }
                    if (Grid.IsViewObstruction(cell)) continue;
                    Vector2I _dif = _viewPos - cell;
                    Vector2 _stepIncrement = new Vector2((float)_dif.X / iteration, (float)_dif.Y / iteration);
                    Vector2 _rayVector = new Vector2(cell.X, cell.Y);
                    for (int i = 0; i <= iteration; i++)
                    {
                        _rayVector += _stepIncrement;
                        Vector2I _intersectedTile = new Vector2I((int)Math.Round(_rayVector.X), (int)Math.Round(_rayVector.Y));
                        if (Grid.IsViewObstruction(_intersectedTile) && _intersectedTile != _viewPos) { break; }
                        if (_viableTiles.Contains(_intersectedTile))
                        {
                            _viableTiles.Add(cell);
                            _visibles.Add(cell);
                            break;
                        }
                    }
                }
            }
            return _visibles;
        }

        public static List<Rect2I> SliceRectHorizontal(Rect2I input, int sliceAt)
        {
            List<Rect2I> slicedRects = [];
            sliceAt = Math.Clamp(sliceAt, 1, input.Size.Y - 1);
            Rect2I slicedRect = new Rect2I(input.Position, new Vector2I(input.Size.X, sliceAt));
            Rect2I remainder = new Rect2I(new Vector2I(input.Position.X, input.Position.Y + sliceAt), new Vector2I(input.Size.X, input.Size.Y - sliceAt));
            slicedRects.Add(slicedRect);
            slicedRects.Add(remainder);
            return slicedRects;
        }

        public static List<Rect2I> SliceRectVertical(Rect2I input, int sliceAt)
        {
            List<Rect2I> slicedRects = [];
            sliceAt = Math.Clamp(sliceAt, 1, input.Size.X - 1);
            Rect2I slicedRect = new Rect2I(input.Position, new Vector2I(sliceAt, input.Size.Y));
            Rect2I remainder = new Rect2I(new Vector2I(input.Position.X + sliceAt, input.Position.Y), new Vector2I(input.Size.X - sliceAt, input.Size.Y));
            slicedRects.Add(slicedRect);
            slicedRects.Add(remainder);
            return slicedRects;
        }

        public static List<Rect2I> SliceRectangle(Rect2I input, int minSize, float maxLengthRatio)
        {
            List<Rect2I> subdividedRects = [];
            maxLengthRatio = Math.Max(maxLengthRatio, 2);
            minSize = Math.Min(minSize, (int)Math.Floor((decimal)(input.Size.X * input.Size.Y) / 2));
            Random r = new Random();
            bool _sliceHorizontal;
            if (input.Size.Y == input.Size.X)
            {
                if (r.NextDouble() > 0.5) { _sliceHorizontal = true; }
                else { _sliceHorizontal = false; }
            }
            else if (input.Size.Y > input.Size.X)
            {
                _sliceHorizontal = true;
            } else
            {
                _sliceHorizontal = false;
            }

            if (_sliceHorizontal)
            {
                int minSlice = Math.Max((int)Math.Ceiling((decimal)(minSize / input.Size.X)), (int)Math.Ceiling(input.Size.X / maxLengthRatio));
                minSlice = Math.Min(minSlice, (int)Math.Floor((decimal)input.Size.Y / 2));
                int randomSlice = r.Next(minSlice, input.Size.Y - minSlice);
                subdividedRects = SliceRectHorizontal(input, randomSlice);
            } else
            {
                int minSlice = Math.Max((int)Math.Ceiling((decimal)(minSize / input.Size.Y)), (int)Math.Ceiling(input.Size.Y / maxLengthRatio));
                minSlice = Math.Min(minSlice, (int)Math.Floor((decimal)input.Size.X / 2));
                int randomSlice = r.Next(minSlice, input.Size.X - minSlice);
                subdividedRects = SliceRectVertical(input, randomSlice);
            }
            return subdividedRects;
        }

        public static Rect2I ShrinkRectEven(Rect2I input, int shrinkBy)
        {
            Vector2I _reductionVector = new Vector2I(shrinkBy, shrinkBy);
            Rect2I _shrunkRect = new Rect2I(input.Position + _reductionVector, input.Size - _reductionVector * 2);
            return _shrunkRect;
        }
        public static Rect2I ShrinkRectUneven(Rect2I input, int shrinkBy)
        {
            Vector2I _reductionVector = new Vector2I(shrinkBy, shrinkBy);
            Rect2I _shrunkRect = new Rect2I(input.Position + _reductionVector, input.Size - _reductionVector);
            return _shrunkRect;
        }

        public static Rect2I ShrinkRectRandom(Rect2I input, int shrinkBy)
        {
            Vector2I _reductionX = new Vector2I(shrinkBy, 0);
            Vector2I _reductionY = new Vector2I(0, shrinkBy);
            Random r = new Random();
            Rect2I _shrunkRect = input;
            if (r.NextDouble() > 0.5) { _shrunkRect.Position += _reductionX; } else
            {
                _shrunkRect.Size -= _reductionX;
            }
            if (r.NextDouble() > 0.5) { _shrunkRect.Position += _reductionY; }
            else
            {
                _shrunkRect.Size -= _reductionY;
            }
            return _shrunkRect;
        }
    }
}
