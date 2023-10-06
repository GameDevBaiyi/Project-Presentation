#if UNITY_EDITOR
using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
public abstract class PropertyConfigBaker
{
    private const string _tbRoleProperty = "tbRoleProperty";

    public static void BakeTbRolePropertyAndCharacteristicProperty()
    {
        BakeTbRolePropertyToPropertyConfig();
        BakeTbRoleDataToCharacteristicPropertyConfig();
    }

    public static void BakeTbRolePropertyToPropertyConfig()
    {
        List<PropertyConfig> propertyConfigs = new List<PropertyConfig>();
        JsonData tbProperty = JsonUtilities.GetJsonData(_tbRoleProperty);

        foreach (string propertyName in tbProperty["PropertyID"].Keys)
        {
            JsonData propertyJson = tbProperty["PropertyID"][propertyName];
            PropertyConfig propertyConfig = new PropertyConfig();

            //PropertyEnum _propertyEnum
            PropertyEnum propertyEnum = propertyName.ToEnumNoError<PropertyEnum>();
            propertyConfig.SetPropertyEnum(propertyEnum);

            //TextId _propertyNameId
            TextId propertyNameId = new TextId();
            if (propertyJson.TryGetNestedJson(out JsonData propertyNameIdJson,"Name"))
            {
                propertyNameId.Id = propertyNameIdJson.ToInt();
            }
            propertyConfig.SetPropertyNameId(propertyNameId);

            //int _propertyTypeId
            int propertyTypeId = 0;
            if (propertyJson.TryGetNestedJson(out JsonData propertyTypeIdJson,"PropertyTypeID"))
            {
                propertyTypeId = propertyTypeIdJson.ToInt();
            }
            propertyConfig.SetPropertyTypeId(propertyTypeId);

            propertyConfigs.Add(propertyConfig);
        }
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetPropertyConfigs(propertyConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"{_tbRoleProperty} 数据转 {nameof(PropertyConfig)} 成功.");
    }

    private static void BakeTbRoleDataToCharacteristicPropertyConfig()
    {
        List<CharacteristicPropertyConfig> playerCharacterConfigRows = new List<CharacteristicPropertyConfig>();
        
        JsonData tbRoleData = JsonUtilities.GetJsonData(CharacterConfigBaker.TbRoleData);

        foreach (string characterEnumString in tbRoleData["RoleID"].Keys)
        {
            JsonData characterData = tbRoleData["RoleID"][characterEnumString];
            for (int i = 1; i < 6; i++)
            {
                if (characterData.TryGetNestedJson(out JsonData p1Json,$"P{i}"))
                {
                    CharacteristicPropertyConfig characteristicPropertyConfig = new CharacteristicPropertyConfig();
                
                    //PropertyEnum _propertyEnum
                    PropertyEnum propertyEnum = (PropertyEnum)400 + i;
                    characteristicPropertyConfig.SetPropertyEnum(propertyEnum);

                    //CharacterEnum _characterEnum
                    CharacterEnum characterEnum = characterEnumString.ToEnum<CharacterEnum>();
                    characteristicPropertyConfig.SetCharacterEnum(characterEnum);
                
                    //TextId _propertyNameId
                    TextId propertyNameId = new TextId(p1Json.ToInt());
                    characteristicPropertyConfig.SetPropertyNameId(propertyNameId);

                    playerCharacterConfigRows.Add(characteristicPropertyConfig);
                }
            }

        }
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetCharacteristicPropertyConfigs(playerCharacterConfigRows);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);
        Debug.Log("性格录制成功");
    }
}
}
#endif