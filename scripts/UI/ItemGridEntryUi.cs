using Godot;
using MICE.scripts.lib.itemlogic;

public partial class ItemGridEntryUi : Panel
{
    private TextureRect _icon;
    private Label _quantityLabel;

    // stuff that gets set before ready fires
    private Texture2D _pendingTexture;
    private int _pendingQuantity;
    private bool _isEmpty = true;

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
        _quantityLabel = GetNode<Label>("QtyLabel");

        // applies what's necessary, we're ready!
        if (_isEmpty)
        {
            SetEmpty();
        }
        else
        {
            _icon.Texture = _pendingTexture;
            _quantityLabel.Text = _pendingQuantity > 1 ? _pendingQuantity.ToString() : "";
        }
    }

    public void SetItemData(ItemEntry entry)
    {
        _isEmpty = false;
        _pendingTexture = entry.Item.Sprite;
        _pendingQuantity = entry.Quantity;

        // if already ready, apply NOWWWW
        if (_icon != null)
        {
            _icon.Texture = _pendingTexture;
            _quantityLabel.Text = _pendingQuantity > 1 ? _pendingQuantity.ToString() : "";
        }
    }

    public void SetEmpty()
    {
        _isEmpty = true;
        _pendingTexture = null;
        _pendingQuantity = 0;

        if (_icon != null)
        {
            _icon.Texture = null;
            _quantityLabel.Text = "";
            Modulate = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        }
    }
}