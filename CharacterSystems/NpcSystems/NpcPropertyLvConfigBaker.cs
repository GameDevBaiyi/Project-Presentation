#if UNITY_EDITOR
using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.Common;
using LowLevelSystems.UISystems.DialogueSystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
public class NpcPropertyLvConfigBaker
{
    public const string TbAiLevel = "tbAILevel";

    public static void RecordNpcPropertyLvConfigFromTb()
    {
        JsonData tbAiLevelJson = JsonUtilities.GetJsonData(TbAiLevel);
        List<NpcPropertyLvConfig> npcPropertyLvConfigs = new List<NpcPropertyLvConfig>();

        foreach (string propertyEnumString in tbAiLevelJson["PropertyID"].Keys)
        {
            NpcPropertyLvConfig npcPropertyLvConfig = new NpcPropertyLvConfig();
            
            //PropertyEnum _propertyEnum
            PropertyEnum propertyEnum = propertyEnumString.ToEnum<PropertyEnum>();
            npcPropertyLvConfig.SetPropertyEnum(propertyEnum);

            JsonData aiPropertyValuesJson = tbAiLevelJson["PropertyID"][propertyEnumString];
            
            //float _lvIncreaseFactor
            float lvIncreaseFactor = 0;
            if (aiPropertyValuesJson.TryGetNestedJson(out JsonData addValueJson,"Add"))
            {
                lvIncreaseFactor = addValueJson.ToFloat() / 100;
            }
            npcPropertyLvConfig.SetLvIncreaseFactor(lvIncreaseFactor);

            npcPropertyLvConfigs.Add(npcPropertyLvConfig);
        }
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetNpcPropertyLvConfigs(npcPropertyLvConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"{TbAiLevel} 数据转 {nameof(NpcPropertyLvConfig)} 成功.");
    }
}
}
#endif