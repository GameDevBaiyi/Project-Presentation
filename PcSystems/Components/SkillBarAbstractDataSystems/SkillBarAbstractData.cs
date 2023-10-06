using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.SkillBarAbstractDataSystems
{
[Serializable]
public class SkillBarAbstractData
{
    [Title("Config")]
    private const int _maxNumberOfPredicatingSlots = 3;
    private const int _maxNumberOfSkillBarStages = 3;

    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private int _countOfPredicatingSlots;
    public int CountOfPredicatingSlotsPy => this._countOfPredicatingSlots;
    public void SetCountOfPredicatingSlots(int countOfPredicatingSlots)
    {
        this._countOfPredicatingSlots = countOfPredicatingSlots;
    }

    [ShowInInspector]
    private int _currentStageOfSkillBar;
    public int CurrentStageOfSkillBarPy => this._currentStageOfSkillBar;
    public void SetCurrentStageOfSkillBar(int currentStageOfSkillBar)
    {
        this._currentStageOfSkillBar = currentStageOfSkillBar;
    }

    public SkillBarAbstractData(int characterId,int initialCountOfPredicatingSlots)
    {
        this.SetCharacterId(characterId);
        this._countOfPredicatingSlots = initialCountOfPredicatingSlots;
        //初始设置为第一阶段.
        this.SetCurrentStageOfSkillBar(1);
    }

    [Title("Methods")]
    public void AddPredicatingSlots(int addend)
    {
        if (addend < 0)
        {
            Debug.LogError($"抽技能堆的 预示槽 数量不能减少, 加数为 {addend}.");
            return;
        }
        if (addend == 0) return;

        this._countOfPredicatingSlots = Mathf.Clamp(this._countOfPredicatingSlots + addend,0,_maxNumberOfPredicatingSlots);
    }

    public void UnlockSkillBarToNextStage()
    {
        //Debug.
        if (this._currentStageOfSkillBar >= _maxNumberOfSkillBarStages)
        {
            Debug.LogError($"该角色: {this._characterId.CharacterPy.CharacterEnumPy.PcConfig().CharacterNamePy}的技能栏阶段已经到达了最大值, 仍在向下一阶段解锁, 是否有错? ");
            return;
        }
        this._currentStageOfSkillBar++;
    }
}
}