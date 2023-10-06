using System;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
[Serializable]
public struct StoreLevel
{
    public int Level;

    public StoreLevel(int level)
    {
        this.Level = level;
    }

    [ShowInInspector]
    public StoreLevelConfig StoreLevelConfigPy => StoreLevelDetails.GetStoreLevelConfig(this);
}
public abstract class StoreLevelDetails : Details
{
    public static StoreLevelConfig GetStoreLevelConfig(StoreLevel storeLevel)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int level = storeLevel.Level;
        if (level == 0) return null;

        if (!Details.CommonDesignSO.InteractionConfigHubPy.StoreLevel_StoreLevelConfigPy.TryGetValue(level,out StoreLevelConfig storeLevelConfig))
        {
            Debug.LogError($"未找到该商店等级对应的配置: {level}");
        }
        return storeLevelConfig;
    }
}
}