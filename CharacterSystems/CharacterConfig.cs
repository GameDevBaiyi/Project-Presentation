using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
[SuppressMessage("ReSharper","InconsistentNaming")]
public class CharacterConfig
{
    [Serializable]
    public struct PropertyEnumAndValue
    {
        [SerializeField]
        private PropertyEnum _propertyEnum;
        public PropertyEnum PropertyEnumPy => this._propertyEnum;
        public void SetPropertyEnum(PropertyEnum propertyEnum)
        {
            this._propertyEnum = propertyEnum;
        }

        [SerializeField]
        private float _value;
        public float ValuePy => this._value;
        public void SetValue(float value)
        {
            this._value = value;
        }

        public PropertyEnumAndValue(PropertyEnum propertyEnum,float value)
        {
            this._propertyEnum = propertyEnum;
            this._value = value;
        }
    }

    [SerializeField]
    protected CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;
    public void SetCharacterEnum(CharacterEnum characterEnum)
    {
        this._characterEnum = characterEnum;
    }

    [SerializeField]
    protected string _characterName;
    public string CharacterNamePy => this._characterName;
    public void SetCharacterName(string characterName)
    {
        this._characterName = characterName;
    }

    [SerializeField]
    protected Vector2Int _headIconPosInBattle;
    public Vector2Int HeadIconPosInBattlePy => this._headIconPosInBattle;
    public void SetHeadIconPosInBattle(Vector2Int headIconPosInBattle)
    {
        this._headIconPosInBattle = headIconPosInBattle;
    }

    [SerializeField]
    protected Vector2Int _headIconPosInPreparation;
    public Vector2Int HeadIconPosInPreparationPy => this._headIconPosInPreparation;
    public void SetHeadIconPosInPreparation(Vector2Int headIconPosInPreparation)
    {
        this._headIconPosInPreparation = headIconPosInPreparation;
    }

    [SerializeField]
    protected List<PropertyEnumAndValue> _propertyEnumAndValueList = new List<PropertyEnumAndValue>();
    public List<PropertyEnumAndValue> PropertyEnumAndValueListPy => this._propertyEnumAndValueList;
    protected Dictionary<PropertyEnum,float> _propertyEnum_initialValue;
    public void SetPropertyEnumAndValueList(List<PropertyEnumAndValue> propertyEnumAndValueList)
    {
        this._propertyEnumAndValueList = propertyEnumAndValueList;
    }

    [SerializeReference]
    protected List<InteractionConfig> _interactionConfigs = new List<InteractionConfig>();
    public List<InteractionConfig> InteractionConfigsPy => this._interactionConfigs;
    public void SetInteractionConfigs(List<InteractionConfig> interactionConfigs)
    {
        this._interactionConfigs = interactionConfigs;
    }

    [SerializeField]
    private int _manualCardType;
    public int ManualCardTypePy => this._manualCardType;
    public void SetManualCardType(int manualCardType)
    {
        this._manualCardType = manualCardType;
    }

    [Title("Methods")]
    public void Initialize()
    {
        this._propertyEnum_initialValue = new Dictionary<PropertyEnum,float>(this._propertyEnumAndValueList.Count);
        foreach (PropertyEnumAndValue propertyEnumAndValue in this._propertyEnumAndValueList)
        {
            this._propertyEnum_initialValue[propertyEnumAndValue.PropertyEnumPy] = propertyEnumAndValue.ValuePy;
        }
    }

    public float GetInitialPropertyValue(PropertyEnum propertyEnum)
    {
        this._propertyEnum_initialValue.TryGetValue(propertyEnum,out float value);
        return value;
    }
}
}