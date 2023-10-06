using System.Collections.Generic;
using System.Linq;

using Common.DataTypes;
using Common.Template;

using LowLevelSystems.Common;
using LowLevelSystems.HeronTeamSystems.Base;
using LowLevelSystems.HeronTeamSystems.Components;
using LowLevelSystems.ItemSystems.BackpackSystems;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.ItemPileSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
public class TradeCenter : Singleton<TradeCenter>
{
    public enum CellSideEnum
    {
        None,
        Left,
        Right,
    }

    [Title("Data")]
    private readonly ObjectPool<CellOfTradeCenter> _cellPool;
    private readonly ObjectPool<ItemPileInTradeCenter> _itemPilePool;

    [ShowInInspector]
    private List<CellOfTradeCenter> _leftCells;
    public List<CellOfTradeCenter> LeftCellsPy => this._leftCells;

    [ShowInInspector]
    private List<CellOfTradeCenter> _rightCells;
    public List<CellOfTradeCenter> RightCellsPy => this._rightCells;

    [ShowInInspector]
    private Backpack.SortingCriteriaEnum _sortingCriteriaEnum;
    public Backpack.SortingCriteriaEnum SortingCriteriaEnumPy => this._sortingCriteriaEnum;

    public void SetSortingCriteriaEnum(Backpack.SortingCriteriaEnum sortingCriteriaEnum)
    {
        this._sortingCriteriaEnum = sortingCriteriaEnum;
    }

    [ShowInInspector]
    private CampEnum _moneyCampEnum;
    public CampEnum MoneyCampEnumPy => this._moneyCampEnum;

    [ShowInInspector]
    private int _amountOfTransaction;
    public int AmountOfTransactionPy => this._amountOfTransaction;

    public TradeCenter()
    {
        //构造一个 Row 的生成 Function.
        CellOfTradeCenter CellGenerator()
        {
            return new CellOfTradeCenter();
        }
        this._cellPool = new ObjectPool<CellOfTradeCenter>(100,CellGenerator);
        this._itemPilePool = new ObjectPool<ItemPileInTradeCenter>(100,() => new ItemPileInTradeCenter());
        this._rightCells = new List<CellOfTradeCenter>(50);
        this._leftCells = new List<CellOfTradeCenter>(50);

        this._sortingCriteriaEnum = Backpack.SortingCriteriaEnum.Rarity;
    }

    [Title("每次交易的 Cache")]
    [ShowInInspector]
    private CharacterId _npcId;
    public CharacterId NpcIdPy => this._npcId;
    public void SetNpcId(int npcId)
    {
        this._npcId.InstanceId = npcId;
    }

    [ShowInInspector]
    private CharacterId _pcId;
    public CharacterId PcIdPy => this._pcId;
    public void SetPcId(int pcId)
    {
        this._pcId.InstanceId = pcId;
    }

    [ShowInInspector]
    private HashSet<int> _tradableMainTypeIdSet;
    public HashSet<int> TradableMainTypeIdSetPy => this._tradableMainTypeIdSet;

    private Trade _trade;
    private Backpack _npcBackpack;
    private Backpack _pcBackpack;

    /// <summary>
    /// 初始化左右两边的格子, 分别显示两个背包的道具堆. 
    /// </summary>
    [Title("Methods")]
    IEnumerable<ItemPileInTradeCenter> AddedPilesOnLeftPy => this._leftCells.Where(t => t.ItemPileInTradeCenterPy is { CharacterTypeEnumPy: Character.CharacterTypeEnum.Pc })
                                                                 .Select(t => t.ItemPileInTradeCenterPy);
    IEnumerable<ItemPileInTradeCenter> AddedPilesOnRightPy => this._rightCells.Where(t => t.ItemPileInTradeCenterPy is { CharacterTypeEnumPy: Character.CharacterTypeEnum.Npc })
                                                                  .Select(t => t.ItemPileInTradeCenterPy);

