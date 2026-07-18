using Godot;
using System.Collections.Generic;

public abstract partial class Actor : Node2D, IActor
{
    [Export] public float MoveSpeed = 4f;
    [Export] public Vector2I Cell { get; set; }

    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);


    // all actors need this -- vector2I value of zero is "wait"
    public abstract Vector2I DecideDirection(Grid grid);
    public abstract void TickTurn();
    public abstract void FaceDirection(Vector2I dir);

    public static readonly Vector2I[] _directions =
    {
        new Vector2I(0, 1), new Vector2I(1, 1), new Vector2I(0, -1),
        new Vector2I(-1, -1), new Vector2I(1, 0), new Vector2I(-1, 0),
        new Vector2I(-1, 1), new Vector2I(1, -1)
    };

    // attempting to write a pathfinding algo from scratch :P should output a directional vector that follows the path to the destination
    public Vector2I BreadthFirstSearch(Vector2I start, Vector2I destination, Grid grid, int maxIterations = 5000)
    {
        if (start == destination) { return Vector2I.Zero; }

        var _checkedCells = new Godot.Collections.Dictionary<string, int>();
        var _searchFrontier = new Queue<Vector2I>();
        _searchFrontier.Enqueue(start);
        var cameFrom = new System.Collections.Generic.Dictionary<Vector2I, Vector2I>();
        var visited = new HashSet<Vector2I>() { start };

        // using this to track valid queued neighbors
        bool found = false;
        int iterations = 0;

        while (_searchFrontier.Count > 0 && iterations++ < maxIterations)
        {
            // get current cell by dequeuing from frontier
            Vector2I current = _searchFrontier.Dequeue();

            for (int d = 0; d < _directions.Length; d++)
            {
                // populating neighbors
                Vector2I neighbor = current + _directions[d];

                // conditions to skip this neighbor
                if (visited.Contains(neighbor)) { continue; }
                if (!grid.IsFree(neighbor)) { continue; }

                // otherwise, add it and log where we're coming from
                visited.Add(neighbor);
                cameFrom[neighbor] = current;

                if (neighbor == destination)
                {
                    found = true;
                    break;
                }

                _searchFrontier.Enqueue(neighbor);
            }

            if (found) { break; }
        }

        if (!found)
        {
            GD.PrintErr("Pathfinding failed!");
            return Vector2I.Zero;
        }

        // walking backward until we find parent...
        Vector2I step = destination;
        while (cameFrom[step] != start) step = cameFrom[step];

        GD.Print($"Path to destination found, next step: {step}");
        return step - start;
    }
}
