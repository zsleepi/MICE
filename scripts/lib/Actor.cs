using Godot;
using MICE.scripts.data;
using MICE.scripts.lib;
using MICE.scripts.lib.itemlogic;
using MICE.scripts.lib.stats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

public partial class Actor : Node2D, IActor
{
    [Export] public Vector2I Cell { get; set; }
    [Export] public string Species;
    [Export] public ActorSprite Sprite { get; set; }
    public Signature signature { get; set; }
    public Inventory inventory { get; set; }
    public CoreSkills coreSkills { get; set; }
    public Experience experience { get; set; }

    public Health health { get; set; }
    public Psyche psyche { get; set; }

    public MutableStat MovementSpeed = new MutableStat(10);
    public MutableStat SightRange = new MutableStat(20);
    public MutableStat DarkSightRange = new MutableStat(4);
}
