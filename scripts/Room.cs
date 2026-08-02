using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D
{
	[Export] public TileMapLayer Terrain;
	[Export] public Node2D RoomTransitions;
	[Export] public Node2D Actors;
    [Export] public Node2D SpawnPoints;
	[Export] public Vector2I PlayerSpawnCell;

	public IEnumerable<Actor> GetActors() =>
		Actors.GetChildren().OfType<Actor>();

    public IEnumerable<RoomTransitionCell> GetRoomTransitions() =>
        RoomTransitions.GetChildren().OfType<RoomTransitionCell>();

    // uses terrain tilemaplayer to auto-fetch the needed size.....thats rly cool
    public Rect2 GetPlayArea()
    {
        Rect2I cellRect = Terrain.GetUsedRect();

        // GetUsedRect returns position = top-left cell, size = how many cells
        // so the "end" cell is position + size
        Vector2I topLeftCell = cellRect.Position;
        Vector2I bottomRightCell = cellRect.Position + cellRect.Size;

        // convert to world coords
        Vector2 topLeftWorld = Terrain.ToGlobal(Terrain.MapToLocal(topLeftCell));
        Vector2 bottomRightWorld = Terrain.ToGlobal(Terrain.MapToLocal(bottomRightCell));

        return new Rect2(topLeftWorld, bottomRightWorld - topLeftWorld);
    }

    public Vector2I FindSpawnCell(string id)
    {
        foreach (Node child in SpawnPoints.GetChildren())
        {
            if (child is SpawnPoint sp && sp.Id == id)
            {
                return sp.GetCell();
            }
        }

        // fallback to player spawn cell
        return PlayerSpawnCell;
    }
}
