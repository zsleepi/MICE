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
    [Export] public Sprite2D Sprite { get; set; }
    public Signature signature { get; set; }
    public Inventory inventory { get; set; }
    public CoreSkills coreSkills { get; set; }
    public Experience experience { get; set; }

    public Health health { get; set; }
    public Psyche psyche { get; set; }

    public MutableStat MovementSpeed = new MutableStat(10);
    public MutableStat SightRange = new MutableStat(15);
    public MutableStat DarkSightRange = new MutableStat(5);


    public override void _Ready()
    {
        Sprite.Texture = SpriteUtils.GetCharSprite(Species, "", 1);
        signature = SpeciesData.GetSpecies(Species).BaseSig;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        Sprite.Texture = SpriteUtils.RecolorSprite(Sprite.Texture.GetImage(), signature);
    }

    public void RandomizeSprite()
    {
        signature.Randomize();
        UpdateSprite();
    }

    public void FaceDirection(Vector2I dir)
    {
        if (dir.X != 0) Sprite.FlipH = dir.X > 0;
    }
}
