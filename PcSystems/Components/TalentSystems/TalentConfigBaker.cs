#if UNITY_EDITOR
using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SkillSystems.Base;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
public static class TalentConfigBaker
{
    public const string TbTalentTree = "tbTalentTree";

    public static void BakeTbTalentTreeToTalentSystemConfigs()
    {
        BakeTbTalentTreeToCharacterConfigHubTalentNodeConfigs();
        BakeTbTalentTreeToCharacterConfigHubTalentBookConfigs();
    }

    private static void BakeTbTalentTreeToCharacterConfigHubTalentNodeConfigs()
    {
        //List<TalentBookConfig.TalentNodeConfig> _talentNodeConfigs
        List<TalentBookConfig.TalentNodeConfig> talentNodeConfigs = new List<TalentBookConfig.TalentNodeConfig>();
        JsonData tbTalentTree = JsonUtilities.GetJsonData(TbTalentTree);
        foreach (string talentBookString in tbTalentTree["MainType"].Keys)
        {
            JsonData tbTalentPageJson = tbTalentTree["MainType"][talentBookString]["SkillType"];
            foreach (string talentPageString in tbTalentPageJson.Keys)
            {
                JsonData talentNodeJson = tbTalentPageJson[talentPageString]["NodeID"];
                foreach (string nodeIdString in talentNodeJson.Keys)
                {
                    TalentBookConfig.TalentNodeConfig talentNodeConfig = new TalentBookConfig.TalentNodeConfig();
                    //int _nodeId
                    int nodeId = int.Parse(nodeIdString);
                    talentNodeConfig.SetNodeId(nodeId);

                    //List<int> _precedingNodeIds
                    List<int> precedingNodeIds = new List<int>();
                    if (talentNodeJson.TryGetNestedJson(out JsonData precedingNodeIdsJson,nodeIdString,"FatherNode"))
                    {
                        precedingNodeIds = precedingNodeIdsJson.ToList<int>();
                    }
                    talentNodeConfig.SetPrecedingNodeIds(precedingNodeIds);
                    
                    //TextId _nameTextId
                    if (talentNodeJson.TryGetNestedJson(out JsonData nameTextIdJson,nodeIdString,"Name")) 
                    {
                        TextId nameTextId = new TextId(nameTextIdJson.ToInt());
                        talentNodeConfig.SetNameTextId(nameTextId);
                    }
                    
                    //TextId _descriptionTextId
                    if (talentNodeJson.TryGetNestedJson(out JsonData descriptionTextIdJson,nodeIdString,"Des")) 
                    {
                        TextId descriptionTextId = new TextId(descriptionTextIdJson.ToInt());
                        talentNodeConfig.SetDescriptionTextId(descriptionTextId);
                    }

                    talentNodeConfigs.Add(talentNodeConfig);
                }
            }
        }
        //记录 List.
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetTalentNodeConfigs(talentNodeConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);
        Debug.Log("所有天赋点录制完成");
    }

