using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents some basic grid functionality..... reusing this from an old project
/// and kind of hoping it still holds up
/// </summary>
public class Grid
{
    public const int TILE_SIZE = 18;

    private readonly HashSet<Vector2I> _impassable = [];
    private readonly HashSet<Vector2I> _viewObstruction = [];
    private readonly HashSet<Vector2I> _lightSource = [];
    private readonly HashSet<Vector2I> _lit = [];
    private readonly Dictionary<Vector2I, IActor> _occupants = [];

    public void SetImpassable(Vector2I cell) => _impassable.Add(cell);
    public void SetObstruction(Vector2I cell) => _viewObstruction.Add(cell);
    public void SetLightSource(Vector2I cell) => _lightSource.Add(cell);
    public void SetLit(Vector2I cell) => _lit.Add(cell);

    public bool IsWalkable(Vector2I cell) => !_impassable.Contains(cell);

    public HashSet<Vector2I> GetLightSources() => _lightSource;

    public bool IsFree(Vector2I cell) =>
        IsWalkable(cell) && !_occupants.ContainsKey(cell);
    public bool IsViewObstruction(Vector2I cell) => _viewObstruction.Contains(cell);
    public bool IsLightSource(Vector2I cell) => _lightSource.Contains(cell);
    public bool IsLit(Vector2I cell) => _lit.Contains(cell);
    public IActor ActorAt(Vector2I cell) =>
        _occupants.TryGetValue(cell, out var a) ? a : null;

    public void Register(IActor actor, Vector2I cell) => _occupants[cell] = actor;

    public void Move(Vector2I from, Vector2I to)
    {
        if (!_occupants.TryGetValue(from, out var a)) return;
        _occupants.Remove(from);
        _occupants[to] = a;
    }

    public void Clear()
    {
        _impassable.Clear();
        _occupants.Clear();
        _viewObstruction.Clear();
    }
}
