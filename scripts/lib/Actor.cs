using Godot;

public abstract partial class Actor : Node2D, IActor
{
    [Export] public float MoveSpeed = 4f;

    public Vector2I Cell { get; set; }
    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);


    // all actors need this -- vector2I value of zero is "wait"
    public abstract Vector2I DecideDirection(Grid grid);
}
