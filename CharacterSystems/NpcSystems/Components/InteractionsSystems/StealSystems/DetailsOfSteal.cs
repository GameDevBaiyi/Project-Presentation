using System.Linq;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.Common;
using LowLevelSystems.DateSystems;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.CurrencySystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.WorldSystems;

using UnityEngine;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.StealSystems
{
public abstract class DetailsOfSteal : Details
{
    /// <summary>
    /// 偷窃成功率:（40+身手属性*5-npc警觉值）%
    /// </summary>
    /// <returns></returns>
    private static float CalculateStealingSuccessRate(float stealProperty,float npcAlert)
    {
        return Mathf.Clamp((40f + stealProperty * 5f - npcAlert) / 100f,0f,1f);
    }
    public static float CalculateStealingSuccessRate(PcEntity pcEntity,NpcEntity npcEntity)
    {
        Steal steal = npcEntity.NpcPy.InteractionsPy.GetInteraction<Steal>(InteractionEnum.Steal);
        return CalculateStealingSuccessRate(pcEntity.PcPy.PropertySystemPy[PropertyEnum.Steal],steal.AlertPy);
    }

    /// <summary>
    /// 被发现概率 = 1 -（50f + 身手属性 * 3f）%
    /// </summary>
    private static float CalculateGetFundRate(float stealProperty)
    {
        return 1f - (50f + stealProperty * 3f) / 100f;
    }

    /// <summary>
    /// 不是偷窃失败或者成功, 而是由于 兴致值不足 等原因无法偷窃. 
    /// </summary>
    public static bool CanSteal(PcEntity pcEntity,NpcEntity npcEntity)
    {
        //功能: 如果当前 Pc 的兴致值足够, 才可以进行偷窃.
        Pc pc = pcEntity.PcPy;
        InterestSystem interestSystem = pc.InterestSystemPy;
        if (!DetailsOfInterestSystem.HasEnoughInterestAndPromptIfNot(interestSystem,SettingsSo.InterestCostForSteal)) return false;

        Steal steal = npcEntity.NpcPy.InteractionsPy.GetInteraction<Steal>(InteractionEnum.Steal);
        if (steal == null) return false;
        if (steal.ItemAndCountListPy.Count <= 0) return false;

        return true;
    }

    /// <summary>
    /// 检测本次偷窃行为的结果.
    /// </summary>
    public static void CheckStealingResults(PcEntity pcEntity,NpcEntity npcEntity,out bool isSuccessful,
                                            out bool isFund)
    {
        Pc pc = pcEntity.PcPy;
        Steal steal = npcEntity.NpcPy.InteractionsPy.GetInteraction<Steal>(InteractionEnum.Steal);
        float stealProperty = pc.PropertySystemPy[PropertyEnum.Steal];
        isSuccessful = Random.Range(0f,1f) < CalculateStealingSuccessRate(stealProperty,steal.AlertPy);
        isFund = !isSuccessful && Random.Range(0f,1f) < CalculateGetFundRate(stealProperty);
    }

    /// <summary>
    /// 由 确认按钮 调用. 
    /// </summary>
    public static void ProcessStealingResults(PcEntity pcEntity,NpcEntity npcEntity,bool isSuccessful,
                                              bool isFund)
    {
        Pc pc = pcEntity.PcPy;
        InterestSystem interestSystem = pc.InterestSystemPy;
        Steal steal = npcEntity.NpcPy.InteractionsPy.GetInteraction<Steal>(InteractionEnum.Steal);
        DetailsOfInterestSystem.ChangeLimitedValue(interestSystem,-SettingsSo.InterestCostForSteal);
        //Npc 偷窃次数增加. 
        steal.AddTimesStolen();
        if (isSuccessful)
        {
            //加入背包. 
            (Item,int) randomItem = steal.StealRandomItem();
            HeronTeam.BackpackPy.AddItems(randomItem.Item1,randomItem.Item2);
            return;
        }

        if (!isFund) return;

        //先刷新偷窃被发现次数.
        pc.AddTimesTheftWasDetected();

        //找到当前阵营对应的偷窃配置.
        CampEnum currentCamp = SceneHub.CurrentCityEnumPy.City().CityJurisdictionSystemPy.CurrentCampPy;
        StealingPenaltiesConfig stealingPenaltiesConfig = SettingsSo.StealingPenaltiesConfigs.Find(t => t.CampEnumPy == currentCamp);
        //找到当前偷窃次数对应的偷窃配置.
        StealingPenaltiesConfig.PenaltiesConfig penaltiesConfig = stealingPenaltiesConfig.PenaltiesConfigsPy.ElementAtOrDefault(pc.TimesTheftWasDetectedPy)
                                                               ?? stealingPenaltiesConfig.PenaltiesConfigsPy.Last();
        //扣钱.
        DetailsOfCurrency.DeductCurrency(penaltiesConfig.FinePy);

        //确定刑期. 偷窃被发现次数 * 倍数.
        Date sentence = new Date(pc.TimesTheftWasDetectedPy * penaltiesConfig.TimesPy,0f);
        DetailsOfPrison.CheckAndArrestAsync(pc,sentence);
    }
}
}