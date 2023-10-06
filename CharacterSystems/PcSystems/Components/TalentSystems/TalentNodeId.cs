using System;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public struct TalentNodeId
{
    [SerializeField]
    public int NodeId;
    public TalentNodeId(int nodeId)
    {
        this.NodeId = nodeId;
    }

    [ShowInInspector]
    public TalentBookConfig.TalentNodeConfig TalentNodeConfigPy => TalentNodeIdDetails.GetTalentNodeConfig(this);
}

public abstract class TalentNodeIdDetails : Details
{
    public static TalentBookConfig.TalentNodeConfig GetTalentNodeConfig(TalentNodeId talentNodeId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int nodeId = talentNodeId.NodeId;
        if (nodeId == 0) return null;

        if (!CommonDesignSO.CharacterConfigHubPy.Id_TalentNodeConfigsPy.TryGetValue(nodeId,out TalentBookConfig.TalentNodeConfig talentNodeConfig))
        {
            Debug.LogError($"未找到 天赋节点: {nodeId}");
        }

        return talentNodeConfig;
    }
}
}