using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions;
using LowLevelSystems.SkillSystems.Base;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
[Serializable]
public class AIConfig
{
    [Serializable]
    public class NormalAction
    {
        [SerializeField]
        private SkillMainIdAndQualityEnum _skillMainIdAndQualityEnum;
        public SkillMainIdAndQualityEnum SkillMainIdAndQualityEnumPy => this._skillMainIdAndQualityEnum;

        [SerializeField]
        private bool _canBeReplaced;
        public bool CanBeReplacedPy => this._canBeReplaced;

        public NormalAction(SkillMainIdAndQualityEnum skillMainIdAndQualityEnum,bool canBeReplaced)
        {
            this._skillMainIdAndQualityEnum = skillMainIdAndQualityEnum;
            this._canBeReplaced = canBeReplaced;
        }
    }

    [Serializable]
    public class SpecialAction
    {
        [SerializeReference]
        private SpecialActionCondition _specialActionCondition;
        public SpecialActionCondition SpecialActionConditionPy => this._specialActionCondition;

        [SerializeField]
        private SkillMainIdAndQualityEnum _skillMainIdAndQualityEnum;
        public SkillMainIdAndQualityEnum SkillMainIdAndQualityEnumPy => this._skillMainIdAndQualityEnum;

        public SpecialAction(SpecialActionCondition specialActionCondition,SkillMainIdAndQualityEnum skillMainIdAndQualityEnum)
        {
            this._specialActionCondition = specialActionCondition;
            this._skillMainIdAndQualityEnum = skillMainIdAndQualityEnum;
        }
    }

    [SerializeField]
    private CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;
    public void SetCharacterEnum(CharacterEnum characterEnum)
    {
        this._characterEnum = characterEnum;
    }

    [SerializeField]
    private List<NormalAction> _normalActions = new List<NormalAction>();
    public List<NormalAction> NormalActionsPy => this._normalActions;
    public void SetNormalActions(List<NormalAction> normalActions)
    {
        this._normalActions = normalActions;
    }

    [SerializeField]
    private List<SpecialAction> _specialActions = new List<SpecialAction>();
    public List<SpecialAction> SpecialActionsPy => this._specialActions;
    public void SetSpecialActions(List<SpecialAction> specialActions)
    {
        this._specialActions = specialActions;
    }
}
}