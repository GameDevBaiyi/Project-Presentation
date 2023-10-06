using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.BackpackSystems;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.ItemPileSystems;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems
{
public abstract class LuggageDetails : Details
{
    /// <summary>
    /// 如果 双方 是 可堆叠的道具, 那么就是 堆叠. 如果不可堆叠, 就是放入或者替换. 
    /// </summary>
    public static void PutItemsOfBackpackIntoLuggage(Backpack backpack,ItemPileInBackpack itemPileInBackpack,CellOfLuggage cellOfLuggage)
    {
        Item item = itemPileInBackpack.ItemPy;
        int maxNumberOfStacksInLuggage = item.ItemConfigIdAndQualityEnumPy.ItemConfigPy.ItemSubTypeEnumPy.ItemSubTypeConfig().MaxNumberOfStacksInLuggagePy;

        //如果该格子上没有道具, 那么就是放入.
        if (!cellOfLuggage.HasItemsPy)
        {
            //从 背包道具堆 尝试移除该道具在行囊中的最大堆叠数量.
            backpack.RemoveItemsFromPile(itemPileInBackpack,maxNumberOfStacksInLuggage,out int realCountRemoved);

            //生成一个道具堆, 放到 Cell 上.
            ItemPileInLuggage itemPileInLuggage = ItemPileInLuggageFactory.GenerateItemPileInLuggage(item,realCountRemoved);
            cellOfLuggage.SetItemPileInLuggage(itemPileInLuggage);
            return;
        }

        //如果该格子上有道具, 看这两种道具是否可堆叠.
        ItemPileInLuggage pileInLuggage = cellOfLuggage.ItemPileInLuggagePy;
        //如果可堆叠.
        bool isStackable = ItemDetails.CheckIfStackable(item,pileInLuggage.ItemPy);
        if (isStackable)
        {
            //如果可堆叠. 那么从背包道具堆尝试移除 剩余 capacity 的道具. 
            backpack.RemoveItemsFromPile(itemPileInBackpack,pileInLuggage.LeftCapacityPy,out int realCountRemoved);
            //将实际数量的道具数量添加到行囊中.
            pileInLuggage.SetCountOfItems(pileInLuggage.CountOfItemsPy + realCountRemoved);
            return;
        }

        //如果不可堆叠, 那么就是替换. 先尝试从 背包道具堆 中移除 行囊最大堆叠数量 的道具. 
        backpack.RemoveItemsFromPile(itemPileInBackpack,maxNumberOfStacksInLuggage,out int realCountRemovedLc);
        //再记录 行囊道具堆的 item 和 count.
        Item itemInLuggage = pileInLuggage.ItemPy;
        int countOfItemsInLuggage = pileInLuggage.CountOfItemsPy;
        //将行囊道具堆的添加到背包中.
        backpack.AddItems(itemInLuggage,countOfItemsInLuggage);
        //将 行囊道具堆的 Item 和 count 重新设置.
        pileInLuggage.SetItem(item);
        pileInLuggage.SetCountOfItems(realCountRemovedLc);
    }

    /// <summary>
    /// 将一整个 行囊格 中的道具, 放入到 背包中.
    /// </summary>
    public static void RemoveItemPileInLuggageIntoBackpack(CellOfLuggage cellOfLuggage)
    {
        ItemPileInLuggage itemPileInLuggage = cellOfLuggage.ItemPileInLuggagePy;
        cellOfLuggage.SetItemPileInLuggage(null);
        Item item = itemPileInLuggage.ItemPy;
        HeronTeam.BackpackPy.AddItems(item,itemPileInLuggage.CountOfItemsPy);
    }
}
}