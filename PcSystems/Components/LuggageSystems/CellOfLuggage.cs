using System;

using LowLevelSystems.ItemSystems.ItemPileSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems
{
[Serializable]
public class CellOfLuggage
{
    [ShowInInspector]
    private ItemPileInLuggage _itemPileInLuggage;
    public ItemPileInLuggage ItemPileInLuggagePy => this._itemPileInLuggage;
    public void SetItemPileInLuggage(ItemPileInLuggage itemPileInLuggage)
    {
        this._itemPileInLuggage = itemPileInLuggage;
    }

    [ShowInInspector]
    public bool HasItemsPy => this._itemPileInLuggage != null;
}
}