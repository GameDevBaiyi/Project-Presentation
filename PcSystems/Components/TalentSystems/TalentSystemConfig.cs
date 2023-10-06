using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SkillSystems.Base;

using UnityEngine;
using UnityEngine.Serialization;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public class TalentBookConfig
{
    [Serializable]
    public class TalentPageConfig
    {
        [SerializeField]
        private SkillSubTypeEnum _skillSubTypeEnum;
        public SkillSubTypeEnum SkillSubTypeEnumPy => this._skillSubTypeEnum;
        public void SetSkillSubTypeEnum(SkillSubTypeEnum skillSubTypeEnum)
        {
            this._skillSubTypeEnum = skillSubTypeEnum;
        }

        [SerializeField]
        private List<TalentNodeId> _talentNodeIds;
        public List<TalentNodeId> TalentNodeIdsPy => this._talentNodeIds;
        public void SetTalentNodeIds(List<TalentNodeId> talentNodeIds)
        {
            this._talentNodeIds = talentNodeIds;
        }
    }

    [Serializable]
    public class TalentNodeConfig
    {
        [SerializeField]
        private int _nodeId;
        public int NodeIdPy => this._nodeId;
        public void SetNodeId(int nodeId)
        {
            this._nodeId = nodeId;
        }

        [SerializeField]
        private List<int> _precedingNodeIds;
        public List<int> PrecedingNodeIdsPy => this._precedingNodeIds;
        public void SetPrecedingNodeIds(List<int> precedingNodeIds)
        {
            this._precedingNodeIds = precedingNodeIds;
        }

        [SerializeField]
        private TextId _nameTextId;
        public TextId NameTextIdPy => this._nameTextId;
        public void SetNameTextId(TextId nameTextId)
        {
            this._nameTextId = nameTextId;
        }

        [SerializeField]
        private TextId _descriptionTextId;
        public TextId DescriptionTextIdPy => this._descriptionTextId;
        public void SetDescriptionTextId(TextId descriptionTextId)
        {
            this._descriptionTextId = descriptionTextId;
        }
    }

    [FormerlySerializedAs("_mainSkillTypeEnum")]
    [SerializeField]
    private SkillMainTypeEnum _skillMainTypeEnum;
    public SkillMainTypeEnum SkillMainTypeEnumPy => this._skillMainTypeEnum;
    public void SetMainSkillTypeEnum(SkillMainTypeEnum skillMainTypeEnum)
    {
        this._skillMainTypeEnum = skillMainTypeEnum;
    }

    [SerializeField]
    private List<TalentPageConfig> _talentPageConfigs;
    public List<TalentPageConfig> TalentPageConfigsPy => this._talentPageConfigs;
    public void SetTalentPageConfigs(List<TalentPageConfig> talentPageConfigs)
    {
        this._talentPageConfigs = talentPageConfigs;
    }
}
}