using Godot;
using MICE.scripts.data;
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

    public float turnCooldown = 0;

    public void TickTime(float time)
    {
        turnCooldown -= time;
    }
    public void SetAI(INPCController _ai)
    {
        ai = _ai;
    }

    public override void _Ready()
    {
        SetAI(DetermineAi(AItype));
        AudioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        Sprite.Texture = SpriteUtils.GetCharSprite(Species, "", 1);
        signature = SpeciesData.GetSpecies(Species).BaseSig;
        UpdateSprite();
    }

    public override void HandleBump(bool didBump)
    {
        ai.HandleBump(didBump);
    }

    private INPCController DetermineAi(string AItype)
    {
        return AItype switch
        {
            "hostile" => new HostileAI(this),
            "wander" => new WanderAI(this),
            _ => null
        };
    }
}
