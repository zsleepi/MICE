using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spider : Actor
{
    [Export] public Sprite2D Sprite;
    [Export] public Vector2I TargetCell;
    public IActor Target;

    private Vector2I _currentDirection = Vector2I.Zero;
    private int _turnsUntilRepath = 0;
    private int _turnsUntilRetarget = 0;

    public override void TickTurn()
    {
        _turnsUntilRepath--;
        _turnsUntilRetarget--;
    }

    public override Vector2I DecideDirection(Grid grid)
    {
        if (_turnsUntilRetarget <= 0 && Target != null)
        {
            TargetCell = Target.Cell;
            _turnsUntilRetarget = 5;
        }

        if (_turnsUntilRepath > 0)
        {
            return _currentDirection;
        }

        _currentDirection = BreadthFirstSearch(this.Cell, TargetCell, grid);
        _turnsUntilRepath = 1;
        return _currentDirection;
    }

    public override void FaceDirection(Vector2I dir)
    {        
        if (dir.X != 0) Sprite.FlipH = dir.X > 0;
    }
}
