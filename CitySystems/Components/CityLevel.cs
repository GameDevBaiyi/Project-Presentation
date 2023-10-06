using System;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
[Serializable]
public struct CityLevel
{
    public int Level;

    public CityLevel(int level)
    {
        this.Level = level;
    }

    [ShowInInspector]
    public CityLevelConfig CityLevelConfigPy => CityLevelDetails.GetCityLevelConfig(this);
}
public abstract class CityLevelDetails : Details
{
    public static CityLevelConfig GetCityLevelConfig(CityLevel cityLevel)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int level = cityLevel.Level;
        if (!CommonDesignSO.SceneConfigHubPy.CityLevel_CityLevelConfigPy.TryGetValue(level,out CityLevelConfig cityLevelConfig))
        {
            Debug.LogError($"未找到该等级对应的 城镇等级配置. : {level}");
        }
        return cityLevelConfig;
    }
}
}