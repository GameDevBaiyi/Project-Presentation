using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems.Inheritors.BountyTaskSystems;
using LowLevelSystems.SceneSystems.Base;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
public static class CityEnumExtensions
{
    public static CityConfig CityConfig(this CityEnum cityEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        if (cityEnum == CityEnum.None) return null;

        if (!Details.CommonDesignSO.SceneConfigHubPy.CityEnum_CityConfigPy.TryGetValue(cityEnum,out CityConfig cityConfig))
        {
            Debug.LogError($"未找到 {nameof(CityEnum)}: {cityEnum} 对应的 {nameof(CityConfig)}");
        }

        return cityConfig;
    }

#if UNITY_EDITOR
    public static CityConfig CityConfigEditorOnly(this CityEnum cityEnum)
    {
        CommonDesignSO commonDesignSO = DevTools.ProgrammerTools.DevUtilities.GetCommonConfigSO();
        if (cityEnum == CityEnum.None) return null;

        if (!commonDesignSO.SceneConfigHubPy.CityEnum_CityConfigPy.TryGetValue(cityEnum,out CityConfig cityConfig))
        {
            Debug.LogError($"未找到 {nameof(CityEnum)}: {cityEnum} 对应的 {nameof(CityConfig)}");
        }

        return cityConfig;
    }
#endif

    public static City City(this CityEnum cityEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        if (cityEnum == CityEnum.None) return null;

        if (!Details.SceneHub.CityEnum_SceneIdPy.TryGetValue(cityEnum,out SceneId sceneId))
        {
            Debug.LogError($"SceneHub 中未记录 {nameof(CityEnum)}: {cityEnum} 对应的 {nameof(SceneId)}.");
            return null;
        }

        return sceneId.CityPy;
    }
}
}