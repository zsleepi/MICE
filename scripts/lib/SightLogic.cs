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
        public static HashSet<Vector2I> GetSensedTiles(Actor actor)
        {
            HashSet<Vector2I> _visibles = [];
            foreach (Vector2I hash in GetSightTiles(actor))
            {
                _visibles.Add(hash);
            }
            foreach (Vector2I hash in GetTouchTiles(actor))
            {
                _visibles.Add(hash);
            }
            return _visibles;
        }

        // try optimizing with polygon math? https://legends2k.github.io/2d-fov/design.html
        public static HashSet<Vector2I> GetSightTiles(Actor actor)
        {
            Vector2I _viewPos = actor.Cell;
            int _sightRange = (int)actor.SightRange.GetMod();
            int _darkSightRange = (int)actor.DarkSightRange.GetMod();

            HashSet<Vector2I> _visibles = [_viewPos];
            HashSet<Vector2I> _viableTiles = [_viewPos];

            for (int iteration = 1; iteration <= _sightRange; iteration++)
            {
                List<Vector2I> _squarePerimeter = GridUtils.GetSquarePerimeter(_viewPos, iteration);
                foreach (Vector2I cell in _squarePerimeter)
                {
                    if (!GridUtils.IsWithinDistance(_viewPos, cell, _sightRange)) { continue; }
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
                            if (GridUtils.IsWithinDistance(_viewPos, cell, _darkSightRange) || Grid.IsLit(cell)) { _visibles.Add(cell); }
                            break;
                        }
                    }
                }
            }
            return _visibles;
        }

        public static HashSet<Vector2I> GetTouchTiles(Actor actor)
        {
            Vector2I _pos = actor.Cell;

            HashSet<Vector2I> _sensedTiles = [_pos];
            foreach (var tile in GridUtils.Directions)
            {
                _sensedTiles.Add(_pos + tile);
            }
            return _sensedTiles;
        }
    }
}
