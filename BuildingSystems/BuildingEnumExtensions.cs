using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
public static class BuildingEnumExtensions
{
    public static string GetBuildingName(this BuildingEnum buildingEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        if (buildingEnum == BuildingEnum.None) return null;

        if (!Details.CommonDesignSO.SceneConfigHubPy.BuildingEnum_BuildingConfigPy.TryGetValue(buildingEnum,out BuildingConfig buildingConfig))
        {
            Debug.LogError($"未找到 {nameof(BuildingEnum)}: {buildingEnum} 对应的 {nameof(BuildingConfig)}");
            return null;
        }

        return buildingConfig.NameIdPy.TextPy;
    }
}
}