    public void Initialize(int npcId,int pcId)
    {
        this.SetNpcId(npcId);
        this.SetPcId(pcId);

        this._trade = (Trade)(this._npcId.NpcPy.InteractionsPy.InteractionEnum_InteractionPy[InteractionEnum.Trade]);
        //需要尝试刷新货物.
        this._trade.CheckAndRefresh();
        this._npcBackpack = this._trade.BackpackPy;
        this._tradableMainTypeIdSet = this._npcBackpack.ItemPileInBackpackHubPy.InstanceId_InstancePy.Values
                                          .Select(t => (int)t.ItemPy.ItemSubTypeEnumPy.ItemSubTypeConfig().ItemMainTypeEnumPy)
                                          .Distinct()
                                          .ToHashSet();
        this._pcBackpack = HeronTeam.InstancePy.BackpackPy;

        //初始化后, 主动刷新一次 cells.
        this.InitializeCellsAndSetAmountTo0(null);
    }

    public void InitializeCellsAndSetAmountTo0(int? mainTypeId)
    {
        this.InitializeCells(this._npcBackpack,this._leftCells,mainTypeId,CellSideEnum.Left);
        this.InitializeCells(this._pcBackpack,this._rightCells,mainTypeId,CellSideEnum.Right);

        this.SortAllCells();

        //交易金额归零.
        this._moneyCampEnum = this._npcId.NpcPy.SceneIdPy.CityOrParentCityPy.CityJurisdictionSystemPy.CurrentCampPy;
        this._amountOfTransaction = 0;
    }

    private void InitializeCells(Backpack backpack,List<CellOfTradeCenter> cells,int? mainTypeId,
                                 CellSideEnum cellSideEnum)
    {
        //所有的 cells 放进 pool 中.
        foreach (CellOfTradeCenter cell in cells)
        {
            this._cellPool.ReturnItemToPool(cell);
            if (cell.ItemPileInTradeCenterPy != null)
            {
                this._itemPilePool.ReturnItemToPool(cell.ItemPileInTradeCenterPy);
            }
        }
        cells.Clear();

        List<ItemPileInBackpack> itemPilesInBackpack = backpack.GetAllItemPilesOfMainType(mainTypeId);
        int leftCapacity = backpack.LeftCapacityPy;
        int countOfItemPiles = itemPilesInBackpack.Count;
        int countOfCells = countOfItemPiles + leftCapacity;
        for (int i = 0; i < countOfCells; i++)
        {
            CellOfTradeCenter cellOfTradeCenter = this._cellPool.GetItemFromPool();
            cellOfTradeCenter.SetCellSideEnum(cellSideEnum);
            if (i >= countOfItemPiles)
            {
                cellOfTradeCenter.SetItemPileInTradeCenter(null);
            }
            else
            {
                ItemPileInTradeCenter itemPileInTradeCenter = this._itemPilePool.GetItemFromPool();
                itemPileInTradeCenter.SetItemPileTypeEnum(ItemPile.ItemPileTypeEnum.ItemPileInTradeCenter);
                ItemPileInBackpack itemPileInBackpack = itemPilesInBackpack[i];
                itemPileInTradeCenter.SetItem(itemPileInBackpack.ItemPy);
                itemPileInTradeCenter.SetCountOfItems(itemPileInBackpack.CountOfItemsPy);
                itemPileInTradeCenter.SetCharacterTypeEnum(cellSideEnum == CellSideEnum.Left ? Character.CharacterTypeEnum.Npc : Character.CharacterTypeEnum.Pc);
                itemPileInTradeCenter.SetItemPileInBackpack(itemPileInBackpack);

                cellOfTradeCenter.SetItemPileInTradeCenter(itemPileInTradeCenter);
            }
            cells.Add(cellOfTradeCenter);
        }
    }

    private readonly List<ItemPileInTradeCenter> _orderedPilesCache = new List<ItemPileInTradeCenter>(50);

