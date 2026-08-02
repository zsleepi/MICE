using Godot;

public partial class CameraController : Camera2D
{
    private Node2D _target;
    private Rect2 _bounds;
    private bool _useBounds;

    public void Follow(Node2D target) => _target = target;

    public void SetBounds(Rect2 bounds)
    {
        _bounds = bounds;
        _useBounds = true;
    }

    public void ClearBounds() => _useBounds = false;

    public override void _Process(double delta)
    {
        // holy annoying

        if (_target == null) return;

        Vector2 desired = _target.GlobalPosition;

        if (_useBounds)
        {
            Vector2 viewport = GetViewportRect().Size;
            Vector2 half = viewport / 2f;

            // x axis
            float minX = _bounds.Position.X + half.X;
            float maxX = _bounds.End.X - half.X;

            if (maxX > minX)
                desired.X = Mathf.Clamp(desired.X, minX, maxX);
            else
                desired.X = _bounds.GetCenter().X; // room too small case, so just center

            // y axis
            float minY = _bounds.Position.Y + half.Y;
            float maxY = _bounds.End.Y - half.Y;

            if (maxY > minY)
                desired.Y = Mathf.Clamp(desired.Y, minY, maxY);
            else
                desired.Y = _bounds.GetCenter().Y;
        }

        // fancy looking lerping stuff
        GlobalPosition = GlobalPosition.Lerp(desired, 0.15f);
    }

    public void TeleportToTarget()
    {
        if (_target != null)
        {
            GlobalPosition = _target.GlobalPosition;
        }
    }
}