namespace MICE.scripts.lib
{
    public interface IWeapon
    {
        int DamageValue { get; set; }
        string EquipSlot { get; set; }
    }
}