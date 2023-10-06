using System;
using System.Collections.Generic;

using LowLevelSystems.ItemSystems.Base;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
[Serializable]
public class TradeConfig : InteractionConfig
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Trade;

    [SerializeField]
    private List<Item.ItemSubTypeEnum> _itemTypeEnums = new List<Item.ItemSubTypeEnum>();
    public List<Item.ItemSubTypeEnum> ItemTypeEnumsPy => this._itemTypeEnums;
    public void SetItemTypeEnums(List<Item.ItemSubTypeEnum> itemTypeEnums)
    {
        this._itemTypeEnums = itemTypeEnums;
    }
}
}