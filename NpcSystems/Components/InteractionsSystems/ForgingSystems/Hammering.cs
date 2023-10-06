using System;

using Common.Extensions;

using LowLevelSystems.ItemSystems.EquipmentSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class Hammering
{
    public enum HammerActionEnum
    {
        None,
        Grind,
        Knock,
        Repair,
    }

    public enum IntensityEnum
    {
        None,
        Big,
        Small,
    }

    [ShowInInspector]
    private EquipmentEmbryo _equipmentEmbryo;
    public EquipmentEmbryo EquipmentEmbryoPy => this._equipmentEmbryo;
    public void SetEquipmentEmbryo(EquipmentEmbryo equipmentEmbryo)
    {
        this._equipmentEmbryo = equipmentEmbryo;
    }

    [ShowInInspector]
    private float _feng;
    public float FengPy => this._feng;
    public void SetFeng(float feng)
    {
        this._feng = feng;
    }

    [ShowInInspector]
    private Vector2 _perfectRangeForFeng;
    public Vector2 PerfectRangeForFengPy => this._perfectRangeForFeng;
    public void SetPerfectRangeForFeng(Vector2 perfectRangeForFeng)
    {
        this._perfectRangeForFeng = perfectRangeForFeng;
    }

    [ShowInInspector]
    private Vector2 _excellentRangeForFeng;
    public Vector2 ExcellentRangeForFengPy => this._excellentRangeForFeng;
    public void SetExcellentRangeForFeng(Vector2 excellentRangeForFeng)
    {
        this._excellentRangeForFeng = excellentRangeForFeng;
    }

    [ShowInInspector]
    private float _ren;
    public float RenPy => this._ren;
    public void SetRen(float ren)
    {
        this._ren = ren;
    }

    [ShowInInspector]
    private Vector2 _perfectRangeForRen;
    public Vector2 PerfectRangeForRenPy => this._perfectRangeForRen;
    public void SetPerfectRangeForRen(Vector2 perfectRangeForRen)
    {
        this._perfectRangeForRen = perfectRangeForRen;
    }

    [ShowInInspector]
    private Vector2 _excellentRangeForRen;
    public Vector2 ExcellentRangeForRenPy => this._excellentRangeForRen;
    public void SetExcellentRangeForRen(Vector2 excellentRangeForRen)
    {
        this._excellentRangeForRen = excellentRangeForRen;
    }

    [ShowInInspector]
    private int _hammerTimes;
    public int HammerTimesPy => this._hammerTimes;
    public void SetHammerTimes(int hammerTimes)
    {
        this._hammerTimes = hammerTimes;
    }

    [ShowInInspector]
    private IntensityEnum _intensityEnum;
    public IntensityEnum IntensityEnumPy => this._intensityEnum;
    public void SetIntensityEnum(IntensityEnum intensityEnum)
    {
        this._intensityEnum = intensityEnum;
    }

    [Title("Methods")]
    public ForgingScore ForgingScorePy
    {
        get
        {
            //先计算 锋 得分.
            int scoreOfFeng = 0;
            if (this._feng.IsInRange(this._perfectRangeForFeng.x,this._perfectRangeForFeng.y,ExclusiveFlags.None))
            {
                scoreOfFeng = 2;
            }
            else if (this._feng.IsInRange(this._excellentRangeForFeng.x,this._excellentRangeForFeng.y,ExclusiveFlags.None))
            {
                scoreOfFeng = 1;
            }

            //计算 韧 得分.
            int scoreOfRen = 0;
            if (this._ren.IsInRange(this._perfectRangeForRen.x,this._perfectRangeForRen.y,ExclusiveFlags.None))
            {
                scoreOfRen = 2;
            }
            else if (this._ren.IsInRange(this._excellentRangeForRen.x,this._excellentRangeForRen.y,ExclusiveFlags.None))
            {
                scoreOfRen = 1;
            }

            return new ForgingScore(scoreOfFeng + scoreOfRen);
        }
    }

    // 三种行为都是对 锋 和 韧 造成影响. 
    public void UsingHammerAction(HammerActionEnum hammerActionEnum)
    {
        HammerActionConfig hammerActionConfig = hammerActionEnum.HammerActionConfig(this._intensityEnum);

        //改变锋 韧 值.
        int fengAdded = Random.Range(hammerActionConfig.RangeOfFengAddendPy.x,hammerActionConfig.RangeOfFengAddendPy.y + 1);
        this._feng += fengAdded;
        int renAdded = Random.Range(hammerActionConfig.RangeOfRenAddendPy.x,hammerActionConfig.RangeOfRenAddendPy.y + 1);
        this._ren += renAdded;

        //检测失败的结果.
        if (!this._feng.IsInRange(0,100,ExclusiveFlags.None)
         || !this._ren.IsInRange(0,100,ExclusiveFlags.None))
        {
            Debug.LogError("锻造失败了, 但暂时无对应的行为.");
        }
    }
}
}