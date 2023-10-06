using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.ItemSystems.ItemPileSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems
{
[Serializable]
public class Luggage
{
    [Title("Config")]
    [ShowInInspector]
    public const int MaxNumberOfSlots = 6;

    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private List<CellOfLuggage> _cellsOfLuggage;
    public List<CellOfLuggage> CellsOfLuggagePy => this._cellsOfLuggage;
    public void SetCellsOfLuggage(List<CellOfLuggage> cellsOfLuggage)
    {
        this._cellsOfLuggage = cellsOfLuggage;
    }

    [Title("Methods")]
    [ShowInInspector]
    public int CurrentCountOfSlotsPy => this._cellsOfLuggage.Count;

    [ShowInInspector]
    public bool HasAlreadyFullPy => this._cellsOfLuggage.All(t => t.HasItemsPy);

    public void AddSlots(int addend)
    {
        addend = Mathf.Min(addend,MaxNumberOfSlots - this.CurrentCountOfSlotsPy);
        for (int i = 0; i < addend; i++)
        {
            this._cellsOfLuggage.Add(new CellOfLuggage());
        }
    }

    public void RemoveItemsFromCell(CellOfLuggage cellOfLuggage,int countOfItemsToRemove,out int realCountRemoved)
    {
        ItemPileInLuggage itemPileInLuggage = cellOfLuggage.ItemPileInLuggagePy;
        realCountRemoved = Mathf.Min(itemPileInLuggage.CountOfItemsPy,countOfItemsToRemove);
        itemPileInLuggage.SetCountOfItems(itemPileInLuggage.CountOfItemsPy - realCountRemoved);
        if (itemPileInLuggage.CountOfItemsPy <= 0)
        {
            cellOfLuggage.SetItemPileInLuggage(null);
        }
    }
}
}