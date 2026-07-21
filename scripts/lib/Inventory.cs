using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Inventory
    {
        public Inventory() { }
        // accessing the inventory will be done only through class methods!
        // this class is meant for use in actors and storage objects, or for any mutable collection of itemEntries.
        internal List<ItemEntry> itemList = [];

        public void TakeItem(ItemEntry item, Inventory toInventory)
        {
            RemoveItem(item);
            toInventory.AddItem(item);
        }

        // this function is a reformulation of TakeItem. it does the opposite thing using the same code.
        public void StoreItem(ItemEntry item, Inventory fromInventory)
        {
            fromInventory.TakeItem(item, this);
        }

        public void TakeAll(Inventory toInventory)
        {
            foreach (var item in itemList)
            {
                TakeItem(item, toInventory);
            }
        }
        public void StoreAll(Inventory fromInventory)
        {
            foreach (var item in fromInventory.PeekInventory())
            {
                StoreItem(item, fromInventory);
            }
        }

        public void Drop(ItemEntry item, Vector2I tile)
        {
            // drop item on floor
        }

        public void ClearInventory() { itemList.Clear(); }

        // basic logic for manipulating entries
        public void RemoveItem(ItemEntry item) {
            if (this.HasItem(item.Item)) {
                int amountToRemove = item.Quantity;
                ItemEntry foundItem = itemList.Find(i => i.Item == item.Item);
                if (amountToRemove >= foundItem.Quantity)
                {
                    itemList.Remove(foundItem);
                }
                else
                {
                    itemList.Find(i => i.Item == item.Item).Quantity -= item.Quantity;
                }
            } else { GD.PrintErr("Tried to remove non-existent item") ; }
        }
        public void AddItem(ItemEntry item) {
            if (itemList.Exists(i => i.Item == item.Item) && item.Item.Stackable)
            {
                itemList.Find(i => i.Item == item.Item).Quantity += item.Quantity;
            } else { itemList.Add(item); }
        }

        public bool HasItem(Item item)
        {
            return itemList.Exists(i => i.Item == item);
        }

        public bool HasItemAmount(ItemEntry item)
        {
            return itemList.Exists(i => i.Quantity >= item.Quantity && i.Item == item.Item);
        }

        // this is a public way to view an instance of the inventory.
        public List<ItemEntry> PeekInventory() { return itemList; }

        // implement sorting capabilties here
        public void SortInventory(string SortBy)
        {
            // UNFINISHED
        }

        // later, this could be used to let items modify themselves or activate effects. Examples: cursed items or rotting food
        public void TickInventory(int time)
        {

        }

        public void PrintInventory(string ActorName)
        {
            GD.Print($"{ActorName}'s inventory:");
            GD.Print($"{itemList}");
            foreach (ItemEntry item in itemList)
            {
                string _name = item.Item.Name;
                int _quantity = item.Quantity;
                GD.Print($"{_quantity} {_name}");
            }
        }
    }
}
