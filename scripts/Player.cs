using Godot;
using System;

public partial class Player : Node2D, IActor
{
    [Export] public Sprite2D Sprite;
    [Export] public float MoveSpeed = 6.5f;

    public Vector2I Cell { get; set; }
    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);

    public bool LastMovementBumped { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Vector2I GetIntent()
    {
        var dir = Vector2I.Zero; // wait case
        if (Input.IsActionPressed("move_up"))    dir += Vector2I.Up;
        if (Input.IsActionPressed("move_down"))  dir += Vector2I.Down;
        if (Input.IsActionPressed("move_left"))  dir += Vector2I.Left;
        if (Input.IsActionPressed("move_right")) dir += Vector2I.Right;

        return dir;
    }

    public void FaceDirection(Vector2I dir)
    {
        if (dir.X != 0) Sprite.FlipH = dir.X > 0;
    }

    public void HandleBump(bool didBump) { }
}
