using System;
using System.Collections.Generic;

using Common.Extensions;

using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.QualitySystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.StealSystems
{
[Serializable]
public class Steal : Interaction,ICanRefreshOnDateChanged
{
    [Title("Data")]
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Steal;

    [ShowInInspector]
    private int _timesStolen;
    public void AddTimesStolen()
    {
        this._timesStolen++;
    }

    [ShowInInspector]
    private int _lastDayRefreshed = -1;
    public int LastDayRefreshedPy => this._lastDayRefreshed;

    [ShowInInspector]
    public int RefreshCyclePy => Details.SettingsSo.StealRefreshCycle;

    [ShowInInspector]
    private int _wealthLevel;
    public int WealthLevelPy => this._wealthLevel;

    [ShowInInspector]
    private readonly List<(Item,int)> _itemAndCountList = new List<(Item,int)>(3);
    public List<(Item,int)> ItemAndCountListPy => this._itemAndCountList;

    public Steal(CharacterId characterId) : base(characterId)
    {
    }

    [Title("Methods")]
    [ShowInInspector]
    public float AlertPy
    {
        get
        {
            float alert = 0f;
            if (_timesStolen > 0)
            {
                for (int i = 1; i < this._timesStolen + 1; i++)
                {
                    alert += 10f * Mathf.Pow(2f,i - 1f);
                }
            }
            return alert;
        }
    }

    public void CheckAndRefresh()
    {
        if (!ICanRefreshOnDateChanged.IsTimeToRefresh(this)) return;
        this._timesStolen = 0;
        this._wealthLevel = Details.SettingsSo.NpcWealthLevelWeights.GetRandomIndexByUsingItemAsWeight();
        this._lastDayRefreshed = Details.DateSystem.DaysPy;
        this._itemAndCountList.Clear();
        for (int i = 0; i < 3; i++)
        {
            int indexOfItemSubTypeConfig = Details.CommonDesignSO.ItemConfigHubPy.WeightsOfItemSubtypesCanBeStolenPy.GetRandomIndexByUsingItemAsWeight();
            ItemSubTypeConfig itemSubTypeConfig = Details.CommonDesignSO.ItemConfigHubPy.ItemSubTypeConfigsPy[indexOfItemSubTypeConfig];
            ItemConfigId itemConfigId = itemSubTypeConfig.ItemConfigIdsPy.GetRandomItem();
            int numberStolen = Random.Range(itemSubTypeConfig.RangeOfItemsStolenPy.x,itemSubTypeConfig.RangeOfItemsStolenPy.y + 1);
            //根据城镇探索等级和 Npc 贫富确定稀有度. 
            Vector2Int stolenItemRarityFluctuationRange = Details.SettingsSo.StolenItemRarityFluctuationRangeCorrespondingYoNPCsWealthLevel[this._wealthLevel];
            int rarityAddend = Random.Range(stolenItemRarityFluctuationRange.x,stolenItemRarityFluctuationRange.y + 1);
            QualityEnum qualityEnum = Details.SceneHub.CurrentCityEnumPy.City().CityExploreSystemPy.CityLevelPy.CityLevelConfigPy.QualityEnumOfStolenItemPy.Add(rarityAddend);
            ItemConfigIdAndQualityEnum itemConfigIdAndQualityEnum = new ItemConfigIdAndQualityEnum(itemConfigId,qualityEnum);
            Item item = ItemFactory.GenerateRandomItem(itemSubTypeConfig.ItemSubTypeEnumPy,itemConfigIdAndQualityEnum);
            if (item != null)
            {
                this._itemAndCountList.Add(new(item,numberStolen));
            }
        }
    }

    public (Item,int) StealRandomItem()
    {
        if (this._itemAndCountList.Count <= 0) return default((Item,int));
        int randomIndex = Random.Range(0,this._itemAndCountList.Count);
        (Item,int) item = this._itemAndCountList[randomIndex];
        this._itemAndCountList.RemoveAt(randomIndex);
        return item;
    }
}
}