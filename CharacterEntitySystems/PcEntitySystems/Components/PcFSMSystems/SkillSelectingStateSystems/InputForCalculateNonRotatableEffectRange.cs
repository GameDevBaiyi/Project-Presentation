using System.Collections.Generic;

using Common.InputFrame;

using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
/// <summary>
/// 接收输入: 鼠标在技能范围内移动改变中心, 也就改变生效范围.
/// 只有以鼠标为中心的操作才需要此输入.
/// </summary>
public class InputForCalculateNonRotatableEffectRange : Details
{
    [Title("Data")]
    [ShowInInspector]
    private MouseGridAction _input;
    public MouseGridAction InputPy => this._input;
    public void SetInput(MouseGridAction input)
    {
        this._input = input;
    }

    [ShowInInspector]
    private bool _isReceivingInput;
    public bool IsReceivingInputPy => this._isReceivingInput;
    public void SetIsReceivingInput(bool isReceivingInput)
    {
        this._isReceivingInput = isReceivingInput;
    }

    public InputForCalculateNonRotatableEffectRange()
    {
        this._input = new MouseGridAction(this.HandleInput);
    }

    [Title("Methods")]
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

        //如果鼠标活动在技能范围外面, 清理中心点, 清理 实际生效范围, 关闭 UI, 关闭 控制技能释放的 Input .
        Vector3Int mouseGridCoord = _inputManager.MouseCoordPy;
        if (!selectingSkillState.SkillUsingRangeSetPy.Contains(mouseGridCoord))
        {
            //清理中心点
            selectingSkillState.SetEffectCenter(null);
            //清理 实际生效范围
            List<List<Vector3Int>> effectRangeList = selectingSkillState.EffectRangeListPy;
            int countOfEffectRangeList = effectRangeList.Count;
            int selectingRoundIndex = selectingSkillState.SelectingRoundIndexPy.Value;
            if (countOfEffectRangeList > selectingRoundIndex)
            {
                effectRangeList.RemoveAt(selectingRoundIndex);
            }
            //关闭 UI
            UiManager.CellUiShowerForSkillEffectRangePy.Hide();
            //控制技能释放的 Input
            selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(false);
            return;
        }

        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        //在技能范围内活动, 设置其为 中心点, 不能旋转则 角色方向 为方向. 计算并显示 UI, 打开 控制技能释放的 Input.
        selectingSkillState.SetEffectCenter(mouseGridCoord);
        selectingSkillState.SetEffectDirectionIndex(currentPc.CoordSystemPy.DirectionIndexPy);
        SelectingSkillStateDetails.RecalculateEffectRange();
        selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(true);
    }
}
}