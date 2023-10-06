using System.Collections.Generic;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.Config;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions
{
public abstract partial class DetailsOfSpecialActionCondition : Details
{
    [CanBeNull]
    public static SkillSugarConfig CalculateSpecialSkillSugarConfig(NpcEntity npcEntity,List<AIConfig.SpecialAction> specialActions)
    {
        foreach (AIConfig.SpecialAction specialAction in specialActions)
        {
            switch (specialAction.SpecialActionConditionPy.SpecialActionConditionEnumPy)
            {
            case SpecialActionConditionEnum.HasLowHpAlly:
                if (HasLowHpAlly(npcEntity,(HasLowHpAlly)specialAction.SpecialActionConditionPy)) return specialAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
                break;

            case SpecialActionConditionEnum.HasLowHpEnemy:
                if (HasLowHpEnemy(npcEntity,(HasLowHpEnemy)specialAction.SpecialActionConditionPy)) return specialAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
                break;

            case SpecialActionConditionEnum.HasAllyWithoutBuff:
                if (HasAllyWithoutBuff(npcEntity,(HasAllyWithoutBuff)specialAction.SpecialActionConditionPy)) return specialAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
                break;

            case SpecialActionConditionEnum.HasEnemyWithoutBuff:
                if (HasEnemyWithoutBuff(npcEntity,(HasEnemyWithoutBuff)specialAction.SpecialActionConditionPy)) return specialAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
                break;

            case SpecialActionConditionEnum.HasMultipleEnemies:
                if (HasMultipleEnemies(npcEntity,(HasMultipleEnemies)specialAction.SpecialActionConditionPy)) return specialAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
                break;

            default:
                Debug.LogError($" CalculateSpecialSkillSugarConfig 含有未预设的 switch.");
                break;
            }
        }

        return null;
    }
}
}