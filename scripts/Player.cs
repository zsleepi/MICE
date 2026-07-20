using Godot;
using System;

public partial class Player :  Actor
{
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
}
