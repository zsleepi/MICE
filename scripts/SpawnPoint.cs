using Godot;

[Tool]
[GlobalClass]
public partial class SpawnPoint : Marker2D
{
    [Export] public string Id;
    [Export] public TileMapLayer Terrain;

    public Vector2I GetCell()
    {
        if (Terrain == null) return Vector2I.Zero;

        Vector2 localPos = Terrain.ToLocal(GlobalPosition);
        return Terrain.LocalToMap(localPos);
    }
}