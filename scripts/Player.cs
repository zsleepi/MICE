using Godot;
using System;

public partial class Player : Node2D, IActor
{
    [Export] public Sprite2D Sprite;
    [Export] public float MoveSpeed = 6.5f;

    public Vector2I Cell { get; set; }
    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);

    // Called when the node enters the scene tree for the first time.
    public Vector2I GetIntent()
    {
        if (Input.IsActionPressed("move_up")) return Vector2I.Up;
        if (Input.IsActionPressed("move_down")) return Vector2I.Down;
        if (Input.IsActionPressed("move_left")) return Vector2I.Left;
        if (Input.IsActionPressed("move_right")) return Vector2I.Right;

        return Vector2I.Zero; // wait case
    }
}
