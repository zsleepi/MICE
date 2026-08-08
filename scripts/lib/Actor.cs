using Godot;
using MICE.scripts.lib;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract partial class Actor : Node2D, IActor
{
    [Export] public float MoveSpeed = 8f;
    [Export] public Vector2I Cell { get; set; }
    [Export] public Sprite2D Sprite;

    public Inventory inventory = new();
    public CoreSkills coreSkills;
    public Signature signature = new ( new Color(1,1,1), new Color(1,1,1) );

    public int health;
    public int maxHealth;
    public int psyche;
    public int maxPsyche;

    // Actor plays audio using this node
    public AudioStreamPlayer2D AudioPlayer;
    public Node2D Node => this;
    public float StepDuration => 1f / Mathf.Max(MoveSpeed, 0.01f);

    public override void _Ready()
    {
        AudioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        RandomizeSprite();
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

    public virtual void HandleBump(bool didBump)
    {
        if (didBump) { AudioPlayer.Play(); }
    }
}
