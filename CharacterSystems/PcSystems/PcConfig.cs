using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems
{
[Serializable]
public class PcConfig : CharacterConfig
{
    [Serializable]
    public struct ModuleOfSkillBarEnumsWrapper
    {
        [SerializeField]
        private List<ModuleOfSkillBarEnum> _moduleOfSkillBarEnums;
        public List<ModuleOfSkillBarEnum> ModuleOfSkillBarEnumsPy => this._moduleOfSkillBarEnums;
        public void SetModuleOfSkillBarEnums(List<ModuleOfSkillBarEnum> moduleOfSkillBarEnums)
        {
            this._moduleOfSkillBarEnums = moduleOfSkillBarEnums;
        }
    }

    [SerializeField]
    private int _initialMaxInterest;
    public int InitialMaxInterestPy => this._initialMaxInterest;
    public void SetInitialMaxInterest(int initialMaxInterest)
    {
        this._initialMaxInterest = initialMaxInterest;
    }

    //技能相关.
    //技能背包 (已学习) 初始格子数.
    [SerializeField]
    private int _initialCellCountOnLearnedSkillBag;
    public int InitialCellCountOnLearnedSkillBagPy => this._initialCellCountOnLearnedSkillBag;
    public void SetInitialCellCountOnLearnedSkillBag(int initialCellCountOnLearnedSkillBag)
    {
        this._initialCellCountOnLearnedSkillBag = initialCellCountOnLearnedSkillBag;
    }
    
    //技能背包 (已装备). 
    [SerializeField]
    private List<int> _maxCellCountPerRowOnEquippedSkillBag;
    public List<int> MaxCellCountPerRowOnEquippedSkillBagPy => this._maxCellCountPerRowOnEquippedSkillBag;
    public void SetMaxCellCountPerRowOnEquippedSkillBag(List<int> maxCellCountPerRowOnEquippedSkillBag)
    {
        this._maxCellCountPerRowOnEquippedSkillBag = maxCellCountPerRowOnEquippedSkillBag;
    }
    
    //技能背包 (已装备). 
    [SerializeField]
    private List<int> _initialUnlockedCellCountPerRowOnEquippedSkillBag;
    public List<int> InitialUnlockedCellCountPerRowOnEquippedSkillBagPy => this._initialUnlockedCellCountPerRowOnEquippedSkillBag;
    public void SetInitialUnlockedCellCountPerRowOnEquippedSkillBag(List<int> initialUnlockedCellCountPerRowOnEquippedSkillBag)
    {
        this._initialUnlockedCellCountPerRowOnEquippedSkillBag = initialUnlockedCellCountPerRowOnEquippedSkillBag;
    }
    
    //技能背包 (已装备). 初始时有几行解锁了
    [SerializeField]
    private int _initialUnlockedRowCountOnEquippedSkillBag;
    public int InitialUnlockedRowCountOnEquippedSkillBagPy => this._initialUnlockedRowCountOnEquippedSkillBag;
    public void SetInitialUnlockedRowCountOnEquippedSkillBag(int initialUnlockedRowCountOnEquippedSkillBag)
    {
        this._initialUnlockedRowCountOnEquippedSkillBag = initialUnlockedRowCountOnEquippedSkillBag;
    }
    
    //抽技能堆的配置.
    [SerializeField]
    private int _initialPredicatedSlotCount;
    public int InitialPredicatedSlotCountPy => this._initialPredicatedSlotCount;
    public void SetInitialPredicatedSlotCount(int initialPredicatedSlotCount)
    {
        this._initialPredicatedSlotCount = initialPredicatedSlotCount;
    }

    [SerializeField]
    private List<ModuleOfSkillBarEnumsWrapper> _moduleOfSkillBarConfig = new List<ModuleOfSkillBarEnumsWrapper>();
    public List<ModuleOfSkillBarEnumsWrapper> ModuleOfSkillBarConfigPy => this._moduleOfSkillBarConfig;
    public void SetModuleOfSkillBarConfig(List<ModuleOfSkillBarEnumsWrapper> moduleOfSkillBarConfig)
    {
        this._moduleOfSkillBarConfig = moduleOfSkillBarConfig;
    }

    [SerializeField]
    private int _initialCountOfLuggageSlots;
    public int InitialCountOfLuggageSlotsPy => this._initialCountOfLuggageSlots;
    public void SetInitialCountOfLuggageSlots(int initialCountOfLuggageSlots)
    {
        this._initialCountOfLuggageSlots = initialCountOfLuggageSlots;
    }

    [SerializeField]
    private List<TalentBookConfig> _talentBookConfigs = new List<TalentBookConfig>();
    public List<TalentBookConfig> TalentBookConfigsPy => this._talentBookConfigs;
    public void SetTalentBookConfigs(List<TalentBookConfig> talentBookConfigs)
    {
        this._talentBookConfigs = talentBookConfigs;
    }
}
}