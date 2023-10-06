using System;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
[Serializable]
public class CityJurisdictionSystem
{
    [Title("Config")]
    private const int _maxJurisdictionValue = 1000;

    [Title("Data")]
    [ShowInInspector]
    private readonly CityEnum _cityEnum;
    public CityEnum CityEnumPy => this._cityEnum;

    [ShowInInspector]
    private int _currentJurisdictionValue;
    public int CurrentJurisdictionValuePy => this._currentJurisdictionValue;

    [ShowInInspector]
    private CampEnum _currentCamp;
    public CampEnum CurrentCampPy => this._currentCamp;

    public CityJurisdictionSystem(CityEnum cityEnum,CampEnum initialCampEnum)
    {
        this._cityEnum = cityEnum;
        this._currentJurisdictionValue = _maxJurisdictionValue;
        this._currentCamp = initialCampEnum;
    }

    /// <summary>
    /// 改变管辖值. 当管辖值变为 0 时, 切换城镇管辖权.
    /// </summary>
    [Title("Methods")]
    public void ChangeLimitedValue(int addend)
    {
        int targetValue = this._currentJurisdictionValue + addend;
        this._currentJurisdictionValue = Mathf.Clamp(targetValue,0,_maxJurisdictionValue);
        if (this._currentJurisdictionValue <= 0)
        {
            DetailsOfCityJurisdictionSystem.SwitchCampAsync(this);
        }
    }

    /// <summary>
    /// 改变管辖阵营. 并将管辖值恢复到最大值.
    /// </summary>
    public void ChangeCamp()
    {
        this._currentCamp = this._currentCamp == CampEnum.Sun ? CampEnum.Moon : CampEnum.Sun;
        this._currentJurisdictionValue = _maxJurisdictionValue;
    }
}
}