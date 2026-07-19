using Godot;
using System;

public interface IActor
{
	Vector2I Cell { get; set; }
	Node2D Node { get; }
	float StepDuration { get; }
    abstract void HandleBump(bool didBump);
}
