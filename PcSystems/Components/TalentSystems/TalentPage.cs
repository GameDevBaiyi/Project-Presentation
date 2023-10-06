using System;
using System.Collections.Generic;

using LowLevelSystems.SkillSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public class TalentPage
{
    [ShowInInspector]
    private readonly SkillSubTypeEnum _skillSubTypeEnum;
    public SkillSubTypeEnum SkillSubTypeEnumPy => this._skillSubTypeEnum;

    [ShowInInspector]
    private readonly Dictionary<int,TalentNode> _id_talentNode;
    public Dictionary<int,TalentNode> Id_TalentNodePy => this._id_talentNode;

    public TalentPage(SkillSubTypeEnum skillSubTypeEnum,Dictionary<int,TalentNode> idTalentNode)
    {
        this._skillSubTypeEnum = skillSubTypeEnum;
        this._id_talentNode = idTalentNode;
    }

    [Title("Methods")]
    public bool HasPrecedingNodeLocked(int nodeId)
    {
        //Debug.
        if (!this._id_talentNode.TryGetValue(nodeId,out TalentNode talentNode))
        {
            Debug.LogError($"该天赋节点在该页不存在: {nodeId}");
            return true;
        }

        bool hasPrecedingNodeLocked = false;
        foreach (int precedingNodeId in talentNode.TalentNodeIdPy.TalentNodeConfigPy.PrecedingNodeIdsPy)
        {
            //Debug.
            if (!this._id_talentNode.TryGetValue(precedingNodeId,out TalentNode precedingNode))
            {
                Debug.LogError($"该天赋节点在该页不存在: {precedingNodeId}. 但是却是 {nodeId} 的前置节点. ");
                continue;
            }

            if (precedingNode.IsLockedPy)
            {
                hasPrecedingNodeLocked = true;
                break;
            }
        }
        return hasPrecedingNodeLocked;
    }
}
}