    public void SortAllCells()
    {
        switch (this._sortingCriteriaEnum)
        {
        case Backpack.SortingCriteriaEnum.Rarity:
            this._orderedPilesCache.Clear();
            this._orderedPilesCache.AddRange(this._leftCells.Where(t => t.ItemPileInTradeCenterPy != null)
                                                 .Select(t => t.ItemPileInTradeCenterPy)
                                                 .OrderByDescending(t => t.CharacterTypeEnumPy)
                                                 .ThenByDescending(t => t.ItemPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy.QualityConfig().LevelPy));
            foreach (CellOfTradeCenter cellOfTradeCenter in this._leftCells)
            {
                cellOfTradeCenter.SetItemPileInTradeCenter(null);
            }
            int index = 0;
            foreach (ItemPileInTradeCenter leftOrderedPile in this._orderedPilesCache)
            {
                this._leftCells[index].SetItemPileInTradeCenter(leftOrderedPile);
                index++;
            }

            this._orderedPilesCache.Clear();
            this._orderedPilesCache.AddRange(this._rightCells.Where(t => t.ItemPileInTradeCenterPy != null)
                                                 .Select(t => t.ItemPileInTradeCenterPy)
                                                 .OrderBy(t => t.CharacterTypeEnumPy)
                                                 .ThenByDescending(t => t.ItemPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy.QualityConfig().LevelPy));
            foreach (CellOfTradeCenter cellOfTradeCenter in this._rightCells)
            {
                cellOfTradeCenter.SetItemPileInTradeCenter(null);
            }
            index = 0;
            foreach (ItemPileInTradeCenter rightOrderedPile in this._orderedPilesCache)
            {
                this._rightCells[index].SetItemPileInTradeCenter(rightOrderedPile);
                index++;
            }
            break;

        case Backpack.SortingCriteriaEnum.UnitPrice:
            this._orderedPilesCache.Clear();
            this._orderedPilesCache.AddRange(this._leftCells.Where(t => t.ItemPileInTradeCenterPy != null)
                                                 .Select(t => t.ItemPileInTradeCenterPy)
                                                 .OrderByDescending(t => t.CharacterTypeEnumPy)
                                                 .ThenByDescending(t => t.ItemPy.UnitPricePy));
            foreach (CellOfTradeCenter cellOfTradeCenter in this._leftCells)
            {
                cellOfTradeCenter.SetItemPileInTradeCenter(null);
            }
            index = 0;
            foreach (ItemPileInTradeCenter leftOrderedPile in this._orderedPilesCache)
            {
                this._leftCells[index].SetItemPileInTradeCenter(leftOrderedPile);
                index++;
            }

            this._orderedPilesCache.Clear();
            this._orderedPilesCache.AddRange(this._rightCells.Where(t => t.ItemPileInTradeCenterPy != null)
                                                 .Select(t => t.ItemPileInTradeCenterPy)
                                                 .OrderBy(t => t.CharacterTypeEnumPy)
                                                 .ThenByDescending(t => t.ItemPy.UnitPricePy));
            foreach (CellOfTradeCenter cellOfTradeCenter in this._rightCells)
            {
                cellOfTradeCenter.SetItemPileInTradeCenter(null);
            }
            index = 0;
            foreach (ItemPileInTradeCenter rightOrderedPile in this._orderedPilesCache)
            {
                this._rightCells[index].SetItemPileInTradeCenter(rightOrderedPile);
                index++;
            }
            break;

        default:
            Debug.LogError($"未实现: {this._sortingCriteriaEnum}");
            break;
        }
    }

    /// <summary>
    /// 将一个 cell 上的指定数量的道具 放到另一边的 cell 上, 位置是紧贴着最后一个道具. 
    /// </summary>
    public void RemoveItemsToAnotherSide(CellOfTradeCenter cell,int itemCountToRemove)
    {
        //如果该格子上没有道具.
        ItemPileInTradeCenter itemPileInTradeCenter = cell.ItemPileInTradeCenterPy;
        if (itemPileInTradeCenter == null) return;

        int countOfItems = itemPileInTradeCenter.CountOfItemsPy;
        if (itemCountToRemove > countOfItems)
        {
            Debug.LogError($"该格子上的道具数不足. 道具数为: {countOfItems}, 交易道具数为: {itemCountToRemove}. ");
            itemCountToRemove = countOfItems;
        }

        //将道具放到另外一边的末尾的空白格子.
        List<CellOfTradeCenter> cellsOfAnotherSide = cell.CellSideEnumPy == CellSideEnum.Left ? this._rightCells : this._leftCells;
        CellOfTradeCenter lastCellOnAnotherSide = cellsOfAnotherSide[^1];
        //如果最后一个格子都满了.
        if (lastCellOnAnotherSide.ItemPileInTradeCenterPy != null) return;

        //生成一个新的堆, 放到该格子上.
        ItemPileInTradeCenter newItemPile = this._itemPilePool.GetItemFromPool();
        newItemPile.SetItemPileTypeEnum(ItemPile.ItemPileTypeEnum.ItemPileInTradeCenter);
        newItemPile.SetItem(itemPileInTradeCenter.ItemPy);
        newItemPile.SetCountOfItems(itemCountToRemove);
        newItemPile.SetCharacterTypeEnum(itemPileInTradeCenter.CharacterTypeEnumPy);
        newItemPile.SetItemPileInBackpack(itemPileInTradeCenter.ItemPileInBackpackPy);
        lastCellOnAnotherSide.SetItemPileInTradeCenter(newItemPile);

        //原格子减少指定的道具, 如果数量清空, 就清理掉该格子的道具堆引用.
        itemPileInTradeCenter.SetCountOfItems(itemPileInTradeCenter.CountOfItemsPy - itemCountToRemove);
        if (itemPileInTradeCenter.CountOfItemsPy <= 0)
        {
            this._itemPilePool.ReturnItemToPool(itemPileInTradeCenter);
            cell.SetItemPileInTradeCenter(null);
        }

        //计算成交价格.
        this.CalculateAmountOfTransaction();

        //排序一次.
        this.SortAllCells();
    }

