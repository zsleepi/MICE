using Godot;
using System;

/// <summary>
/// handles player movement. movement is grid-based and this controller currently enforces
/// discrete steps along the grid. we use smoothing for movement rather than instantaneous
/// jumps.
/// TODO: implement horizontal flipping upon last left/right movement, continue to adjust
/// feeling of movement
/// </summary>
public partial class PlayerController : CharacterBody2D
{
    // we're using 18x18 tiles for the grid generally speaking
    private const int GridSize = 18;

    [ExportGroup("Movement Settings")]
    [Export] private float _moveSpeed = 200f;     // pixels per second
    [Export] private float _moveCooldown = 0.11f; // seconds between grid steps

    private float _timeSinceLastMove = 0f;
    private Vector2I _gridPos = Vector2I.Zero;

    // for smooth sliding (if we want that)
    private Vector2 _targetPosition;

    public override void _Ready()
    {
        _targetPosition = GetCellCenter(_gridPos);
        Position = _targetPosition;
    }

    public override void _Process(double delta)
    {
        float deltaF = (float)delta;
        _timeSinceLastMove += deltaF;

        if (_timeSinceLastMove < _moveCooldown)
        {
            return; // do nothing if cooldown not reached to prevent movement spam
        }

        Vector2I direction = Vector2I.Zero;

        // accumulating inputs here; subscribing using += rather than directly =
        if (Input.IsActionPressed("move_up"))    direction += Vector2I.Up;
        if (Input.IsActionPressed("move_down"))  direction += Vector2I.Down;
        if (Input.IsActionPressed("move_left"))  direction += Vector2I.Left;
        if (Input.IsActionPressed("move_right")) direction += Vector2I.Right;

        // clamping so opposing keys cancel
        direction = new Vector2I(
            Math.Clamp(direction.X, -1, 1),
            Math.Clamp(direction.Y, -1, 1)
        );

        if (direction != Vector2I.Zero)
        {
            _gridPos += direction;
            _targetPosition = GetCellCenter(_gridPos);
            _timeSinceLastMove = 0f;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaF = (float)delta;

        // moving toward target by at most _moveSpeed * deltaF pixels
        Position = Position.MoveToward(_targetPosition, _moveSpeed * deltaF);
    }

    /// <summary>
    /// helper method for fetching center of player
    /// </summary>
    /// <param name="gridCell"></param>
    /// <returns></returns>
    private Vector2 GetCellCenter(Vector2I gridCell)
    {
        return new Vector2(
            gridCell.X * GridSize + GridSize / 2f,
            gridCell.Y * GridSize + GridSize / 2f
        );
    }
}