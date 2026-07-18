using Godot;
using System;

public partial class Fox : Actor
{
    [Export] public Sprite2D Sprite;

    private Vector2I _currentDirection = Vector2I.Zero;
    private float _moveTimer;
    private int _turnsUntilChange = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _moveTimer = 0f;
	}

    public override void TickTurn()
    {
        _currentDirection = Vector2I.Zero;
        _turnsUntilChange--;

        if (_turnsUntilChange <= 0)
        {
            _currentDirection = GetRandomDirection();
            _turnsUntilChange = (int)GD.RandRange(2, 6); // random number of turns in this range
        }
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
