using System;
using System.Collections.Generic;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
[Serializable]
public class LibraryOfExtra4Values
{
    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private Dictionary<PropertyEnum,Dictionary<PosOfPropertyFormulaEnum,float>> _propertyEnum_posOfPropertyFormulaEnum_value;
    public Dictionary<PropertyEnum,Dictionary<PosOfPropertyFormulaEnum,float>> PropertyEnum_PosOfPropertyFormulaEnum_ValuePy => this._propertyEnum_posOfPropertyFormulaEnum_value;
    public void SetPropertyEnum_PosOfPropertyFormulaEnum_Value(Dictionary<PropertyEnum,Dictionary<PosOfPropertyFormulaEnum,float>> propertyEnum_posOfPropertyFormulaEnum_value)
    {
        this._propertyEnum_posOfPropertyFormulaEnum_value = propertyEnum_posOfPropertyFormulaEnum_value;
    }

    public LibraryOfExtra4Values(int characterId)
    {
        this.SetCharacterId(characterId);
        this._propertyEnum_posOfPropertyFormulaEnum_value = new Dictionary<PropertyEnum,Dictionary<PosOfPropertyFormulaEnum,float>>(10);
    }

    [Title("Methods")]
    public float this[PropertyEnum propertyEnum,PosOfPropertyFormulaEnum posOfPropertyFormulaEnum]
    {
        get
        {
            if (!this._propertyEnum_posOfPropertyFormulaEnum_value.TryGetValue(propertyEnum,out Dictionary<PosOfPropertyFormulaEnum,float> posOfPropertyFormulaEnum_value))
                return 0f;
            posOfPropertyFormulaEnum_value.TryGetValue(posOfPropertyFormulaEnum,out float currentValue);
            return currentValue;
        }
    }

    public void ChangeProperty(PropertyEnum propertyEnum,PosOfPropertyFormulaEnum posOfPropertyFormulaEnum,float addend)
    {
        if (addend == 0f) return;

        if (!this._propertyEnum_posOfPropertyFormulaEnum_value.TryGetValue(propertyEnum,out Dictionary<PosOfPropertyFormulaEnum,float> _))
        {
            this._propertyEnum_posOfPropertyFormulaEnum_value[propertyEnum] = new Dictionary<PosOfPropertyFormulaEnum,float>() { { posOfPropertyFormulaEnum,0f }, };
        }

        if (!this._propertyEnum_posOfPropertyFormulaEnum_value[propertyEnum].ContainsKey(posOfPropertyFormulaEnum))
        {
            this._propertyEnum_posOfPropertyFormulaEnum_value[propertyEnum][posOfPropertyFormulaEnum] = 0f;
        }

        this._propertyEnum_posOfPropertyFormulaEnum_value[propertyEnum][posOfPropertyFormulaEnum] += addend;
    }

    public void FormatAllTo0()
    {
        foreach (Dictionary<PosOfPropertyFormulaEnum,float> posOfPropertyFormulaEnum_value in this._propertyEnum_posOfPropertyFormulaEnum_value.Values)
        {
            posOfPropertyFormulaEnum_value.Clear();
        }
    }
}
}