using Godot;
using MICE.scripts.lib;
using System;
using System.Collections.Generic;

public partial class MaskTiles : TileMapLayer
{
    public void SetMaskedCells(List<Vector2I> cells)
    {
        this.Clear();
        // recast
        Godot.Collections.Array<Vector2I> _array = [];
        foreach (Vector2I cell in cells)
        {
            _array.Add(cell);
        }
        this.SetCellsTerrainConnect(_array, 0, 0, true);
    }
}
