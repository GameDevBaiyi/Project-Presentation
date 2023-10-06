using System.Collections.Generic;

using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
public static class HammerActionEnumExtensions
{
    public static HammerActionConfig HammerActionConfig(this Hammering.HammerActionEnum hammerActionEnum,Hammering.IntensityEnum intensityEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (hammerActionEnum == Hammering.HammerActionEnum.None) return null;

        if (!Details.CommonDesignSO.InteractionConfigHubPy.HammerActionEnum_IntensityEnum_HammerActionConfigPy.TryGetValue(hammerActionEnum,
                                                                                                                                out Dictionary<Hammering.IntensityEnum,
                                                                                                                                            HammerActionConfig>
                                                                                                                                        intensityEnum_hammerActionConfig))
        {
            Debug.LogError($"未找到: {hammerActionEnum} 的 {typeof(HammerActionConfig)}.");
            return null;
        }
        if (!intensityEnum_hammerActionConfig.TryGetValue(intensityEnum,out HammerActionConfig hammerActionConfig))
        {
            Debug.LogError($"未找到: {hammerActionEnum} 对应的: {intensityEnum} 的 {typeof(HammerActionConfig)}.");
            return null;
        }

        return hammerActionConfig;
    }
}
}