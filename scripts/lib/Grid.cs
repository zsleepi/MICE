using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents some basic grid functionality..... reusing this from an old project
/// and kind of hoping it still holds up
/// </summary>
public static class Grid
{
    public const int TILE_SIZE = 18;

    private static readonly HashSet<Vector2I> _impassable = [];
    private static readonly HashSet<Vector2I> _viewObstruction = [];
    private static readonly HashSet<Vector2I> _lightSource = [];
    private static readonly HashSet<Vector2I> _lit = [];
    private static readonly Dictionary<Vector2I, IActor> _occupants = [];

    public static void SetImpassable(Vector2I cell) => _impassable.Add(cell);
    public static void SetObstruction(Vector2I cell) => _viewObstruction.Add(cell);
    public static void SetLightSource(Vector2I cell) => _lightSource.Add(cell);
    public static void SetLit(Vector2I cell) => _lit.Add(cell);

    public static bool IsWalkable(Vector2I cell) => !_impassable.Contains(cell);

    public static HashSet<Vector2I> GetLightSources() => _lightSource;

    public static bool IsFree(Vector2I cell) =>
        IsWalkable(cell) && !_occupants.ContainsKey(cell);
    public static bool IsViewObstruction(Vector2I cell) => _viewObstruction.Contains(cell);
    public static bool IsLightSource(Vector2I cell) => _lightSource.Contains(cell);
    public static bool IsLit(Vector2I cell) => _lit.Contains(cell);
    public static IActor ActorAt(Vector2I cell) =>
        _occupants.TryGetValue(cell, out var a) ? a : null;

    public static void Register(IActor actor, Vector2I cell) => _occupants[cell] = actor;

    public static void Move(Vector2I from, Vector2I to)
    {
        if (!_occupants.TryGetValue(from, out var a)) return;
        _occupants.Remove(from);
        _occupants[to] = a;
    }

    public static void Clear()
    {
        _impassable.Clear();
        _occupants.Clear();
        _viewObstruction.Clear();
        _lit.Clear();
        _lightSource.Clear();
    }
}
