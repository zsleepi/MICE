using Godot;

[Tool]
[GlobalClass]
public partial class RoomTransitionCell : Node2D
{
    // there are different ways to do this... like just detecting the edge of the camera
    // but i like the flexibility of defining a specific area where we allow a room
    // transition

    [Export] public TileMapLayer DebugTileMap;

    // defining the area of the room transition tiles:
    [Export] public Vector2I TopLeftCell;     // top-left corner
    [Export] public Vector2I BottomRightCell; // bottom‑right corner

    [Export] public string TargetRoomPath; // not using packedscene... causes circular reference issues
    [Export] public string TargetSpawnId;

    // this helps us see the area in the godot editor!
    public override void _Draw()
    {
        // hide all the visual drawing stuff while actually running the game
        if (!Engine.IsEditorHint())
            return;

        if (DebugTileMap == null || DebugTileMap.TileSet == null)
            return;

        // gets the right tile size
        Vector2I tileSize = DebugTileMap.TileSet.TileSize;

        int minX = Mathf.Min(TopLeftCell.X, BottomRightCell.X);
        int maxX = Mathf.Max(TopLeftCell.X, BottomRightCell.X);
        int minY = Mathf.Min(TopLeftCell.Y, BottomRightCell.Y);
        int maxY = Mathf.Max(TopLeftCell.Y, BottomRightCell.Y);

        Vector2 topLeftWorld = DebugTileMap.ToGlobal(
            DebugTileMap.MapToLocal(new Vector2I(minX, minY))
        );

        Vector2 bottomRightWorld = DebugTileMap.ToGlobal(
            DebugTileMap.MapToLocal(new Vector2I(maxX + 1, maxY + 1))
        );

        Vector2 topLeftLocal = ToLocal(topLeftWorld);
        Vector2 bottomRightLocal = ToLocal(bottomRightWorld);

        Rect2 area = new Rect2(topLeftLocal, bottomRightLocal - topLeftLocal);

        // finally drawin!!
        DrawRect(area, new Color(0.2f, 0.5f, 1f, 0.25f), true); // fill
        DrawRect(area, Colors.Aqua, false);                     // outline
    }

    // checks if a cell is within this area
    public bool Contains(Vector2I cell)
    {
        return cell.X >= TopLeftCell.X && cell.X <= BottomRightCell.X &&
               cell.Y >= TopLeftCell.Y && cell.Y <= BottomRightCell.Y;
    }

    // this is how we load the scenes now rather than packedroom at runtime
    public PackedScene LoadTargetRoom() =>
        ResourceLoader.Load<PackedScene>("res://scenes/" + TargetRoomPath + ".tscn");
}