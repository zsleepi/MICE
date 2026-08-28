using Godot;
using System;

public partial class MaskTiles : TileMapLayer
{

    public void DoSight(Godot.Collections.Array<Vector2I> cells)
    {
        this.Clear();
        this.SetCellsTerrainConnect(cells, 0, 0, true);
    }
}
