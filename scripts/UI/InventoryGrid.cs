using Godot;
using MICE.scripts.lib.itemlogic;
using MICE.scripts.UI;

public partial class InventoryGrid : Panel, IMenu
{
    private Inventory _inventory;

    [Export] private PackedScene _gridSlotScene;
    [Export] private GridContainer _itemsGrid;

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override void _Ready()
    {
        // safety check
        if (_inventory == null)
        {
            _inventory = new Inventory();
            GD.PushWarning("InventoryGridWindow: No inventory set, using empty one");
        }

        _inventory.InventoryChange += OnInventoryChanged;
        _Refresh();
    }

    public void Close()
    {
        _inventory.InventoryChange -= OnInventoryChanged;
        QueueFree();
    }

    private void OnInventoryChanged() => _Refresh();

    private void _Refresh()
    {
        // free up existing slots
        foreach (Node child in _itemsGrid.GetChildren())
        {
            child.Free();
        }

        // populating
        foreach (ItemEntry entry in _inventory.PeekInventory())
        {
            var slot = (ItemGridEntryUi)_gridSlotScene.Instantiate();
            slot.SetItemData(entry);
            _itemsGrid.AddChild(slot);
        }

        // fill remaining slots with empty placeholders.. for a 5x6 grid:
        int maxSlots = 30;
        int currentCount = _itemsGrid.GetChildCount();
        for (int i = currentCount; i < maxSlots; i++) {
            var slot = (ItemGridEntryUi)_gridSlotScene.Instantiate();
            slot.SetEmpty();
            _itemsGrid.AddChild(slot);
        }
    }
}