using Godot;
using System;

public partial class Fox : NPC
{
    private Vector2I _currentDirection = Vector2I.Zero;
    private int _turnsUntilChange = 0;

    public override void TickTurn()
    {
        _currentDirection = Vector2I.Zero;
        _turnsUntilChange--;
    }

    public override Vector2I DecideDirection(Grid grid)
    {
        if (_turnsUntilChange > 0)
        {
            return _currentDirection;
        }

        _currentDirection = GetRandomDirection();
        _turnsUntilChange = (int)GD.RandRange(2, 6);
        return _currentDirection;
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

    // i know exactly what i'm doing
}
