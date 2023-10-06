using Common.InputFrame;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using Sirenix.OdinInspector;

using UnityEngine;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
/// <summary>
/// 选择完一轮范围后, 可能会释放技能也可能会开启下一轮选择.
/// </summary>
public class InputForBeginNextSelectingRoundOrUseSkill : Details
{
    [Title("Data")]
    [ShowInInspector]
    private KeyDownAction _input;
    public KeyDownAction InputPy => this._input;
    public void SetInput(KeyDownAction input)
    {
        this._input = input;
    }

    [Title("Methods")]
    [ShowInInspector]
    private bool _isReceivingInput;
    public bool IsReceivingInputPy => this._isReceivingInput;
    public void SetIsReceivingInput(bool isReceivingInput)
    {
        this._isReceivingInput = isReceivingInput;
    }

    public InputForBeginNextSelectingRoundOrUseSkill()
    {
        this._input = new KeyDownAction(KeyCode.Mouse0,this.HandleInput);
    }

    private void HandleInput()
    {
        if (!this._isReceivingInput) return;

        //做一个检测, 如果该行为, 不在技能选择状态, 就报错. 
        SelectingSkillState selectingSkillState = InputFSM.InputStateEnum.SelectingSkill.InputState() as SelectingSkillState;
        if (!selectingSkillState.IsInStatePy)
        {
            Debug.LogError($"未在技能输入状态.");
            return;
        }

        //如果点在 UI 上, 不管.
        if (_inputManager.IsOnUIPy) return;

        // Debug. 进入技能释放, 无论如何都应该计算好了这一轮的 技能生效范围.
        int countOfEffectRangeList = selectingSkillState.EffectRangeListPy.Count;
        int selectingRoundIndex = selectingSkillState.SelectingRoundIndexPy.Value;
        if (countOfEffectRangeList <= selectingRoundIndex)
        {
            Debug.LogError($"当前的范围 List 个数: {countOfEffectRangeList}, 当前正在第 {selectingRoundIndex} 轮.");
            return;
        }

        //如果配置中还有下一轮, 那么就开启下一轮检测.
        SkillSugarConfig skillSugarConfig = selectingSkillState.SkillSugarConfigPy;
        if (skillSugarConfig.SkillEffectConfigPy.Count > selectingRoundIndex + 1)
        {
            SelectingSkillStateDetails.EnterNextSelectingRound();
            return;
        }

        //看看 Pc 的 Ap 是否足够.
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        if (currentPc.PropertySystemPy[PropertyEnum.MaxAp] < skillSugarConfig.CostApPy)
        {
            UiManager.PromptOnMousePosPy.Show($"Ap 不足.");
            return;
        }

        //应用技能效果, 并且执行动画等.
        DetailsOfSkillEffects.ApplySkillEffectsAndDoAnime(currentPc.CharacterIdPy.CharacterEntityPy,selectingSkillState.EffectCenterPy.Value,selectingSkillState.EffectRangeListPy,skillSugarConfig,
                                                         selectingSkillState.EffectDirectionIndexPy.Value);

        //普通状态.
        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.Normal);
        //再尝试自动进入移动, 简化操作. 
        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.MovingInBattle);
    }
}
}