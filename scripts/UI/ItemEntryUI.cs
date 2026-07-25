using Godot;
using MICE.scripts.lib.itemlogic;
using System;

public partial class ItemEntryUI : Panel
{
    private TextureRect iconNode;
    private Label labelNode;
    private string label;
    private Texture2D icon;

    public override void _Ready()
    {
        iconNode = GetNode<TextureRect>("HBoxContainer/Icon");
        labelNode = GetNode<Label>("HBoxContainer/Label");
        iconNode.Texture = icon;
        labelNode.Text = label;
    }

    public void SetItemData(ItemEntry entry)
    {
        icon = entry.Item.Sprite;
        label = entry.Item.Name;
        if (entry.Quantity > 1) { label += " x" + entry.Quantity; }
    }
}
