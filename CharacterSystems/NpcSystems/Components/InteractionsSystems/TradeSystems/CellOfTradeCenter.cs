using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
public class CellOfTradeCenter
{
    [ShowInInspector]
    private TradeCenter.CellSideEnum _cellSideEnum;
    public TradeCenter.CellSideEnum CellSideEnumPy => this._cellSideEnum;
    public void SetCellSideEnum(TradeCenter.CellSideEnum cellSideEnum)
    {
        this._cellSideEnum = cellSideEnum;
    }

    [ShowInInspector]
    private ItemPileInTradeCenter _itemPileInTradeCenter;
    public ItemPileInTradeCenter ItemPileInTradeCenterPy => this._itemPileInTradeCenter;
    public void SetItemPileInTradeCenter(ItemPileInTradeCenter itemPileInTradeCenter)
    {
        this._itemPileInTradeCenter = itemPileInTradeCenter;
    }
}
}