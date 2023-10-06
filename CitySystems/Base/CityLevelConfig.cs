using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;
using LowLevelSystems.QualitySystems;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
[Serializable]
public class CityLevelConfig
{
    [SerializeField]
    private int _cityLevel;
    public int CityLevelPy => this._cityLevel;
    public void SetCityLevel(int cityLevel)
    {
        this._cityLevel = cityLevel;
    }

    [SerializeField]
    private StoreLevel _storeLevel;
    public StoreLevel StoreLevelPy => this._storeLevel;
    public void SetStoreLevel(StoreLevel storeLevel)
    {
        this._storeLevel = storeLevel;
    }

    [SerializeField]
    private QualityEnum _qualityEnumOfStolenItem;
    public QualityEnum QualityEnumOfStolenItemPy => this._qualityEnumOfStolenItem;
    public void SetQualityEnumOfStolenItem(QualityEnum qualityEnumOfStolenItem)
    {
        this._qualityEnumOfStolenItem = qualityEnumOfStolenItem;
    }

    [SerializeField]
    private Vector2Int _rangeOfBountyTasks;
    public Vector2Int RangeOfBountyTasksPy => this._rangeOfBountyTasks;
    public void SetRangeOfBountyTasks(Vector2Int rangeOfBountyTasks)
    {
        this._rangeOfBountyTasks = rangeOfBountyTasks;
    }

    [SerializeField]
    private List<int> _weightsOfBountyTaskLevel;
    public List<int> WeightsOfBountyTaskLevelPy => this._weightsOfBountyTaskLevel;
    public void SetWeightsOfBountyTaskLevel(List<int> weightsOfBountyTaskLevel)
    {
        this._weightsOfBountyTaskLevel = weightsOfBountyTaskLevel;
    }
}
}