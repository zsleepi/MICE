using Godot;
using MICE.scripts.lib;
using System.Collections.Generic;

/// <summary>
/// Represents some basic grid functionality..... reusing this from an old project
/// and kind of hoping it still holds up
/// </summary>
public static class Grid
{
    // consts
    public const int TILE_SIZE = 18;

    // lists
    private static readonly HashSet<Vector2I> _impassable = [];
    private static readonly HashSet<Vector2I> _viewObstruction = [];
    private static readonly HashSet<Vector2I> _lightSource = [];
    private static readonly HashSet<Vector2I> _lightmap = [];
    private static readonly Dictionary<Vector2I, IActor> _occupants = [];

    // setters
    public static void SetImpassable(Vector2I cell) => _impassable.Add(cell);
    public static void SetObstruction(Vector2I cell) => _viewObstruction.Add(cell);
    public static void SetLightSource(Vector2I cell) => _lightSource.Add(cell);
    public static void SetLit(Vector2I cell) => _lightmap.Add(cell);

    // getters
    public static bool IsWalkable(Vector2I cell) => !_impassable.Contains(cell);
    public static HashSet<Vector2I> GetLightSources() => _lightSource;
    public static bool IsFree(Vector2I cell) =>
        IsWalkable(cell) && !_occupants.ContainsKey(cell);
    public static bool IsViewObstruction(Vector2I cell) => _viewObstruction.Contains(cell);
    public static bool IsLightSource(Vector2I cell) => _lightSource.Contains(cell);
    public static bool IsLit(Vector2I cell) => _lightmap.Contains(cell);

    // mass data setters
    public static void UpdateGridData(Room room)
    {
        ClearData();
        foreach (Vector2I cell in room.Terrain.GetUsedCells())
        {
            TileData data = room.Terrain.GetCellTileData(cell);
            if (data != null && !data.GetCustomData("Walkable").AsBool())
            {
                SetImpassable(cell);
            }
            if (data != null && data.GetCustomData("ObstructsView").AsBool())
            {
                SetObstruction(cell);
            }
            if (data != null && data.GetCustomData("LightSource").AsBool())
            {
                SetLightSource(cell);
            }
        }
        RecalculateLightMap();
    }
    public static void RecalculateLightMap()
    {
        _lightmap.Clear();
        foreach (Vector2I lightSource in GetLightSources())
        {
            List<Vector2I> litTiles = GridUtils.GetVisibleArea(lightSource, 5);
            foreach (Vector2I Coord in litTiles)
            {
                SetLit(Coord);
            }
        }
    }
    public static void ClearData()
    {
        _impassable.Clear();
        _occupants.Clear();
        _viewObstruction.Clear();
        _lightmap.Clear();
        _lightSource.Clear();
    }

    // actors
    public static IActor ActorAt(Vector2I cell) =>
    _occupants.TryGetValue(cell, out var a) ? a : null;
    public static void Register(IActor actor, Vector2I cell) => _occupants[cell] = actor;
    public static void Move(Vector2I from, Vector2I to)
    {
        if (!_occupants.TryGetValue(from, out var actor)) return;
        _occupants.Remove(from);
        _occupants[to] = actor;
    }
}
