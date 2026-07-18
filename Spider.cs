using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spider : Actor
{
    [Export] public Sprite2D Sprite;

    private Vector2I _currentDirection = Vector2I.Zero;
    private int _turnsUntilChange = 0;
    [Export] public Vector2I targetCell { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

    public override void TickTurn()
    {
        _currentDirection = Vector2I.Zero;
        _turnsUntilChange--;

        if (_turnsUntilChange <= 0)
        {
            _currentDirection = BreadthFirstSearch(this.Cell, targetCell);
            _turnsUntilChange = 1;
        }
    }

    // attempting to write a pathfinding algo from scratch :P should output a directional vector that follows the path to the destination
    public Vector2I BreadthFirstSearch(Vector2I start, Vector2I destination)
    {
        if (start == destination) { return Vector2I.Zero; }
        var _checkedCells = new Godot.Collections.Dictionary<string, int>();
        List<Vector2I> _searchFrontier = new List<Vector2I> { start };
        Vector2I[] _directions;
        _directions = [new Vector2I(0, 1), new Vector2I(1, 1), new Vector2I(0, -1), new Vector2I(-1, -1), new Vector2I(1, 0), new Vector2I(-1, 0), new Vector2I(-1, 1), new Vector2I(1, -1),];
        Vector2I _currentCell = start;

        int _stepsFromStart = 0;
        _checkedCells.Add(start.GetHashCode().ToString(), _stepsFromStart);

        int _searchDepth = 0; // for debugging purposes

        for (int i = 0; i < 10000; i++)
        {
            _stepsFromStart = _checkedCells[_searchFrontier[i].GetHashCode().ToString()];
            for (int directionIndex = 0; directionIndex < _directions.Length; directionIndex++)
            {
                Vector2I cell = _searchFrontier[i] + _directions[directionIndex];
                if (!_checkedCells.ContainsKey(cell.GetHashCode().ToString()))
                {
                    // add new eligible cell to frontier for future search
                    _searchFrontier.Add(cell);
                    _checkedCells.Add(cell.GetHashCode().ToString(), _stepsFromStart + 1);
                    if (_searchDepth < _stepsFromStart+1)
                    {
                        _searchDepth = _stepsFromStart + 1;
                        GD.Print("search depth at " + _searchDepth + " from start");
                    }
                }
                if (cell == destination)
                {
                    // this means a path to the destination exists in the discovered graph. the code should start tracing it back from here.
                    GD.Print("Path to destination found! destination coords: " + cell);
                    List<Vector2I> _path = new List<Vector2I> { cell };
                    for (int j = 0; j < _searchDepth; j++)
                    {
                        for (int k = 0; k < _directions.Length; k++)
                        {
                            Vector2I _adjacentCell = _path[j] + _directions[k];
                            if (_adjacentCell == start)
                            {
                                GD.Print("Returning Vector for pathfinding!");
                                return _path[j] - start;
                            }
                            GD.Print("checking if key "+ _adjacentCell + " in dictionary. " + _checkedCells.ContainsKey(_adjacentCell.GetHashCode().ToString()));
                            if (_checkedCells.ContainsKey(_adjacentCell.GetHashCode().ToString()))
                            {
                                GD.Print("checking if cell closer to start. comparing " + _checkedCells[_adjacentCell.GetHashCode().ToString()] + " and " + _checkedCells[_path[j].GetHashCode().ToString()]);
                                if (_checkedCells[_adjacentCell.GetHashCode().ToString()] < _checkedCells[_path[j].GetHashCode().ToString()])
                                {
                                    GD.Print("Adding new cell to final search!");
                                    _path.Add(_adjacentCell);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        // catch
        GD.PrintErr("Pathfinding failed!");
        return Vector2I.Zero;
    }

    public override Vector2I DecideDirection(Grid grid) => _currentDirection;
    public override void FaceDirection(Vector2I dir)
    {        
        if (dir.X != 0) Sprite.FlipH = dir.X > 0;
    }

    private Vector2I GetRandomDirection()
    {
        uint x, y;

        do
        {
            x = GD.Randi() % 3 - 1;
            y = GD.Randi() % 3 - 1;
        } while (x == 0 && y == 0);

        return new Vector2I((int)x, (int)y);
    }
}
