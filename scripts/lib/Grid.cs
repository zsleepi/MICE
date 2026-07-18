using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents some basic grid functionality..... reusing this from an old project
/// and kind of hoping it still holds up
/// </summary>
public class Grid
{
    private readonly HashSet<Vector2I> _impassable = [];
    private readonly Dictionary<Vector2I, IActor> _occupants = [];

    public void SetImpassable(Vector2I cell) => _impassable.Add(cell);

    public bool IsFree(Vector2I cell) =>
        !_impassable.Contains(cell) && !_occupants.ContainsKey(cell);

    public IActor ActorAt(Vector2I cell) =>
        _occupants.TryGetValue(cell, out var a) ? a : null;

    public void Register(IActor actor, Vector2I cell) => _occupants[cell] = actor;

    public void Move(Vector2I from, Vector2I to)
    {
        if (!_occupants.TryGetValue(from, out var a)) return;
        _occupants.Remove(from);
        _occupants[to] = a;
    }
}
