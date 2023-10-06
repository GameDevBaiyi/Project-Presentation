using System;

using LowLevelSystems.LocalizationSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
[Serializable]
public class PropertyConfig
{
    [SerializeField]
    private PropertyEnum _propertyEnum;
    public PropertyEnum PropertyEnumPy => this._propertyEnum;
    public void SetPropertyEnum(PropertyEnum propertyEnum)
    {
        this._propertyEnum = propertyEnum;
    }

    [SerializeField]
    private TextId _propertyNameId;
    public TextId PropertyNameIdPy => this._propertyNameId;
    public void SetPropertyNameId(TextId propertyNameId)
    {
        this._propertyNameId = propertyNameId;
    }

    [SerializeField]
    private int _propertyTypeId;
    public int PropertyTypeIdPy => this._propertyTypeId;
    public void SetPropertyTypeId(int propertyTypeId)
    {
        this._propertyTypeId = propertyTypeId;
    }
}

[Serializable]
public class CharacteristicPropertyConfig
{
    [SerializeField]
    private PropertyEnum _propertyEnum;
    public PropertyEnum PropertyEnumPy => this._propertyEnum;
    public void SetPropertyEnum(PropertyEnum propertyEnum)
    {
        this._propertyEnum = propertyEnum;
    }

    [SerializeField]
    private CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;
    public void SetCharacterEnum(CharacterEnum characterEnum)
    {
        this._characterEnum = characterEnum;
    }

    [SerializeField]
    private TextId _propertyNameId;
    public TextId PropertyNameIdPy => this._propertyNameId;
    public void SetPropertyNameId(TextId propertyNameId)
    {
        this._propertyNameId = propertyNameId;
    }
}
}