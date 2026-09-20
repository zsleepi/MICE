using Godot;
using MICE.scripts.data;
using MICE.scripts.lib;
using System;

public partial class ActorSprite : Sprite2D
{
    [Export] internal Actor parent;

    public override void _Ready()
    {
        this.Texture = SpriteUtils.GetCharSprite(parent.Species, "", 1);
        parent.signature = SpeciesData.GetSpecies(parent.Species).BaseSig;
        UpdateSprite();
    }
    public void UpdateSprite()
    {
        this.Texture = SpriteUtils.RecolorSprite(this.Texture.GetImage(), parent.signature);
    }

    public void RandomizeSprite()
    {
        parent.signature.Randomize();
        UpdateSprite();
    }

    public void FaceDirection(Vector2I dir)
    {
        if (dir.X != 0) this.FlipH = dir.X > 0;
    }
}
