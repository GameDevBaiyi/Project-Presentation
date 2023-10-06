using System;
using System.Collections.Generic;

using LowLevelSystems.QualitySystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
[Serializable]
public class StoreLevelConfig
{
    [SerializeField]
    private int _storeLevel;
    public int StoreLevelPy => this._storeLevel;

    [SerializeField]
    private List<int> _weights;
    public List<int> WeightsPy => this._weights;

    [SerializeField]
    private List<QualityEnum> _qualityEnums;
    public List<QualityEnum> QualityEnumsPy => this._qualityEnums;

#region EditorOnly
#if UNITY_EDITOR
    public void SetStoreLevel(int storeLevel)
    {
        this._storeLevel = storeLevel;
    }
    public void SetWeights(List<int> weights)
    {
        this._weights = weights;
    }
    public void SetQualityEnums(List<QualityEnum> qualityEnums)
    {
        this._qualityEnums = qualityEnums;
    }
#endif
#endregion
}
}