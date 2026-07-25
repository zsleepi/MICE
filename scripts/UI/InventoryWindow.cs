using Godot;
using System;
using MICE.scripts.lib.itemlogic;
using MICE.scripts.UI;

public partial class InventoryWindow : Panel, IMenu
{
    private Inventory inventoryInstance;
    [Export] private PackedScene itemScene;
    [Export] private VBoxContainer itemsContainer;

    public void SetInventory(Inventory toinventory)
    {
        GD.Print("setting inventory UI data!");
        inventoryInstance = toinventory;
    }
    private void OnInventoryChange() => _Refresh();
    public override void _Ready()
    {
        GD.Print("getting inventory ready!");
        inventoryInstance.InventoryChange += OnInventoryChange;

        _Refresh();
    }

    public void Close()
    {
        inventoryInstance.InventoryChange -= OnInventoryChange;
        QueueFree();
    }

    public void _Refresh() {
        GD.Print("refreshing inventory UI!");
        if (itemsContainer.GetChildCount() > 0 )
        {
            foreach (var item in itemsContainer.GetChildren())
            {
                item.Free();
            }
        }

        foreach (var entry in inventoryInstance.PeekInventory())
        {
            ItemEntryUI _item = (ItemEntryUI)itemScene.Instantiate();
            _item.SetItemData(entry);
            itemsContainer.AddChild(_item);
        }
    }
}
