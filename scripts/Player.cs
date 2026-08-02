using Godot;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;

public partial class Player :  Actor
{
    public Vector2I GetIntent()
    {
        var dir = Vector2I.Zero; // wait case
        if (Input.IsActionPressed("move_up"))    dir += Vector2I.Up;
        if (Input.IsActionPressed("move_down"))  dir += Vector2I.Down;
        if (Input.IsActionPressed("move_left"))  dir += Vector2I.Left;
        if (Input.IsActionPressed("move_right")) dir += Vector2I.Right;

        return dir;
    }

    internal List<IItem> debugItemList = TestItems.ItemList;

    internal IItem debugItem = TestItems.ItemList[0];

    internal int debugItemIndex = 0;
    internal void switchDebugItem()
    {
        debugItemIndex++;
        debugItem = debugItemList[debugItemIndex % 4];
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("debug_print")) { inventory.PrintInventory("mouse"); }
        if (Input.IsActionJustPressed("debug_1")) { inventory.AddItem(new ItemEntry(debugItem, 1)); }
        if (Input.IsActionJustPressed("debug_2")) { inventory.RemoveItem(new ItemEntry(debugItem, 1)); }
        if (Input.IsActionJustPressed("debug_3")) { inventory.ClearInventory(); }
        if (Input.IsActionJustPressed("debug_4")) { switchDebugItem(); } }
    }
}
