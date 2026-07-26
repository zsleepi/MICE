using Godot;
using System;
using MICE.scripts.UI;


public partial class InteractionManager : Control
{
    string currentInteraction = "none";
    Player playerCharacter;
    [Export] internal PackedScene playerInventoryScene;
    [Export] internal PackedScene playerInventoryGridScene;

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
            OpenPlayerInventoryGrid();
        }
        else if (currentInteraction == "playerInventoryGrid")
        {
            CloseInteraction();
        }
        else
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

    public void OpenPlayerInventoryGrid()
    {
        currentInteraction = "playerInventoryGrid";
        InventoryGrid Scene = (InventoryGrid)playerInventoryGridScene.Instantiate();
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
