using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
public static class ScenePrefabEnumExtensions
{
    public static ScenePrefabConfig ScenePrefabConfig(this ScenePrefabEnum scenePrefabEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        if (scenePrefabEnum == ScenePrefabEnum.None) return null;
        if (!Details.CommonDesignSO.SceneConfigHubPy.ScenePrefabEnum_ScenePrefabConfigPy.TryGetValue(scenePrefabEnum,out ScenePrefabConfig scenePrefabConfig))
        {
            Debug.LogError($"未找到 {nameof(ScenePrefabEnum)}: {scenePrefabEnum} 对应的 {nameof(Base.ScenePrefabConfig)}");
        }

        return scenePrefabConfig;
    }

#region EditorOnly
#if UNITY_EDITOR
    public static ScenePrefabConfig ScenePrefabConfigEditorOnly(this ScenePrefabEnum scenePrefabEnum)
    {
        CommonDesignSO commonConfigSO = DevTools.ProgrammerTools.DevUtilities.GetCommonConfigSO();
        if (commonConfigSO.SceneConfigHubPy.ScenePrefabEnum_ScenePrefabConfigPy == null) return null;

        if (!commonConfigSO.SceneConfigHubPy.ScenePrefabEnum_ScenePrefabConfigPy.TryGetValue(scenePrefabEnum,out ScenePrefabConfig scenePrefabConfig))
        {
            Debug.LogError($"未找到 {nameof(ScenePrefabEnum)}: {scenePrefabEnum} 对应的 {nameof(Base.ScenePrefabConfig)}");
        }

        return scenePrefabConfig;
    }
#endif
#endregion
}
}