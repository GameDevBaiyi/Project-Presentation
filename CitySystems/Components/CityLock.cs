using System;
using System.Collections.Generic;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
[Serializable]
public class CityLock
{
    [Title("Data")]
    [ShowInInspector]
    private readonly CityEnum _cityEnum;

    [ShowInInspector]
    private readonly List<BattleConfigId> _battleListToUnlockLv2;
    public List<BattleConfigId> BattleListToUnlockLv2Py => this._battleListToUnlockLv2;

    public CityLock(CityEnum cityEnum,List<BattleConfigId> battleListToUnlockLv2)
    {
        this._cityEnum = cityEnum;
        this._battleListToUnlockLv2 = battleListToUnlockLv2;
    }

    [ShowInInspector]
    private bool _hasUnlockLv1;
    public bool HasUnlockLv1Py => this._hasUnlockLv1;
    public void SetHasUnlockLv1(bool hasUnlockLv1)
    {
        this._hasUnlockLv1 = hasUnlockLv1;
    }

    [ShowInInspector]
    private int _lv2LockCounter;
    public int Lv2LockCounterPy => this._lv2LockCounter;
    public void SetLv2LockCounter(int lv2LockCounter)
    {
        this._lv2LockCounter = lv2LockCounter;
    }

    [Title("Methods")]
    public bool HasUnlockLv2Py => this._lv2LockCounter >= this._battleListToUnlockLv2.Count;
    public BattleConfig NextBattleConfigPy
    {
        get
        {
            //Debug.
            if (this._lv2LockCounter >= this._battleListToUnlockLv2.Count)
            {
                Debug.LogError($"该 City 的二级锁战斗有: {this._battleListToUnlockLv2.Count} 场, 但是当前的二级锁解锁到了: {this._lv2LockCounter}. 没有下一场战斗了. ");
                return null;
            }
            return this._battleListToUnlockLv2[this._lv2LockCounter].BattleConfigPy;
        }
    }

    public void UnlockLv2All()
    {
        this._lv2LockCounter++;
    }

    public void UnlockAll()
    {
        this.SetHasUnlockLv1(true);
        this.SetLv2LockCounter(this._battleListToUnlockLv2.Count);
    }

    public void LockLv1()
    {
        this.SetHasUnlockLv1(false);
    }
}
}