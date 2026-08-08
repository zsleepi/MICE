using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MICE.scripts.lib
{
    internal static class GridUtils
    {
        public static readonly Vector2I[] _directions =
        {
            new Vector2I(0, 1), new Vector2I(1, 1), new Vector2I(0, -1),
            new Vector2I(-1, -1), new Vector2I(1, 0), new Vector2I(-1, 0),
            new Vector2I(-1, 1), new Vector2I(1, -1)
        };

        // attempting to write a pathfinding algo from scratch :P should output a directional vector that follows the path to the destination
        public static Queue<Vector2I> BreadthFirstSearch(Vector2I start, Vector2I destination, Grid grid, int maxIterations = 4000)
        {
            // randomize order that directions are checked in
            var _unorderedDirections = _directions.OrderBy(x => Guid.NewGuid()).ToArray();

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
                    if (!grid.IsFree(neighbor)) { continue; }

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
    }
}
