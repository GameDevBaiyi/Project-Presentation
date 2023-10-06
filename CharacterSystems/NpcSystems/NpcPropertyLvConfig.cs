using System;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
[Serializable]
public class NpcPropertyLvConfig
{
    [SerializeField]
    private PropertyEnum _propertyEnum;
    public PropertyEnum PropertyEnumPy => this._propertyEnum;
    public void SetPropertyEnum(PropertyEnum propertyEnum)
    {
        this._propertyEnum = propertyEnum;
    }

    [SerializeField]
    private float _lvIncreaseFactor;
    public float LvIncreaseFactorPy => this._lvIncreaseFactor;
    public void SetLvIncreaseFactor(float lvIncreaseFactor)
    {
        this._lvIncreaseFactor = lvIncreaseFactor;
    }
}
}