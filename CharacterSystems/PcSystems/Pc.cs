using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.EquipmentInventorySystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.MissionActionsRecorderSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.SkillBarAbstractDataSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;
using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

// ReSharper disable InconsistentNaming

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace LowLevelSystems.CharacterSystems.PcSystems
{
/// <summary>
/// 玩家角色. 
/// </summary>
[Serializable]
public class Pc : Character,ICanRefreshOnDateChanged
{
    [ShowInInspector]
    [NonSerialized]
    [CanBeNull]
    private PcConfig _pcConfig;
    public PcConfig PcConfigPy => this._pcConfig ??= this._characterEnum.PcConfig();

    [ShowInInspector]
    private bool _hasJoined;
    public bool HasJoinedPy => this._hasJoined;
    public void SetHasJoined(bool hasJoined)
    {
        this._hasJoined = hasJoined;
    }

    //记录之前走过的入口, 用于下次从此处出去.
    [ShowInInspector]
    private Dictionary<int,Vector3Int> _sceneId_entranceCoord;
    public Dictionary<int,Vector3Int> SceneId_EntranceCoordPy => this._sceneId_entranceCoord;
    public void SetSceneId_EntranceCoord(Dictionary<int,Vector3Int> sceneId_entranceCoord)
    {
        this._sceneId_entranceCoord = sceneId_entranceCoord;
    }

    [Title("任务记录")]
    [ShowInInspector]
    private MissionActionsRecorder _missionActionsRecorder;
    public MissionActionsRecorder MissionActionsRecorderPy => this._missionActionsRecorder;
    public void SetMissionActionsRecorder(MissionActionsRecorder missionActionsRecorder)
    {
        this._missionActionsRecorder = missionActionsRecorder;
    }
    [Title("兴致")]
    [ShowInInspector]
    private InterestSystem _interestSystem;
    public InterestSystem InterestSystemPy => this._interestSystem;
    public void SetInterestSystem(InterestSystem interestSystem)
    {
        this._interestSystem = interestSystem;
    }
    [Title("技能背包 (已学习)")]
    [ShowInInspector]
    private BagOfLearnedSkill _bagOfLearnedSkill;
    public BagOfLearnedSkill BagOfLearnedSkillPy => this._bagOfLearnedSkill;
    public void SetLearnedSkillBag(BagOfLearnedSkill bagOfLearnedSkill)
    {
        this._bagOfLearnedSkill = bagOfLearnedSkill;
    }
    [Title("技能背包 (已装备)")]
    [ShowInInspector]
    private BagOfEquippedSkill _bagOfEquippedSkill;
    public BagOfEquippedSkill BagOfEquippedSkillPy => this._bagOfEquippedSkill;
    public void SetEquippedSkillBag(BagOfEquippedSkill bagOfEquippedSkill)
    {
        this._bagOfEquippedSkill = bagOfEquippedSkill;
    }

    [Title("技能栏 抽象数据")]
    [ShowInInspector]
    private SkillBarAbstractData _skillBarAbstractData;
    public SkillBarAbstractData SkillBarAbstractDataPy => this._skillBarAbstractData;
    public void SetSkillBarAbstractData(SkillBarAbstractData skillBarAbstractData)
    {
        this._skillBarAbstractData = skillBarAbstractData;
    }

    [Title("行囊")]
    [ShowInInspector]
    private Luggage _luggage;
    public Luggage LuggagePy => this._luggage;
    public void SetLuggage(Luggage luggage)
    {
        this._luggage = luggage;
    }

    [Title("装备栏")]
    [ShowInInspector]
    private EquipmentInventory _equipmentInventory;
    public EquipmentInventory EquipmentInventoryPy => this._equipmentInventory;
    public void SetEquipmentInventory(EquipmentInventory equipmentInventory)
    {
        this._equipmentInventory = equipmentInventory;
    }

    [Title("天赋系统")]
    [ShowInInspector]
    private TalentSystem _talentSystem;
    public TalentSystem TalentSystemPy => this._talentSystem;
    public void SetTalentSystem(TalentSystem talentSystem)
    {
        this._talentSystem = talentSystem;
    }

    [Title("偷窃交互相关")]
    [ShowInInspector]
    private int _lastDayRefreshed = -1;
    public int LastDayRefreshedPy => this._lastDayRefreshed;
    [ShowInInspector]
    public int RefreshCyclePy => Details.SettingsSo.StealRefreshCycle;
    public void CheckAndRefresh()
    {
        if (!ICanRefreshOnDateChanged.IsTimeToRefresh(this)) return;
        this._timesTheftWasDetected = 0;
        this._lastDayRefreshed = Details.DateSystem.DaysPy;
    }
    [ShowInInspector]
    private int _timesTheftWasDetected;
    public int TimesTheftWasDetectedPy => this._timesTheftWasDetected;
    public void AddTimesTheftWasDetected()
    {
        this.CheckAndRefresh();
        this._timesTheftWasDetected++;
    }

    public void OnLeaveBattle()
    {
        this._propertySystem.RemoveLibraryOfExtra4ValuesForBuff();
        //死亡的角色血量恢复1, 其他血量回复满. 
        bool isAlive = this._propertySystem.IsAlivePy;
        if (isAlive)
        {
            this._propertySystem.HealAll();
        }
        else
        {
            this._propertySystem.SetCurrentHp(1f);
            this._propertySystem.SetIsAlive(true);
        }
    }
}
}