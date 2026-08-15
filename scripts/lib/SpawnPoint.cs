using Godot;

[Tool]
[GlobalClass]
public partial class SpawnPoint : Marker2D
{
    [Export] public string Id;
    [Export] public TileMapLayer Terrain;

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return;
        DrawCircle(Vector2.Zero, 6f, new Color(0.2f, 1f, 0.4f, 0.4f));
        DrawArc(Vector2.Zero, 6f, 0, Mathf.Tau, 24, Colors.Lime, 1.5f);
    }

    public Vector2I GetCell()
    {
        if (Terrain == null) return Vector2I.Zero;

        Vector2 localPos = Terrain.ToLocal(GlobalPosition);
        return Terrain.LocalToMap(localPos);
    }
}