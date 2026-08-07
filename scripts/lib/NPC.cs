using Godot;
using MICE.scripts.lib;
using MICE.scripts.lib.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class NPC : Actor, IActor
{
    public INPCController ai;
    [Export] string AItype = "wander";

    public void SetAI(INPCController _ai)
    {
        ai = _ai;
        ai.Attach(this);
    }

    public override void _Ready()
    {
        SetAI(DetermineAiBySpecies(AItype));
        AudioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        UpdateSprite();
    }
    public override void HandleBump(bool didBump)
    {
    ai.HandleBump(didBump);
    }

    private INPCController DetermineAiBySpecies(string AItype)
    {
        return AItype switch
        {
            "hostile" => new HostileAI(),
            "wander" => new WanderAI(),
            _ => null
        };
    }
}
