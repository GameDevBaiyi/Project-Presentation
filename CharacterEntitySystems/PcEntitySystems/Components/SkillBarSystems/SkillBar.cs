using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.SkillBarAbstractDataSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public class SkillBar
{
    [Title("Data")]
    [ShowInInspector]
    private PcEntity _pcEntity;
    public PcEntity PcEntityPy => this._pcEntity;
    public void SetPcEntity(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
    }

    [ShowInInspector]
    private List<ModuleOfSkillBar> _modulesOfSkillBar;
    public List<ModuleOfSkillBar> ModulesOfSkillBarPy => this._modulesOfSkillBar;
    public void SetModulesOfSkillBar(List<ModuleOfSkillBar> modulesOfSkillBar)
    {
        this._modulesOfSkillBar = modulesOfSkillBar;
    }

    public SkillBar(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
        this.SetModulesOfSkillBar(new List<ModuleOfSkillBar>(5));
    }

    /// <summary>
    /// 进入战斗时要生成 技能栏模组, 并添加满技能糖.
    /// </summary>
    [Title("Methods")]
    public void ResetSkillBar()
    {
        //生成 技能栏模组
        this._modulesOfSkillBar.Clear();
        Pc pc = this._pcEntity.PcPy;
        SkillBarAbstractData skillBarAbstractData = pc.SkillBarAbstractDataPy;
        int currentStageOfSkillBar = skillBarAbstractData.CurrentStageOfSkillBarPy;
        PcConfig.ModuleOfSkillBarEnumsWrapper moduleOfSkillBarEnumsWrapper = pc.CharacterEnumPy.PcConfig().ModuleOfSkillBarConfigPy.ElementAtOrDefault(currentStageOfSkillBar - 1);
        if (moduleOfSkillBarEnumsWrapper.ModuleOfSkillBarEnumsPy == null)
        {
            Debug.LogError($"该角色: {pc.CharacterEnumPy.PcConfig().CharacterNamePy} 的技能栏阶段是: {currentStageOfSkillBar}, 但是没有对应的配置. ");
            return;
        }
        List<ModuleOfSkillBarEnum> moduleOfSkillBarEnums = moduleOfSkillBarEnumsWrapper.ModuleOfSkillBarEnumsPy;
        foreach (ModuleOfSkillBarEnum moduleOfSkillBarEnum in moduleOfSkillBarEnums)
        {
            this._modulesOfSkillBar.Add(new ModuleOfSkillBar(this._pcEntity,moduleOfSkillBarEnum));
        }

        //并添加满技能糖.
        foreach (ModuleOfSkillBar moduleOfSkillBar in this._modulesOfSkillBar)
        {
            moduleOfSkillBar.FillSugars(out _);
        }
    }
}
}