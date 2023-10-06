#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions;
using LowLevelSystems.Common;
using LowLevelSystems.QualitySystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
public static class AIConfigBaker
{
    public const string TbAIData = "tbAIData";

    public static void ConvertTbAIDataToAIConfigRow()
    {
        AIConfig commonAIConfig = new AIConfig();
        List<AIConfig> aiConfigRows = new List<AIConfig>();
        JsonData tbAIDataJsonData = JsonUtilities.GetJsonData(TbAIData);

        foreach (string characterEnumString in tbAIDataJsonData["RoleID"].Keys)
        {
            AIConfig aiConfig = new AIConfig();
            JsonData aiConfigJson = tbAIDataJsonData["RoleID"][characterEnumString];

            //CharacterEnum _characterEnum
            CharacterEnum characterEnum = characterEnumString == "RoleCommon" ? CharacterEnum.None : characterEnumString.ToEnum<CharacterEnum>();
            aiConfig.SetCharacterEnum(characterEnum);

            //List<NormalAction> _normalActions
            List<AIConfig.NormalAction> normalActions = new List<AIConfig.NormalAction>();
            if (aiConfigJson.TryGetNestedJson(out JsonData skillMainIdAndQualityEnumJson,"SkillShaft"))
            {
                List<string> skillMainIdAndQualityEnumsString = skillMainIdAndQualityEnumJson.ToList<string>();
                foreach (string skillMainIdAndQualityEnumString in skillMainIdAndQualityEnumsString)
                {
                    string[] skillMainIdAndQualityEnumAndCanBeReplace = skillMainIdAndQualityEnumString.Split("_");
                    SkillMainId skillMainId = new SkillMainId(int.Parse(skillMainIdAndQualityEnumAndCanBeReplace[0]));
                    QualityEnum qualityEnum = skillMainIdAndQualityEnumAndCanBeReplace[1].ToEnum<QualityEnum>();
                    SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum(skillMainId,qualityEnum);
                    bool canBeReplace = true;
                    if (int.Parse(skillMainIdAndQualityEnumAndCanBeReplace[2]) == 1)
                    {
                        canBeReplace = true;
                    }
                    else
                    {
                        canBeReplace = false;
                    }
                    normalActions.Add(new AIConfig.NormalAction(skillMainIdAndQualityEnum,canBeReplace));
                }
            }
            aiConfig.SetNormalActions(normalActions);

            //List<SpecialAction> _specialActions
            List<AIConfig.SpecialAction> specialActions = new List<AIConfig.SpecialAction>();
            if (aiConfigJson.TryGetNestedJson(out JsonData _,"SpecialAction"))
            {
                foreach (string specialActionString in aiConfigJson["SpecialAction"].Keys)
                {
                    JsonData specialActionJson = aiConfigJson["SpecialAction"][specialActionString];
                    switch (specialActionString)
                    {
                    case "HasLowHpAlly":
                        HasLowHpAlly hasLowHpAlly = new HasLowHpAlly();
                        if (specialActionJson.TryGetNestedJson(out JsonData allHpPerJson,"SpecialActionValue"))
                        {
                            List<string> specialActionValueString = allHpPerJson.ToList<string>();
                            //float _hpPercent
                            float hpPercent = float.Parse(specialActionValueString[0]) / 100;
                            hasLowHpAlly.SetHpPercent(hpPercent);
                        }
                        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
                        specialActions.Add(RecordSpecialSkill(specialActionJson,hasLowHpAlly));
                        break;

                    case "HasLowHpEnemy":
                        HasLowHpEnemy hasLowHpEnemy = new HasLowHpEnemy();
                        if (specialActionJson.TryGetNestedJson(out JsonData enemyHpPerJson,"SpecialActionValue"))
                        {
                            List<string> specialActionValueString = enemyHpPerJson.ToList<string>();
                            //float _hpPercent
                            float hpPercent = float.Parse(specialActionValueString[0]) / 100;
                            hasLowHpEnemy.SetHpPercent(hpPercent);
                        }
                        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
                        specialActions.Add(RecordSpecialSkill(specialActionJson,hasLowHpEnemy));
                        break;

                    case "HasAllyWithoutBuff":
                        HasAllyWithoutBuff hasAllyWithoutBuff = new HasAllyWithoutBuff();
                        if (specialActionJson.TryGetNestedJson(out JsonData allBuffAndOtherJson,"SpecialActionValue"))
                        {
                            List<string> specialActionValueString = allBuffAndOtherJson.ToList<string>();
                            //BuffEnum buffEnum
                            BuffEnum buffEnum = specialActionValueString[0].ToEnum<BuffEnum>();
                            hasAllyWithoutBuff.SetBuffEnum(buffEnum);
                        }
                        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
                        specialActions.Add(RecordSpecialSkill(specialActionJson,hasAllyWithoutBuff));
                        break;

                    case "HasEnemyWithoutBuff":
                        HasEnemyWithoutBuff hasEnemyWithoutBuff = new HasEnemyWithoutBuff();
                        if (specialActionJson.TryGetNestedJson(out JsonData enemyBuffAndOtherJson,"SpecialActionValue"))
                        {
                            List<string> specialActionValueString = enemyBuffAndOtherJson.ToList<string>();
                            //BuffEnum buffEnum
                            BuffEnum buffEnum = specialActionValueString[0].ToEnum<BuffEnum>();
                            hasEnemyWithoutBuff.SetBuffEnum(buffEnum);
                        }
                        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
                        specialActions.Add(RecordSpecialSkill(specialActionJson,hasEnemyWithoutBuff));
                        break;

                    case "HasMultipleEnemies":
                        HasMultipleEnemies hasMultipleEnemies = new HasMultipleEnemies();
                        if (specialActionJson.TryGetNestedJson(out JsonData enemyCountJson,"SpecialActionValue"))
                        {
                            List<string> specialActionValueString = enemyCountJson.ToList<string>();
                            //int _enemyCount
                            int enemyCount = int.Parse(specialActionValueString[0]);
                            hasMultipleEnemies.SetEnemyCount(enemyCount);
                        }
                        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
                        specialActions.Add(RecordSpecialSkill(specialActionJson,hasMultipleEnemies));
                        break;
                    }
                }
            }
            aiConfig.SetSpecialActions(specialActions);
            

            if (characterEnumString == "RoleCommon")
            {
                commonAIConfig = aiConfig;
            }
            else
            {
                aiConfigRows.Add(aiConfig);
            }
        }
        //记录 List.
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetCommonAIConfig(commonAIConfig);
        commonDesignSO.CharacterConfigHubPy.SetAiConfigs(aiConfigRows);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"{TbAIData} => {nameof(AIConfig)} 录制成功.");
    }

    private static AIConfig.SpecialAction RecordSpecialSkill(JsonData specialActionJson,SpecialActionCondition specialActionCondition)
    {
        AIConfig.SpecialAction specialAction = new AIConfig.SpecialAction(specialActionCondition,new SkillMainIdAndQualityEnum());
        // SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum();
        if (specialActionJson.TryGetNestedJson(out JsonData specialSkillJson,"SpecialSkill"))
        {
            string[] specialSkills = specialSkillJson.ToString().Split("_");
            SkillMainId skillMainId = new SkillMainId(int.Parse(specialSkills[0]));
            QualityEnum qualityEnum = specialSkills[1].ToEnum<QualityEnum>();
            SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = new SkillMainIdAndQualityEnum(skillMainId,qualityEnum);
            specialAction = new AIConfig.SpecialAction(specialActionCondition,skillMainIdAndQualityEnum);
        }
        return specialAction;
    }
}
}
#endif