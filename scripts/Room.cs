using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D
{
	[Export] public TileMapLayer Terrain;
	[Export] public Node2D Actors;
	[Export] public Vector2I PlayerSpawnCell;

	public IEnumerable<NPC> GetActors() =>
		Actors.GetChildren().OfType<NPC>();
}
