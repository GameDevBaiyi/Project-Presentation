using Common.InputFrame;
using Common.Utilities;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
/// <summary>
/// 接收输入: 当鼠标移动时, 以计算好的 Center 和 鼠标位置计算 Direction, 并计算 生效范围.
/// 同时适用于 以 Pc 为中心的可旋转和 以鼠标点击设置的中心点 为中心的可旋转.
/// </summary>
public class InputForCalculateRotatableEffectRange : Details
{
    [Title("Data")]
    [ShowInInspector]
    private readonly MouseGridAction _input;
    public MouseGridAction InputPy => this._input;

    [ShowInInspector]
    private bool _isReceivingInput;
    public bool IsReceivingInputPy => this._isReceivingInput;
    public void SetIsReceivingInput(bool isReceivingInput)
    {
        this._isReceivingInput = isReceivingInput;
    }

    public InputForCalculateRotatableEffectRange()
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

        //根据鼠标的位置和中心点的位置, 计算方向.
        Vector3Int mouseOffsetCoord = _inputManager.MouseCoordPy;
        Vector3Int effectCenter = selectingSkillState.EffectCenterPy.Value;
        int directionIndex = CubeUtilities.CalculateDirection(OffsetUtilities.OffsetToCube(mouseOffsetCoord) - OffsetUtilities.OffsetToCube(effectCenter));
        selectingSkillState.SetEffectDirectionIndex(directionIndex);

        //显示 生效范围 并计算 实际生效范围.
        SelectingSkillStateDetails.RecalculateEffectRange();
    }
}
}