using Godot;
using MICE.scripts.data;
using MICE.scripts.lib.itemlogic;
using System.Collections.Generic;

public partial class PlayerController : Node
{
    [Export] TurnManager TurnManager;
    [Export] CameraController CameraController;
    [Export] Player Player;
    [Export] private double MovementInputBuffer = 0.05;
    private double MovementInputTime = 0;
    public override void _Process(double delta)
	{
        HandleMovementInput(delta);
        HandleVerticalTransitionInput();
        InventoryDebug();
    }

	public Vector2I GetInputDirection()
	{
        var dir = Vector2I.Zero; // wait case
        if (Input.IsActionPressed("move_up")) dir += Vector2I.Up;
        if (Input.IsActionPressed("move_down")) dir += Vector2I.Down;
        if (Input.IsActionPressed("move_left")) dir += Vector2I.Left;
        if (Input.IsActionPressed("move_right")) dir += Vector2I.Right;

        return dir;
    }

    public void HandleMovementInput(double delta)
    {
        var direction = GetInputDirection();
        if (direction != Vector2I.Zero)
        {
            CameraController.Unstill(Player);
            MovementInputTime += delta;
            if (MovementInputTime >= MovementInputBuffer)
            {
                TurnManager.TryDoTurn(direction);
            }
        }
        else { MovementInputTime = 0; }
    }

    private void HandleVerticalTransitionInput()
    {
        if (Input.IsActionJustPressed("travel_up"))
        {
            TurnManager.TryVerticalTransition(Vector2I.Up);
        }
        else if (Input.IsActionJustPressed("travel_down"))
        {
            TurnManager.TryVerticalTransition(Vector2I.Down);
        }
    }

    internal void InventoryDebug()
    {
        if (Input.IsActionJustPressed("debug_print")) { Player.inventory.PrintInventory("mouse"); }
        if (Input.IsActionJustPressed("debug_1")) { Player.inventory.AddItem(new ItemEntry(debugItem, 1)); }
        if (Input.IsActionJustPressed("debug_2")) { Player.inventory.RemoveItem(new ItemEntry(debugItem, 1)); }
        if (Input.IsActionJustPressed("debug_3")) { Player.inventory.ClearInventory(); }
        if (Input.IsActionJustPressed("debug_4")) { switchDebugItem(); }
    }
    internal List<IItem> debugItemList = TestItems.ItemList;

    internal IItem debugItem = TestItems.ItemList[0];

    internal int debugItemIndex = 0;
    internal void switchDebugItem()
    {
        debugItemIndex++;
        debugItem = debugItemList[debugItemIndex % 4];
    }
}
