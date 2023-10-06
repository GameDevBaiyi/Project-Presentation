using System;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class HammerActionConfig
{
    [SerializeField]
    private Hammering.HammerActionEnum _hammerActionEnum;
    public Hammering.HammerActionEnum HammerActionEnumPy => this._hammerActionEnum;
    public void SetHammerActionEnum(Hammering.HammerActionEnum hammerActionEnum)
    {
        this._hammerActionEnum = hammerActionEnum;
    }

    [SerializeField]
    private Hammering.IntensityEnum _intensityEnum;
    public Hammering.IntensityEnum IntensityEnumPy => this._intensityEnum;
    public void SetIntensityEnum(Hammering.IntensityEnum intensityEnum)
    {
        this._intensityEnum = intensityEnum;
    }

    [SerializeField]
    private Vector2Int _rangeOfFengAddend;
    public Vector2Int RangeOfFengAddendPy => this._rangeOfFengAddend;
    public void SetRangeOfFengAddend(Vector2Int rangeOfFengAddend)
    {
        this._rangeOfFengAddend = rangeOfFengAddend;
    }

    [SerializeField]
    private Vector2Int _rangeOfRenAddend;
    public Vector2Int RangeOfRenAddendPy => this._rangeOfRenAddend;
    public void SetRangeOfRenAddend(Vector2Int rangeOfRenAddend)
    {
        this._rangeOfRenAddend = rangeOfRenAddend;
    }
}
}