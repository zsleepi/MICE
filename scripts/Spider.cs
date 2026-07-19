using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class Spider : Actor
{
    [Export] public Vector2I TargetCell;
    public IActor Target;

    private Vector2I _currentDirection = Vector2I.Zero;
    private int _moveCooldown = 0;
    private int _pathfindCooldown = 0;
    private Queue<Vector2I> _queuedMoves = new Queue<Vector2I>();
    private bool LastMovementBumped = false;
    public override void TickTurn()
    {
        _pathfindCooldown--;
        _moveCooldown--;
    }

    public override void HandleBump(bool didBump)
    {
        if (didBump) { LastMovementBumped = true; } else { LastMovementBumped = false; }
    }

    public override Vector2I DecideDirection(Grid grid)
    {
        if (_moveCooldown > 0)
        {
            return Vector2I.Zero;
        }
        if (_queuedMoves.Count == 0 || LastMovementBumped || _pathfindCooldown <= 0)
        {
            if (Target != null) { TargetCell = Target.Cell; }
            _queuedMoves = BreadthFirstSearch(this.Cell, TargetCell, grid, 30000);
            _pathfindCooldown = (int)GD.RandRange(10, 15);
        }
        _currentDirection = _queuedMoves.Dequeue();
        _moveCooldown = 2;
        return _currentDirection;
    }
}