    private static void BakeTbTalentTreeToCharacterConfigHubTalentBookConfigs()
    {
        //List<TalentBookConfig> _talentBookConfigs;
        List<TalentBookConfig> talentBookConfigs = new List<TalentBookConfig>();
        JsonData tbTalentTree = JsonUtilities.GetJsonData(TbTalentTree);
        foreach (string talentBookString in tbTalentTree["MainType"].Keys)
        {
            TalentBookConfig talentBookConfig = new TalentBookConfig();
            //MainSkillTypeEnum _mainSkillTypeEnum;
            SkillMainTypeEnum skillMainTypeEnum = talentBookString.ToEnum<SkillMainTypeEnum>();
            talentBookConfig.SetMainSkillTypeEnum(skillMainTypeEnum);
            if (skillMainTypeEnum == SkillMainTypeEnum.MST3) continue;
            List<TalentBookConfig.TalentPageConfig> talentPageConfigs = new List<TalentBookConfig.TalentPageConfig>();
            JsonData tbTalentPageJson = tbTalentTree["MainType"][talentBookString]["SkillType"];

            foreach (string talentPageString in tbTalentPageJson.Keys)
            {
                TalentBookConfig.TalentPageConfig talentPageConfig = new TalentBookConfig.TalentPageConfig();
                //SkillSubTypeEnum _skillSubTypeEnum
                SkillSubTypeEnum skillSubTypeEnum = talentPageString.ToEnum<SkillSubTypeEnum>();
                talentPageConfig.SetSkillSubTypeEnum(skillSubTypeEnum);
                //List<TalentNodeId> _talentNodeIds
                List<TalentNodeId> talentNodeIds = new List<TalentNodeId>();
                foreach (string nodeId in tbTalentPageJson[talentPageString]["NodeID"].Keys)
                {
                    TalentNodeId talentNodeId = new TalentNodeId(int.Parse(nodeId));
                    talentNodeIds.Add(talentNodeId);
                }
                talentNodeIds.TrimExcess();
                talentPageConfig.SetTalentNodeIds(talentNodeIds);

                talentPageConfigs.Add(talentPageConfig);
            }
            talentPageConfigs.TrimExcess();
            talentBookConfig.SetTalentPageConfigs(talentPageConfigs);

            talentBookConfigs.Add(talentBookConfig);
        }
        //记录 List.
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetTalentBookConfigs(talentBookConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);
        Debug.Log("体脉灵脉天赋书录制完成");
    }

    public static List<TalentBookConfig> BakeTbTalentTreeToPcConfigTalentBookConfig(string characterEnumString)
    {
        //List<TalentBookConfig> _talentBookConfigs;
        List<TalentBookConfig> talentBookConfigs = new List<TalentBookConfig>();
        JsonData tbTalentTree = JsonUtilities.GetJsonData(TbTalentTree);
        foreach (string talentBookString in tbTalentTree["MainType"].Keys)
        {
            TalentBookConfig talentBookConfig = new TalentBookConfig();
            //MainSkillTypeEnum _mainSkillTypeEnum;
            SkillMainTypeEnum skillMainTypeEnum = talentBookString.ToEnum<SkillMainTypeEnum>();
            talentBookConfig.SetMainSkillTypeEnum(skillMainTypeEnum);
            if (skillMainTypeEnum != SkillMainTypeEnum.MST3) continue;
            List<TalentBookConfig.TalentPageConfig> talentPageConfigs = new List<TalentBookConfig.TalentPageConfig>();
            if (characterEnumString == "R010101"
             || characterEnumString == "R020002") continue;
            JsonData tbTalentPageJson = tbTalentTree["MainType"][talentBookString]["SkillType"][characterEnumString];
            //由于每个人物只有一页心脉,所以只实例化一个talentPageConfig
            TalentBookConfig.TalentPageConfig talentPageConfig = new TalentBookConfig.TalentPageConfig();
            //SkillSubTypeEnum _skillSubTypeEnum
            SkillSubTypeEnum skillSubTypeEnum = SkillSubTypeEnum.None;
            talentPageConfig.SetSkillSubTypeEnum(skillSubTypeEnum);
            //List<TalentNodeId> _talentNodeIds
            List<TalentNodeId> talentNodeIds = new List<TalentNodeId>();
            foreach (string nodeId in tbTalentPageJson["NodeID"].Keys)
            {
                TalentNodeId talentNodeId = new TalentNodeId(int.Parse(nodeId));
                talentNodeIds.Add(talentNodeId);
            }
            talentPageConfig.SetTalentNodeIds(talentNodeIds);

            talentPageConfigs.Add(talentPageConfig);

            talentBookConfig.SetTalentPageConfigs(talentPageConfigs);
            talentBookConfigs.Add(talentBookConfig);
        }
        return talentBookConfigs;
    }
}
}
#endif