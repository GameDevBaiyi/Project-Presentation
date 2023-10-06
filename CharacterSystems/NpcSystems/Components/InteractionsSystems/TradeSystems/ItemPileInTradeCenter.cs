using LowLevelSystems.ItemSystems.ItemPileSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
public class ItemPileInTradeCenter : ItemPile
{
    [ShowInInspector]
    private Character.CharacterTypeEnum _characterTypeEnum;
    public Character.CharacterTypeEnum CharacterTypeEnumPy => this._characterTypeEnum;
    public void SetCharacterTypeEnum(Character.CharacterTypeEnum characterTypeEnum)
    {
        this._characterTypeEnum = characterTypeEnum;
    }

    [ShowInInspector]
    private ItemPileInBackpack _itemPileInBackpack;
    public ItemPileInBackpack ItemPileInBackpackPy => this._itemPileInBackpack;
    public void SetItemPileInBackpack(ItemPileInBackpack itemPileInBackpackHub)
    {
        this._itemPileInBackpack = itemPileInBackpackHub;
    }

    [ShowInInspector]

    public override int LeftCapacityPy => 0;
}
}