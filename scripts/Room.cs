using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D
{
    [Export] public TileMapLayer Terrain;
    [Export] public Node2D Actors;
    [Export] public Vector2I PlayerSpawnCell;

    private SpawnPoint[] _spawnPoints;
    private RoomTransitionCell[] _transitionCells;

    public IEnumerable<NPC> GetActors() =>
        Actors.GetChildren().OfType<NPC>();

    public override void _Ready()
    {
        // cache spawn points and transitions for fast lookup!!!
        var spawnNode = GetNodeOrNull("SpawnPoints");
        _spawnPoints = spawnNode?.GetChildren().OfType<SpawnPoint>().ToArray()
                       ?? Array.Empty<SpawnPoint>();

        var transNode = GetNodeOrNull("RoomTransitions");
        _transitionCells = transNode?.GetChildren().OfType<RoomTransitionCell>().ToArray()
                           ?? Array.Empty<RoomTransitionCell>();
    }

    public SpawnPoint GetSpawnPoint(string id)
    {
        foreach (var sp in _spawnPoints)
            if (sp.Id == id) return sp;
        return null;
    }

    public RoomTransitionCell GetTransitionAt(Vector2I cell)
    {
        foreach (var tc in _transitionCells)
            if (tc.Contains(cell)) return tc;
        return null;
    }
}