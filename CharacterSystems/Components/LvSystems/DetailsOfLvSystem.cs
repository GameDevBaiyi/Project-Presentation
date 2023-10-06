using System.Collections.Generic;

using Common.Extensions;

using JetBrains.Annotations;

using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.Base;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.LvSystems
{
public abstract class DetailsOfLvSystem : Details
{
    /// <summary>
    /// 升级一次, 获得奖励. 
    /// </summary>
    private static void EarnRewardsForLevelingUp(Pc pc,int currentLv,out List<(PropertyEnum,int)> propertyAddends)
    {
        int lv2PropertyEnumCount = Random.Range(1,4);
        propertyAddends = new List<(PropertyEnum,int)>(lv2PropertyEnumCount); // Ui.
        IEnumerable<PropertyEnum> propertyEnums = PropertySystem.AllLv2PropertyEnumsPy.TakeRandomItems(lv2PropertyEnumCount);
        foreach (PropertyEnum propertyEnum in propertyEnums)
        {
            int addend = Random.Range(1,3);
            pc.PropertySystemPy.PermanentLibraryOfExtra4ValuesPy.ChangeProperty(propertyEnum,PosOfPropertyFormulaEnum.baseAdd,addend);
            propertyAddends.Add(new(propertyEnum,addend));
        }

        if (currentLv % 2 != 0)
        {
            TalentBook talentBook = pc.TalentSystemPy.MainSkillTypeEnum_BookPy[SkillMainTypeEnum.MST1];
            talentBook.AddTalentPoints(1);
        }
        else
        {
            TalentBook talentBook = pc.TalentSystemPy.MainSkillTypeEnum_BookPy[SkillMainTypeEnum.MST2];
            talentBook.AddTalentPoints(1);
        }
    }

    /// <summary>
    /// 升级多次, 获得奖励. 
    /// </summary>
    private static void EarnRewardsForLevelingUp(Pc pc,int originLv,int lvAddend,
                                                 out List<(PropertyEnum,int)> propertyAddends)
    {
        int targetLv = originLv + lvAddend;
        propertyAddends = new List<(PropertyEnum,int)>(lvAddend * 3); // Ui
        for (int currentLv = originLv; currentLv < targetLv; currentLv++)
        {
            EarnRewardsForLevelingUp(pc,currentLv,out List<(PropertyEnum,int)> propertyAddendsOnce);
            foreach ((PropertyEnum,int) propertyAddend in propertyAddendsOnce)
            {
                int findIndex = propertyAddends.FindIndex(t => t.Item1 == propertyAddend.Item1);
                if (findIndex >= 0)
                {
                    PropertyEnum propertyEnum = propertyAddends[findIndex].Item1;
                    int propertyAddendInt = propertyAddend.Item2 + propertyAddends[findIndex].Item2;
                    propertyAddends[findIndex] = new(propertyEnum,propertyAddendInt);
                }
                else
                {
                    propertyAddends.Add(propertyAddend);
                }
            }
        }
    }

    // BaiyiTODO. 升级的奖励不应该和战斗的结算绑定. 
    public static void SettleLvOnLeavingBattle(
        [CanBeNull] out List<(CharacterEnum,ICanDoLevelAnime previousLv,ICanDoLevelAnime currentLv,List<(PropertyEnum,int)>,bool HasLeveledUp)> lvInfoList,out int exp)
    {
        lvInfoList = null;
        exp = 0;
        if (!_battleManager.IsVictory) return;
        List<CharacterEntity> characterEntities = _battleManager.AllCharacterEntitiesOfPcCampPy;
        exp = _battleManager.BattleConfigPy.ExpPy;
        foreach (CharacterEntity characterEntity in characterEntities)
        {
            if (characterEntity is not PcEntity pcEntity) continue;
            Pc pc = pcEntity.PcPy;
            LvSystem pcLvSystem = pc.LvSystemPy;

            //记录之前的 Lv.
            lvInfoList ??= new(characterEntities.Count);
            LvSystem.Level previousLevel = pcLvSystem.LvPy;

            int originLv = pcLvSystem.LvPy.CurrentLv;
            pcLvSystem.AddExp(exp);
            int lvAddend = pcLvSystem.LvPy.CurrentLv - originLv;
            EarnRewardsForLevelingUp(pc,originLv,lvAddend,out List<(PropertyEnum,int)> propertyAddends);
            GetPropertyPointsOnLevelingUp(pc,originLv,pcLvSystem.LvPy.CurrentLv);

            //记录之后的 Lv.
            LvSystem.Level currentLevel = pcLvSystem.LvPy;
            lvInfoList.Add(new(characterEntity.CharacterPy.CharacterEnumPy,previousLevel,currentLevel,propertyAddends,currentLevel.CurrentLv > previousLevel.CurrentLv));
        }
    }

    private static void GetPropertyPointsOnLevelingUp(Pc pc,int originLv,int currentLv)
    {
        //计算之前共有几点属性点. 等级 = 第一个点的等级 + (x - 1) * 间隔. 
        int previousNumber = (originLv - SettingsSo.FirstLevelToObtainPropertyPoint) / SettingsSo.LevelIntervalOfPropertyPoint + 1;
        int currentNumber = (currentLv - SettingsSo.FirstLevelToObtainPropertyPoint) / SettingsSo.LevelIntervalOfPropertyPoint + 1;
        int propertyPoints = currentNumber - previousNumber;
        if (propertyPoints > 0) pc.PropertySystemPy.AddPropertyPoints(propertyPoints);
    }
}
}