using System.Collections.Generic;

using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
public static class PropertyEnumExtensions
{
    public static string Name(this PropertyEnum propertyEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (propertyEnum == PropertyEnum.None) return null;

        if (!Details.CommonDesignSO.CharacterConfigHubPy.PropertyEnum_ConfigPy.TryGetValue(propertyEnum,out PropertyConfig propertyConfig))
        {
            Debug.LogError($"未找到: {propertyEnum} 的 {typeof(PropertyConfig)}.");
            return null;
        }
        return propertyConfig.PropertyNameIdPy.TextPy;
    }

    public static string Name(this PropertyEnum propertyEnum,CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (propertyEnum == PropertyEnum.None) return null;

        if (!Details.CommonDesignSO.CharacterConfigHubPy.CharacteristicPropertyEnum_CharacterEnum_ConfigPy.TryGetValue(propertyEnum,
                                                                                                                       out Dictionary<CharacterEnum,CharacteristicPropertyConfig>
                                                                                                                               characterEnum_config))
        {
            Debug.LogError($"未找到: {propertyEnum} 的 {typeof(CharacteristicPropertyConfig)}.");
            return null;
        }
        if (!characterEnum_config.TryGetValue(characterEnum,out CharacteristicPropertyConfig propertyConfig))
        {
            Debug.LogError($"未找到: {propertyEnum} {characterEnum} 的 {typeof(CharacteristicPropertyConfig)}.");
            return null;
        }

        return propertyConfig.PropertyNameIdPy.TextPy;
    }
}
}