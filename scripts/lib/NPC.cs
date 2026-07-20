using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract partial class NPC : Actor
{
    // all NPC actors need this -- vector2I value of zero is "wait"
    public abstract Vector2I DecideDirection(Grid grid);
    public abstract void TickTurn();
}
