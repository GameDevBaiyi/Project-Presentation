using System;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
[Serializable]
public class CityExploreSystem
{
    /// <summary>
    /// 最大探索等级. 
    /// </summary>
    [Title("Config")]
    private const int _maxExplorationLevel = 20;
    /// <summary>
    /// 最大探索值.
    /// </summary>
    private const int _maxExplorationValue = 1000;

    [Title("Data")]
    [ShowInInspector]
    private CityLevel _cityLevel;
    public CityLevel CityLevelPy => this._cityLevel;
    public void SetCityLevel(int cityLevel)
    {
        this._cityLevel.Level = cityLevel;
    }

    /// <summary>
    /// 当前探索值. 
    /// </summary>
    [ShowInInspector]
    private int _currentExplorationValue;
    public int CurrentExplorationValuePy => this._currentExplorationValue;

    /// <summary>
    /// 改变探索值.
    /// </summary>
    [Title("Methods")]
    public void ChangeExplorationValue(int value)
    {
        int wholeValue = this._currentExplorationValue + value;
        int targetLevel = this._cityLevel.Level + wholeValue / _maxExplorationValue;

        this._currentExplorationValue = wholeValue % _maxExplorationValue;

        if (targetLevel > _maxExplorationLevel)
        {
            this.SetCityLevel(_maxExplorationLevel);
            this._currentExplorationValue = _maxExplorationValue;
        }
    }
}
}