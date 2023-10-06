using System;
using System.Collections.Generic;
using System.Linq;

using Common.Extensions;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems.Inheritors.BountyTaskSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
[Serializable]
public class BulletinBoard : ICanRefreshOnDateChanged
{
    [ShowInInspector]
    private readonly CityEnum _cityEnum;

    private readonly List<(BountyTask BountyTask,int IdOnUi)> _bountyTasks = new List<(BountyTask BountyTask,int IdOnUi)>(10);
    [ShowInInspector]
    public List<(BountyTask BountyTask,int IdOnUi)> BountyTasksPy
    {
        get
        {
            this.CheckAndRefresh();
            return this._bountyTasks;
        }
    }

    [ShowInInspector]
    private int _lastDayRefreshed = -1;
    public int LastDayRefreshedPy => this._lastDayRefreshed;

    public BulletinBoard(CityEnum cityEnum)
    {
        this._cityEnum = cityEnum;
    }

    [ShowInInspector]
    public int RefreshCyclePy => Details.SettingsSo.BountyTaskRefreshCycle;

    public void CheckAndRefresh()
    {
        if (!ICanRefreshOnDateChanged.IsTimeToRefresh(this)) return;
        this._bountyTasks.Clear();
        //根据当前 City 探索等级配置随机任务个数.
        City city = this._cityEnum.City();
        CityLevelConfig cityLevelConfig = city.CityExploreSystemPy.CityLevelPy.CityLevelConfigPy;
        int bountyTaskNumber = cityLevelConfig.RangeOfBountyTasksPy.GetRandomNumber();
        for (int i = 0; i < bountyTaskNumber; i++)
        {
            //根据 探索等级配置中的 权重 随机等级.
            int taskLevel = cityLevelConfig.WeightsOfBountyTaskLevelPy.GetRandomIndexByUsingItemAsWeight() + 1;
            //根据任务等级向下取任务. 
            Dictionary<CampEnum,List<BountyTaskConfig>> campEnum_rarity_bountyConfigs = Details.CommonDesignSO.MissionConfigHubPy.GetBountyConfigsBy(taskLevel);
#if UNITY_EDITOR
            if (campEnum_rarity_bountyConfigs == null) return;
#endif
            //确定 阵营类型. 
            int indexOfWeightsOfCamp = Details.SettingsSo.WeightsOfBountyTaskCamp.GetRandomIndexByUsingItemAsWeight();
            CampEnum cityCampEnum = city.CityJurisdictionSystemPy.CurrentCampPy;
            CampEnum taskCampEnum = indexOfWeightsOfCamp == 0 ? CampEnum.None : cityCampEnum;
            BountyTaskConfig bountyTaskConfig = campEnum_rarity_bountyConfigs[taskCampEnum].GetRandomItem();

            //确定 是否为稀有任务.
            bool isRare = Details.SettingsSo.WeightsOfBountyTaskRarity.GetRandomIndexByUsingItemAsWeight() == 1;
            BountyTask bountyTask = new BountyTask(new BountyTaskId(bountyTaskConfig.TaskIdPy),isRare,cityCampEnum);
            this._bountyTasks.Add(new(bountyTask,-1));
        }

        this._lastDayRefreshed = Details.DateSystem.DaysPy;
    }

    public BountyTask RemoveBountyTask(int idInList)
    {
        var elementAtOrDefault = this._bountyTasks.ElementAtOrDefault(idInList);
        if (elementAtOrDefault.BountyTask == null)
        {
            Debug.LogError($"悬赏任务池中, 不应该存在 null. ");
            return null;
        }
        this._bountyTasks.RemoveAt(idInList);
        return elementAtOrDefault.BountyTask;
    }
}
}