    private void CalculateAmountOfTransaction()
    {
        //玩家获得 和 付出.
        float sellValue = this.AddedPilesOnLeftPy.Sum(t => t.ItemPy.UnitPricePy * t.CountOfItemsPy);
        float purchaseValue = this.AddedPilesOnRightPy.Sum(t => t.ItemPy.UnitPricePy * t.CountOfItemsPy);
        TradeDetails.CalculateTransaction(sellValue,purchaseValue,this._npcId,this._pcId,out this._amountOfTransaction,
                                          out this._moneyCampEnum);
    }

    public void ConfirmTransaction()
    {
        List<ItemPileInTradeCenter> addedPilesOnLeft = this.AddedPilesOnLeftPy.ToList();
        List<ItemPileInTradeCenter> addedPilesOnRight = this.AddedPilesOnRightPy.ToList();

        //玩家获得 和 付出.
        float sellValue = addedPilesOnLeft.Sum(t => t.ItemPy.UnitPricePy * t.CountOfItemsPy);
        float purchaseValue = addedPilesOnRight.Sum(t => t.ItemPy.UnitPricePy * t.CountOfItemsPy);
        TradeDetails.CalculateTransaction(sellValue,purchaseValue,this._npcId,this._pcId,out this._amountOfTransaction,
                                          out this._moneyCampEnum);
        //检测玩家是否有足够的钱.
        Wallet wallet = HeronTeam.InstancePy.WalletPy;
        if (this._amountOfTransaction < 0)
        {
            if (!wallet.HasEnoughMoney(this._moneyCampEnum,-this._amountOfTransaction)) return;
        }
        wallet.ChangeLimitedMoney(this._moneyCampEnum,this._amountOfTransaction);

        //左右两边的背包移除道具.
        foreach (ItemPileInTradeCenter addedItemPileAndCount in addedPilesOnRight)
        {
            this._npcBackpack.RemoveItemsFromPile(addedItemPileAndCount.ItemPileInBackpackPy,addedItemPileAndCount.CountOfItemsPy,out _);
        }
        foreach (ItemPileInTradeCenter addedItemPileAndCount in addedPilesOnLeft)
        {
            this._pcBackpack.RemoveItemsFromPile(addedItemPileAndCount.ItemPileInBackpackPy,addedItemPileAndCount.CountOfItemsPy,out _);
        }

        //左侧背包增加道具, 这些道具不用堆叠. 右侧背包增加道具, 需要堆叠.
        foreach (ItemPileInTradeCenter addedItemPileAndCount in addedPilesOnLeft)
        {
            this._npcBackpack.GenerateNewPiles(addedItemPileAndCount.ItemPileInBackpackPy.ItemPy,addedItemPileAndCount.CountOfItemsPy);
        }
        foreach (ItemPileInTradeCenter addedItemPileAndCount in addedPilesOnRight)
        {
            this._pcBackpack.AddItems(addedItemPileAndCount.ItemPileInBackpackPy.ItemPy,addedItemPileAndCount.CountOfItemsPy);
        }
    }
}
}