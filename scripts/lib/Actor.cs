using Godot;
using MICE.scripts.lib;
using MICE.scripts.data;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Actor : Node2D, IActor
{
    [Export] public Vector2I Cell { get; set; }

    [Export] public Sprite2D Sprite;
    public int SpriteVariant = 1;
    public string job = ""; //placeholder
    [Export] public string species = "";

    public Inventory inventory = new();
    public Signature signature;

    public Experience experience = new(1);
    public CoreSkills coreSkills;

    public Health health;
    public Psyche psyche;

    // Actor plays audio using this node
    public AudioStreamPlayer2D AudioPlayer;
    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);
    internal static float MoveSpeed = 8f;

    public void UpdateSprite()
    {
        Sprite.Texture = SpeciesData.GetCharSprite(species, SpriteVariant, job);
        signature = SpeciesData.GetSpecies(species).baseSig;
        Sprite.Texture = SpriteUtils.RecolorSprite(Sprite.Texture.GetImage(), signature);
    }

    public override void _Ready()
    {
        AudioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        UpdateSprite();
    }

    public void FaceDirection(Vector2I dir)
    {
        if (dir.X != 0) Sprite.FlipH = dir.X > 0;
    }

    public virtual void HandleBump(bool didBump)
    {
        if (didBump) { AudioPlayer.Play(); }
    }
}
