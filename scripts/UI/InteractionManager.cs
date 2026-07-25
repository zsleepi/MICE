using Godot;
using System;
using MICE.scripts.UI;


public partial class InteractionManager : Control
{
    string currentInteraction = "none";
    Player playerCharacter;
    [Export] internal PackedScene playerInventoryScene;

    public override void _Ready()
    {
        playerCharacter = (Player)GetNode("/root/MainGame/World/Player");
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("inventory")) { TogglePlayerInventory(); }
    }


    public void TogglePlayerInventory()
    {
        if (currentInteraction == "playerInventory")
        {
            CloseInteraction();
        } else
        {
            CloseInteraction();
            OpenPlayerInventory();
        }
    }

    public void OpenPlayerInventory()
    {
        currentInteraction = "playerInventory";
        InventoryWindow Scene = (InventoryWindow)playerInventoryScene.Instantiate();
        Scene.SetInventory(playerCharacter.inventory);
        this.AddChild(Scene);
    }

    public void CloseInteraction()
    {
        foreach (IMenu child in this.GetChildren())
        {
            child.Close();
        }
        currentInteraction = "none";
    }
}
