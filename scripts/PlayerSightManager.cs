using Godot;
using MICE.scripts.lib;
using System;
using System.Collections.Generic;

public partial class PlayerSightManager : Node
{
    [Export] Player player;
    [Export] World world;
    [Export] TurnManager turnManager;
    [Export] MaskTiles masktiles;
    internal HashSet<Vector2I> _seenTiles = [];

    public override void _Ready()
    {
        turnManager.PlayerActed += OnPlayerActed;
    }

    private void OnPlayerActed(Vector2I _)
    {
        DoSight();
    }

    public HashSet<Vector2I> GetSeenTiles() => _seenTiles;
    public void DoSight()
    {
        List<Vector2I> cells = SightLogic.GetVisibleTilesOptimized(player.Cell, 15, world._grid, 5);
        masktiles.SetMaskedCells(cells);
        foreach (Vector2I cell in cells)
        {
            _seenTiles.Add(cell);
        }
    }
}
