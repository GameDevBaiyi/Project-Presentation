using System;
using System.Collections.Generic;
using System.Linq;

using Common.Extensions;
using Common.Utilities;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.BackpackSystems;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.CurrencySystems;
using LowLevelSystems.ItemSystems.ItemPileSystems;
using LowLevelSystems.QualitySystems;
using LowLevelSystems.WorldSystems;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
[Serializable]
public class Trade : Interaction,ICanRefreshOnDateChanged
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Trade;

    [Title("Config")]
    public const int MaxNumberOfPiles = 40;
    public static Vector2 RangeOfPriceFloatingOnFalconMoon = new Vector2(0.8f,1.5f);

    [Title("Data")]
    [ShowInInspector]
    private readonly Backpack _backpack = new Backpack();
    public Backpack BackpackPy => this._backpack;

    [ShowInInspector]
    private List<Item.ItemSubTypeEnum> _itemTypeEnums;
    public List<Item.ItemSubTypeEnum> ItemTypeEnumsPy => this._itemTypeEnums;
    public void SetItemTypeEnums(List<Item.ItemSubTypeEnum> itemTypeEnums)
    {
        this._itemTypeEnums = itemTypeEnums;
    }

    [ShowInInspector]
    private float _currentPricePercentOnFalconMoon;
    public float CurrentPricePercentOnFalconMoonPy => this._currentPricePercentOnFalconMoon;
    public void SetCurrentPricePercentOnFalconMoon(float currentPricePercentOnFalconMoon)
    {
        this._currentPricePercentOnFalconMoon = currentPricePercentOnFalconMoon;
    }

    [ShowInInspector]
    private int _lastDayRefreshed = -1;
    public int LastDayRefreshedPy => this._lastDayRefreshed;
    public void SetLastDayRefreshed(int lastDayRefreshed)
    {
        this._lastDayRefreshed = lastDayRefreshed;
    }

    [ShowInInspector]
    public int RefreshCyclePy => Details.SettingsSo.TradeRefreshCycle;

    [ShowInInspector]
    public Trade(CharacterId characterId,List<Item.ItemSubTypeEnum> itemTypeEnums) : base(characterId)
    {
        this._backpack.AddMaxNumberOfPiles(MaxNumberOfPiles);
        this._itemTypeEnums = itemTypeEnums;
    }

    [Title("Methods")]
    [ShowInInspector]
    public StoreLevel StoreLevelPy => TradeDetails.GetStoreLevel(this);
    public void RefreshProducts()
    {
        //刷新价格百分比.
        this._currentPricePercentOnFalconMoon = Random.Range(RangeOfPriceFloatingOnFalconMoon.x,RangeOfPriceFloatingOnFalconMoon.y);

        //清理现在的所有 ItemPile.
        ItemPileInBackpackHub itemPileInBackpackHub = this._backpack.ItemPileInBackpackHubPy;
        List<int> itemPileIds = itemPileInBackpackHub.InstanceId_InstancePy.Keys.ToList();
        foreach (int itemPileId in itemPileIds)
        {
            itemPileInBackpackHub.RemoveInstance(itemPileId);
        }

        //根据商品类型, 重新生成.
        foreach (Item.ItemSubTypeEnum itemTypeEnum in this._itemTypeEnums)
        {
            //1. 确定道具堆个数.
            ItemSubTypeConfig itemSubTypeConfig = itemTypeEnum.ItemSubTypeConfig();
            Vector2Int quantityRangeForItemType = itemSubTypeConfig.QuantityRangeForProductTypeListPy[this.StoreLevelPy.Level - 1];
            int countOfPiles = Random.Range(quantityRangeForItemType.x,quantityRangeForItemType.y + 1);
            for (int i = 0; i < countOfPiles; i++)
            {
                //确定道具的 Id. 随机.
                ItemConfigId itemConfigId = itemSubTypeConfig.ItemConfigIdsPy.GetRandomItem();
                //确定道具的稀有度. 计算权重.
                int weightedIndex = MathUtilities.RandomizeWithWeights(this.StoreLevelPy.StoreLevelConfigPy.WeightsPy);
                QualityEnum targetQualityEnum = this.StoreLevelPy.StoreLevelConfigPy.QualityEnumsPy[weightedIndex];

                //先生成道具, 再生成堆.
                Item item = ItemFactory.GenerateRandomItem(itemTypeEnum,new ItemConfigIdAndQualityEnum(itemConfigId,targetQualityEnum));
                if (item == null) continue;
                //单个道具的个数. 
                int countOfItems = Random.Range(itemSubTypeConfig.RangeOfProductCountPy.x,itemSubTypeConfig.RangeOfProductCountPy.y + 1);
                this._backpack.GenerateNewPiles(item,countOfItems);
            }
        }
    }

    public void CheckAndRefresh()
    {
        if (!ICanRefreshOnDateChanged.IsTimeToRefresh(this)) return;
        this.RefreshProducts();
        this.SetLastDayRefreshed(Details.DateSystem.DaysPy);
    }
}
public abstract class TradeDetails : Details
{
    public static StoreLevel GetStoreLevel(Trade trade)
    {
        //应该找到当前角色所在的 City, 再找到该 City 的等级所对应的 StoreLevel.
        return trade.CharacterIdPy.NpcPy.SceneIdPy.CityOrParentCityPy.CityExploreSystemPy.CityLevelPy.CityLevelConfigPy.StoreLevelPy;
    }

    /// <summary>
    /// 计算交易中心的本次交易. 
    /// </summary>
    public static void CalculateTransaction(float initialSellValue,float initialPurchaseValue,CharacterId npcId,
                                            CharacterId pcId,out int amountOfTransaction,out CampEnum moneyCampEnum)
    {
        //1. 先确定商人是在 天权还是在隼月.
        Npc npc = npcId.NpcPy;
        CampEnum campEnum = npc.SceneIdPy.CityOrParentCityPy.CityJurisdictionSystemPy.CurrentCampPy;
        int dealPropertyValueOfPc = (int)pcId.PcPy.PropertySystemPy[PropertyEnum.Trade];
        if (campEnum == CampEnum.Sun)
        {
            float purchaseValue = TradeUtilities.PurchaseFormulaOnAuthorityOfSky(initialPurchaseValue,dealPropertyValueOfPc);
            float sellValue = TradeUtilities.SellFormulaOnAuthorityOfSky(initialSellValue,dealPropertyValueOfPc,campEnum.Camp().TaxPy);
            amountOfTransaction = (int)(sellValue - purchaseValue);
            moneyCampEnum = campEnum;
        }
        else
        {
            Trade trade = (Trade)npc.InteractionsPy.InteractionEnum_InteractionPy[InteractionEnum.Trade];
            float purchaseValue = TradeUtilities.PurchaseFormulaOnFalconMoon(initialPurchaseValue,dealPropertyValueOfPc,trade.CurrentPricePercentOnFalconMoonPy);
            float sellValue = TradeUtilities.SellFormulaOnFalconMoon(initialSellValue,dealPropertyValueOfPc,campEnum.Camp().TaxPy);
            amountOfTransaction = (int)new Currency(CampEnum.Sun,sellValue - purchaseValue).ToOtherCurrency(campEnum).NumberPy;
            moneyCampEnum = campEnum;
        }
    }
}
}